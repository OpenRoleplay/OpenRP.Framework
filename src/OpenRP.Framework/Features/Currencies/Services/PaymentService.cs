using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Currencies.Components;
using OpenRP.Framework.Features.Currencies.Entities;
using OpenRP.Framework.Features.Inventories.Components;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Framework.Shared.Chat.Services;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;
using OpenRP.Framework.Shared.ServerEvents.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Currencies.Services
{
    public class PaymentService : IPaymentService
    {
        private const float MaxPaymentDistance = 3.0f;

        private readonly ICurrencyService _currencyService;
        private readonly IChatService _chatService;
        private readonly IEntityManager _entityManager;
        private readonly IServerEventAggregator _serverEventAggregator;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(ICurrencyService currencyService, IChatService chatService, IEntityManager entityManager, IServerEventAggregator serverEventAggregator, ILogger<PaymentService> logger)
        {
            _currencyService = currencyService;
            _chatService = chatService;
            _entityManager = entityManager;
            _serverEventAggregator = serverEventAggregator;
            _logger = logger;
        }

        public void RequestPayment(Player sender, Player receiver, string currencyCode, decimal amount)
        {
            CurrencyModel? currency = _currencyService.GetCurrencyByCode(currencyCode);
            string? error = GetPaymentRequestError(sender, receiver, currency, amount);
            if (error != null)
            {
                sender.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, error);
                return;
            }

            Inventory senderWallet = sender.GetPlayerCurrentlyPlayingAsCharacter().GetWallet()!;
            Inventory receiverWallet = receiver.GetPlayerCurrentlyPlayingAsCharacter().GetWallet()!;
            List<BillStack>? bills = _currencyService.GetExactBillsFromWallet(senderWallet, currency!.Id, amount);
            string? billsError = GetBillsError(bills, receiverWallet);
            if (billsError != null)
            {
                sender.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, billsError);
                return;
            }

            sender.AddComponent<PendingPayment>(receiver, currency.Id, amount, bills!);
            SendPaymentRequestedMessages(sender, receiver, currency.Id, amount);
        }

        public void AcceptPayment(Player receiver)
        {
            PendingPayment? payment = GetIncomingPayment(receiver);
            string? error = GetPaymentAcceptError(receiver, payment);
            if (error != null)
            {
                receiver.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, error);
                return;
            }

            payment!.Accept();
            receiver.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, "You have accepted the transaction. It will now start.");
            payment.GetSender().SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"{GetCharacterName(receiver)} has accepted the transaction. It will now start.");
        }

        public async Task ContinueAcceptedPayments()
        {
            foreach (PendingPayment payment in GetAcceptedPayments())
            {
                // An earlier payment's events may have let a player leave, which destroys their payment.
                if (!payment.IsComponentAlive)
                {
                    continue;
                }

                try
                {
                    await ContinuePayment(payment);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to continue a payment.");
                    StopPayment(payment, "The transaction was cancelled because something went wrong.");
                }
            }
        }

        public void CancelPayment(Player player)
        {
            PendingPayment? payment = GetPaymentInvolving(player);
            if (payment != null)
            {
                StopPayment(payment, "The transaction was cancelled.");
            }
        }

        private string? GetPaymentRequestError(Player sender, Player receiver, CurrencyModel? currency, decimal amount)
        {
            if (sender == receiver)
                return "You can't transfer money to yourself!";
            if (!sender.IsPlayerPlayingAsCharacter() || !receiver.IsPlayerPlayingAsCharacter())
                return "Either you or the recipient is not logged into their character.";
            if (!IsNearby(sender, receiver))
                return "You must be near the player in order to pay them!";
            if (currency == null)
                return "You must enter a valid currency code!";
            if (amount <= 0)
                return "Transfer amount must be positive!";
            if (GetPaymentInvolving(sender) != null || GetPaymentInvolving(receiver) != null)
                return "Either you or the recipient is already in a transaction!";
            return GetInventoryOpenedError(sender, receiver);
        }

        private static string? GetBillsError(List<BillStack>? bills, Inventory receiverWallet)
        {
            if (bills == null)
                return "You do not have the right bills to pay exactly this amount!";
            if (bills.Sum(stack => stack.GetWeight()) >= receiverWallet.GetAvailableWeight())
                return "The recipient does not have enough space in their inventory!";
            return null;
        }

        private string? GetPaymentAcceptError(Player receiver, PendingPayment? payment)
        {
            if (payment == null)
                return "You currently do not have a pending transaction request!";
            if (payment.IsAccepted())
                return "You have already accepted this transaction!";
            if (!IsNearby(receiver, payment.GetSender()))
                return "You must be near the player in order to accept the transaction!";
            return GetInventoryOpenedError(payment.GetSender(), receiver);
        }

        private static string? GetInventoryOpenedError(Player sender, Player receiver)
        {
            if (sender.GetComponent<InventoryOpened>() != null)
                return "The sender has their inventory open!";
            if (receiver.GetComponent<InventoryOpened>() != null)
                return "The recipient has their inventory open!";
            return null;
        }

        private static bool IsNearby(Player player, Player otherPlayer)
        {
            return player.IsInRangeOfPoint(MaxPaymentDistance, otherPlayer.Position);
        }

        private PendingPayment? GetIncomingPayment(Player receiver)
        {
            return _entityManager.GetComponents<PendingPayment>().FirstOrDefault(payment => payment.GetReceiver() == receiver);
        }

        private PendingPayment? GetPaymentInvolving(Player player)
        {
            return _entityManager.GetComponents<PendingPayment>().FirstOrDefault(payment => payment.IsPlayerInvolved(player));
        }

        private List<PendingPayment> GetAcceptedPayments()
        {
            return _entityManager.GetComponents<PendingPayment>().Where(payment => payment.IsAccepted()).ToList();
        }

        private async Task ContinuePayment(PendingPayment payment)
        {
            if (!payment.HasMoreBills())
            {
                await EndPayment(payment);
                return;
            }

            if (payment.IsReceiverTurn())
            {
                ReceiveBill(payment);
            }
            else if (!HandOverBill(payment))
            {
                StopPayment(payment, "The transaction was cancelled because the bills could not be handed over.");
                return;
            }
            payment.EndTurn();
        }

        private bool HandOverBill(PendingPayment payment)
        {
            Character sender = payment.GetSender().GetPlayerCurrentlyPlayingAsCharacter();
            Character receiver = payment.GetReceiver().GetPlayerCurrentlyPlayingAsCharacter();
            Inventory? senderWallet = sender.GetWallet();
            Inventory? receiverWallet = receiver.GetWallet();
            if (senderWallet == null || receiverWallet == null)
            {
                return false;
            }

            BillStack bill = payment.GetCurrentBill();
            if (!_currencyService.TransferBills(senderWallet, receiverWallet, bill))
            {
                return false;
            }

            SendHandOverMessage(payment, sender, receiver, bill);
            payment.GetSender().ApplyAnimation("DEALER", "shop_pay", 4.1f, false, true, true, true, 0);
            return true;
        }

        private void ReceiveBill(PendingPayment payment)
        {
            Player receiverPlayer = payment.GetReceiver();
            Character receiver = receiverPlayer.GetPlayerCurrentlyPlayingAsCharacter();
            SendReceiveMessage(receiverPlayer, receiver, payment.GetCurrentBill());
            receiverPlayer.ApplyAnimation("BAR", "Barcustom_get", 4.1f, false, true, true, true, 0);
        }

        private async Task EndPayment(PendingPayment payment)
        {
            Player sender = payment.GetSender();
            Player receiver = payment.GetReceiver();
            payment.Destroy();

            sender.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, "The transaction has completed.");
            receiver.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, "The transaction has completed.");
            sender.ClearAnimations();
            receiver.ClearAnimations();

            await PublishCashUpdate(sender, payment.GetCurrencyId());
            await PublishCashUpdate(receiver, payment.GetCurrencyId());
        }

        /// <summary>
        /// Ends the payment early and tells both players why.
        /// </summary>
        private static void StopPayment(PendingPayment payment, string reason)
        {
            Player sender = payment.GetSender();
            Player receiver = payment.GetReceiver();
            payment.Destroy();

            SendPaymentStoppedMessage(sender, reason);
            SendPaymentStoppedMessage(receiver, reason);
        }

        private static void SendPaymentStoppedMessage(Player player, string reason)
        {
            if (player == null)
            {
                return;
            }

            player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, reason);
            player.ClearAnimations();
        }

        private void SendPaymentRequestedMessages(Player sender, Player receiver, ulong currencyId, decimal amount)
        {
            string formattedAmount = _currencyService.GetCurrencyFormat(currencyId, amount);
            sender.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"Sent a request to {GetCharacterName(receiver)} to start a transaction.");
            receiver.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"{GetCharacterName(sender)} has sent a request to start a transaction to transfer {formattedAmount} to you. Type /acceptpay to proceed.");
        }

        private void SendHandOverMessage(PendingPayment payment, Character sender, Character receiver, BillStack bill)
        {
            string billName = bill.Item.GetName();
            string billsText = bill.Amount > 1 ? $"{bill.Amount} {billName} bills" : $"a {billName} bill";
            string message = sender.HasWallet()
                ? $"reaches into their wallet and retrieves {billsText}, offering the bill to {receiver.GetName()}"
                : $"fumbles through their pockets before producing {billsText}, offering the bill to {receiver.GetName()}";

            if (!payment.IsFirstBill())
            {
                message += " before repeating the process with other bills";
            }
            _chatService.SendPlayerChatMessage(payment.GetSender(), PlayerChatMessageType.ME, $"{message}.");
        }

        private void SendReceiveMessage(Player receiverPlayer, Character receiver, BillStack bill)
        {
            string billName = bill.Item.GetName();
            string message = receiver.HasWallet()
                ? $"carefully examines each {billName} bill before filing them in their wallet's denomination slots"
                : $"palms the {billName} bill{(bill.Amount > 1 ? "s" : "")} smoothly into their sleeve lining";
            _chatService.SendPlayerChatMessage(receiverPlayer, PlayerChatMessageType.ME, $"{message}.");
        }

        private static string GetCharacterName(Player player)
        {
            return player.GetPlayerCurrentlyPlayingAsCharacter().GetName();
        }

        private async Task PublishCashUpdate(Player player, ulong currencyId)
        {
            await _serverEventAggregator.PublishAsync(new OnPlayerCashUpdateEventArgs
            {
                Player = player,
                CurrencyId = currencyId,
            });
        }
    }
}
