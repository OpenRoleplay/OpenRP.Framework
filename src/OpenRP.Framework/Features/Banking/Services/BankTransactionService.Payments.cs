using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Banking.Entities;
using OpenRP.Framework.Features.Banking.Enums;
using OpenRP.Framework.Features.Banking.Helpers;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Currencies.Enums;
using OpenRP.Framework.Features.Players.Extensions;
using SampSharp.Entities.SAMP;
using System;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Banking.Services
{
    public partial class BankTransactionService
    {
        public string? PayWithCard(Player player, ulong cardId, ulong receiverAccountId, decimal amount, string description)
        {
            BankCardDetails? card = _bankCardService.GetCardDetails(cardId);
            BankAccountModel? receiver = _bankAccountService.GetAccount(receiverAccountId);
            string? error = GetCardPaymentError(player, card, receiver, amount);
            if (error != null)
                return error;

            ulong characterId = player.GetPlayerCurrentlyPlayingAsCharacter().GetId();
            string shortDescription = Shorten(description);
            bool paid = _database.UseInTransaction(db =>
                BankLedger.Debit(db, card!.Account.Id, amount, minimumBalanceAfter: 0,
                    new BankLedgerEntry(BankTransactionType.CardPayment, characterId, receiver!.Oban, card.Card.Id, shortDescription))
                && BankLedger.Credit(db, receiver.Id, amount,
                    new BankLedgerEntry(BankTransactionType.PaymentReceived, characterId, card.Account.Oban, card.Card.Id, shortDescription)));
            return paid ? null : "Card declined: there isn't enough money in the account.";
        }

        public async Task<string?> PayInCashAsync(Player player, ulong receiverAccountId, ulong currencyId, decimal amount, string description)
        {
            BankAccountModel? receiver = _bankAccountService.GetAccount(receiverAccountId);
            string? error = GetCashPaymentError(player, receiver, currencyId, amount);
            if (error != null)
                return error;

            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();
            CurrencyChangeResult takeResult = await _currencyService.TakeCharacterCurrency(character, currencyId, amount);
            if (takeResult != CurrencyChangeResult.Success)
                return GetCashError(takeResult, currencyId, isTaking: true);

            if (TryCreditCashPayment(receiver!, amount, character.GetId(), Shorten(description)))
                return null;

            await _currencyService.GiveCharacterCurrency(character, currencyId, amount);
            return "The payment couldn't be taken. Your cash was handed back.";
        }

        private string? GetCardPaymentError(Player player, BankCardDetails? card, BankAccountModel? receiver, decimal amount)
        {
            if (card == null)
                return "This card isn't recognised.";
            string? error = _bankCardService.GetUseError(player, card) ?? GetPaymentAmountError(amount) ?? GetReceiverError(receiver, card.Account.CurrencyId);
            if (error != null)
                return error;
            return receiver!.Id == card.Account.Id ? "You can't pay into the card's own account." : null;
        }

        private string? GetCashPaymentError(Player player, BankAccountModel? receiver, ulong currencyId, decimal amount)
        {
            if (!player.IsComponentAlive || !player.IsPlayerPlayingAsCharacter())
                return "You need to be playing a character.";
            return GetPaymentAmountError(amount) ?? GetReceiverError(receiver, currencyId);
        }

        private static string? GetPaymentAmountError(decimal amount)
        {
            return BankAmounts.IsValid(amount) ? null : "That isn't a valid amount to pay.";
        }

        private static string? GetReceiverError(BankAccountModel? receiver, ulong currencyId)
        {
            if (receiver == null || receiver.ClosedAt != null)
                return "The account to pay into is closed.";
            return receiver.CurrencyId == currencyId ? null : "This payment is in another currency.";
        }

        /// <summary>
        /// False instead of throwing when the database fails, so the caller can always hand the cash back.
        /// </summary>
        private bool TryCreditCashPayment(BankAccountModel receiver, decimal amount, ulong characterId, string description)
        {
            try
            {
                return _database.UseInTransaction(db => BankLedger.Credit(db, receiver.Id, amount,
                    new BankLedgerEntry(BankTransactionType.PaymentReceived, characterId, Description: description)));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Paying {Amount} in cash into bank account {Oban} failed.", amount, receiver.Oban);
                return false;
            }
        }

        private static string Shorten(string description)
        {
            string trimmed = description.Trim();
            return trimmed.Length <= BankingRules.TransactionDescriptionMaxLength ? trimmed : trimmed[..BankingRules.TransactionDescriptionMaxLength];
        }
    }
}
