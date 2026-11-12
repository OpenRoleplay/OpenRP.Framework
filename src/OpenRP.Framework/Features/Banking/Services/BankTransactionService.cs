using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Banking.Entities;
using OpenRP.Framework.Features.Banking.Enums;
using OpenRP.Framework.Features.Banking.Helpers;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Currencies.Enums;
using OpenRP.Framework.Features.Currencies.Services;
using OpenRP.Framework.Features.Players.Extensions;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Banking.Services
{
    public class BankTransactionService : IBankTransactionService
    {
        private const int FeeBatchSize = 500;

        private readonly IBankingDatabase _database;
        private readonly IBankAccountService _bankAccountService;
        private readonly IBankService _bankService;
        private readonly ICurrencyService _currencyService;
        private readonly ILogger<BankTransactionService> _logger;

        public BankTransactionService(IBankingDatabase database, IBankAccountService bankAccountService, IBankService bankService, ICurrencyService currencyService,
            ILogger<BankTransactionService> logger)
        {
            _database = database;
            _bankAccountService = bankAccountService;
            _bankService = bankService;
            _currencyService = currencyService;
            _logger = logger;
        }

        public async Task<string?> DepositAsync(Player player, ulong accountId, decimal amount)
        {
            (Character? character, BankAccountModel? account, string? error) = GetUsableAccount(player, accountId, amount);
            if (error != null)
            {
                return error;
            }

            CurrencyChangeResult takeResult = await _currencyService.TakeCharacterCurrency(character!, account!.CurrencyId, amount);
            if (takeResult != CurrencyChangeResult.Success)
            {
                return GetCashError(takeResult, account.CurrencyId, isTaking: true);
            }

            if (TryCreditDeposit(account, amount, character!.GetId()))
            {
                return null;
            }

            // The account closed in the meantime, or the database failed: the cash goes back to the player.
            await _currencyService.GiveCharacterCurrency(character, account.CurrencyId, amount);
            return "The bank couldn't take the deposit. Your cash was handed back.";
        }

        public async Task<string?> WithdrawAsync(Player player, ulong accountId, decimal amount)
        {
            (Character? character, BankAccountModel? account, string? error) = GetUsableAccount(player, accountId, amount);
            if (error != null)
            {
                return error;
            }

            if (!TryDebit(account!.Id, amount, BankTransactionType.Withdrawal, character!.GetId(), null))
            {
                return "There isn't enough money in the account.";
            }

            CurrencyChangeResult giveResult = await _currencyService.GiveCharacterCurrency(character, account.CurrencyId, amount);
            if (giveResult == CurrencyChangeResult.Success)
            {
                return null;
            }

            // The cash didn't fit: the money goes back into the account, so nothing is lost.
            if (!TryCreditDeposit(account, amount, character.GetId()))
            {
                _logger.LogError("A withdrawal of {Amount} from bank account {Oban} couldn't be handed out or put back. Refund it by hand.", amount, account.Oban);
            }
            return GetCashError(giveResult, account.CurrencyId, isTaking: false) + " The money stays in your account.";
        }

        public string? Transfer(Player player, ulong fromAccountId, string toOban, decimal amount)
        {
            (Character? character, BankAccountModel? fromAccount, string? error) = GetUsableAccount(player, fromAccountId, amount);
            BankAccountModel? toAccount = error == null ? _bankAccountService.GetAccountByOban(toOban) : null;
            error ??= GetTransferTargetError(fromAccount!, toAccount);
            if (error != null)
            {
                return error;
            }

            return TryMove(fromAccount!, toAccount!, amount, character!.GetId())
                ? null
                : "There isn't enough money in the account, or the other account just closed.";
        }

        public void ChargeDueFees()
        {
            DateTime now = DateTime.UtcNow;
            List<BankAccountModel> dueAccounts = _database.Use(db => db.BankAccounts.AsNoTracking()
                .Where(account => account.ClosedAt == null && account.NextFeeAt != null && account.NextFeeAt <= now)
                .OrderBy(account => account.NextFeeAt)
                .Take(FeeBatchSize)
                .ToList());

            foreach (BankAccountModel account in dueAccounts)
            {
                try
                {
                    ChargeFee(account);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Charging the monthly fee of bank account {Oban} failed.", account.Oban);
                }
            }
        }

        /// <summary>
        /// The character, the open account and a valid amount, when the player may use the account fully.
        /// </summary>
        private (Character? Character, BankAccountModel? Account, string? Error) GetUsableAccount(Player player, ulong accountId, decimal amount)
        {
            if (!player.IsComponentAlive || !player.IsPlayerPlayingAsCharacter())
                return (null, null, "You need to be playing a character.");
            if (!BankAmounts.IsValid(amount))
                return (null, null, $"Enter an amount above 0, with at most 2 decimals, up to {BankAmounts.Maximum:N0}.");

            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();
            BankAccountModel? account = _bankAccountService.GetAccount(accountId);
            if (account == null || account.ClosedAt != null)
                return (null, null, "That account is closed.");
            if (_bankAccountService.GetAccessLevel(character, account) < BankAccessLevel.Full)
                return (null, null, "You aren't allowed to use that account.");
            return (character, account, null);
        }

        private static string? GetTransferTargetError(BankAccountModel fromAccount, BankAccountModel? toAccount)
        {
            if (toAccount == null || toAccount.ClosedAt != null)
                return "No open account has that OBAN.";
            if (toAccount.Id == fromAccount.Id)
                return "You can't transfer money to the same account.";
            if (toAccount.CurrencyId != fromAccount.CurrencyId)
                return "That account is in another currency. Transfers only go between accounts in the same currency.";
            return null;
        }

        private string GetCashError(CurrencyChangeResult result, ulong currencyId, bool isTaking)
        {
            string currencyName = _currencyService.GetCurrencies().FirstOrDefault(currency => currency.Id == currencyId)?.Name ?? "this currency";
            return result switch
            {
                CurrencyChangeResult.NoWallet => "You have no wallet to hold the cash.",
                CurrencyChangeResult.NotEnoughFunds => $"You don't have that much cash in {currencyName}.",
                CurrencyChangeResult.NotEnoughSpace => "The cash doesn't fit in your wallet.",
                CurrencyChangeResult.AmountNotPayableInBills => $"That amount can't be paid in {currencyName} bills and coins.",
                _ => isTaking ? "The cash couldn't be taken." : "The cash couldn't be handed over."
            };
        }

        /// <summary>
        /// Credits a deposit, or money put back. False instead of throwing when the database fails, so the caller can
        /// always hand the cash back.
        /// </summary>
        private bool TryCreditDeposit(BankAccountModel account, decimal amount, ulong characterId)
        {
            try
            {
                return TryCredit(account.Id, amount, BankTransactionType.Deposit, characterId, null);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Crediting {Amount} to bank account {Oban} failed.", amount, account.Oban);
                return false;
            }
        }

        /// <summary>
        /// Adds the money to an open account and logs it. False when the account is closed.
        /// </summary>
        private bool TryCredit(ulong accountId, decimal amount, BankTransactionType type, ulong? characterId, string? otherOban)
        {
            return _database.UseInTransaction(db => Credit(db, accountId, amount, type, characterId, otherOban));
        }

        /// <summary>
        /// Takes the money from an open account that holds at least that much, and logs it. False otherwise.
        /// </summary>
        private bool TryDebit(ulong accountId, decimal amount, BankTransactionType type, ulong? characterId, string? otherOban)
        {
            return _database.UseInTransaction(db => Debit(db, accountId, amount, minimumBalanceAfter: 0, type, characterId, otherOban));
        }

        /// <summary>
        /// Both sides in one transaction: either the money moves or nothing happens.
        /// </summary>
        private bool TryMove(BankAccountModel fromAccount, BankAccountModel toAccount, decimal amount, ulong characterId)
        {
            return _database.UseInTransaction(db =>
                Debit(db, fromAccount.Id, amount, minimumBalanceAfter: 0, BankTransactionType.TransferOut, characterId, toAccount.Oban)
                && Credit(db, toAccount.Id, amount, BankTransactionType.TransferIn, characterId, fromAccount.Oban));
        }

        /// <summary>
        /// Claims the fee by moving its due date in one statement, so two servers never charge it twice. The fee only
        /// takes the balance down to the lowest allowed balance.
        /// </summary>
        private void ChargeFee(BankAccountModel account)
        {
            BankModel? bank = _bankService.GetBank(account.BankId);
            // Fees go to the house account of the account's own currency, so currencies are never mixed.
            BankAccountModel? houseAccount = bank == null ? null : _bankAccountService.GetOrOpenHouseAccount(bank, account.CurrencyId);
            _database.UseInTransaction(db =>
            {
                if (!ClaimFee(db, account))
                {
                    return false;
                }

                decimal fee = Math.Min(bank?.MonthlyFee ?? 0, BankingRules.MaxMonthlyFee);
                decimal balance = db.BankAccounts.Where(row => row.Id == account.Id).Select(row => row.Balance).First();
                decimal charge = Math.Min(fee, balance - BankingRules.LowestBalance);
                if (charge > 0 && houseAccount != null
                    && Debit(db, account.Id, charge, BankingRules.LowestBalance, BankTransactionType.Fee, null, houseAccount.Oban))
                {
                    Credit(db, houseAccount.Id, charge, BankTransactionType.FeeIncome, null, account.Oban);
                }
                return true;
            });
        }

        private static bool ClaimFee(BaseDataContext db, BankAccountModel account)
        {
            DateTime? dueAt = account.NextFeeAt;
            return db.BankAccounts
                .Where(row => row.Id == account.Id && row.NextFeeAt == dueAt)
                .ExecuteUpdate(setters => setters.SetProperty(row => row.NextFeeAt, dueAt + BankingRules.FeeInterval)) == 1;
        }

        private static bool Credit(BaseDataContext db, ulong accountId, decimal amount, BankTransactionType type, ulong? characterId, string? otherOban)
        {
            int updated = db.BankAccounts
                .Where(row => row.Id == accountId && row.ClosedAt == null)
                .ExecuteUpdate(setters => setters.SetProperty(row => row.Balance, row => row.Balance + amount));
            if (updated != 1)
            {
                return false;
            }
            AddTransaction(db, accountId, amount, type, characterId, otherOban);
            return true;
        }

        private static bool Debit(BaseDataContext db, ulong accountId, decimal amount, decimal minimumBalanceAfter, BankTransactionType type, ulong? characterId, string? otherOban)
        {
            int updated = db.BankAccounts
                .Where(row => row.Id == accountId && row.ClosedAt == null && row.Balance - amount >= minimumBalanceAfter)
                .ExecuteUpdate(setters => setters.SetProperty(row => row.Balance, row => row.Balance - amount));
            if (updated != 1)
            {
                return false;
            }
            AddTransaction(db, accountId, -amount, type, characterId, otherOban);
            return true;
        }

        private static void AddTransaction(BaseDataContext db, ulong accountId, decimal amount, BankTransactionType type, ulong? characterId, string? otherOban)
        {
            decimal balanceAfter = db.BankAccounts.Where(row => row.Id == accountId).Select(row => row.Balance).First();
            db.BankTransactions.Add(new BankTransactionModel
            {
                BankAccountId = accountId,
                Amount = amount,
                BalanceAfter = balanceAfter,
                Type = type,
                OtherOban = otherOban,
                CharacterId = characterId
            });
            db.SaveChanges();
        }
    }
}
