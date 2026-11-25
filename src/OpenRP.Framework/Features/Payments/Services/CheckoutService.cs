using Microsoft.Extensions.Logging;
using OpenRP.Framework.Features.Banking.Services;
using OpenRP.Framework.Features.Banking.Services.Dialogs;
using OpenRP.Framework.Features.Payments.Components;
using OpenRP.Framework.Features.Payments.Entities;
using OpenRP.Framework.Features.Payments.Services.Dialogs;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using SampSharp.Entities.SAMP;
using System;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Payments.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IPaymentDialogService _paymentDialogService;
        private readonly IBankDialogService _bankDialogService;
        private readonly IBankTransactionService _bankTransactionService;
        private readonly ILogger<CheckoutService> _logger;

        public CheckoutService(IPaymentDialogService paymentDialogService, IBankDialogService bankDialogService, IBankTransactionService bankTransactionService,
            ILogger<CheckoutService> logger)
        {
            _paymentDialogService = paymentDialogService;
            _bankDialogService = bankDialogService;
            _bankTransactionService = bankTransactionService;
            _logger = logger;
        }

        public Task<PaymentResult> RequestPaymentAsync(Player player, PaymentRequest request)
        {
            if (!player.IsComponentAlive || !player.IsPlayerPlayingAsCharacter())
                return Task.FromResult(PaymentResult.Failed("The player isn't playing a character."));
            if (player.GetComponent<PendingCheckout>() != null)
                return Task.FromResult(Fail(player, "Finish your other payment first."));

            PendingCheckout checkout = player.AddComponent<PendingCheckout>(request);
            _paymentDialogService.OpenPaymentMethodChoice(player, request,
                () => _ = PayInCashAsync(player, checkout),
                () => PayWithCard(player, checkout),
                () => checkout.End(PaymentResult.Cancelled));
            return checkout.GetResult();
        }

        private async Task PayInCashAsync(Player player, PendingCheckout checkout)
        {
            PaymentRequest request = checkout.GetRequest();
            try
            {
                string? error = await _bankTransactionService.PayInCashAsync(player, request.ReceiverAccountId, request.CurrencyId, request.Amount, $"Cash: {request.Description}");
                EndWith(player, checkout, error);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "A cash payment for {Description} failed.", request.Description);
                EndWith(player, checkout, "The payment couldn't be made.");
            }
        }

        private void PayWithCard(Player player, PendingCheckout checkout)
        {
            PaymentRequest request = checkout.GetRequest();
            _bankDialogService.OpenCardAuthorisation(player, request.CurrencyId, "Pay",
                cardId => EndWith(player, checkout, _bankTransactionService.PayWithCard(player, cardId, request.ReceiverAccountId, request.Amount, request.Description)),
                () => checkout.End(PaymentResult.Cancelled));
        }

        /// <summary>
        /// Paid when there's no error; otherwise the player is told why and the payment failed.
        /// </summary>
        private static void EndWith(Player player, PendingCheckout checkout, string? error)
        {
            if (!checkout.IsComponentAlive)
                return;
            checkout.End(error == null ? PaymentResult.Paid : Fail(player, error));
        }

        private static PaymentResult Fail(Player player, string error)
        {
            if (player.IsComponentAlive)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, error);
            }
            return PaymentResult.Failed(error);
        }
    }
}
