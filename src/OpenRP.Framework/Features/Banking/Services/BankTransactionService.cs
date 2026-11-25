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
    /// <summary>
    /// Teller deposits, withdrawals, transfers and fees here; payments in BankTransactionService.Payments.cs.
    /// </summary>
    public partial class BankTransactionService : IBankTransactionService
    {
        private const int FeeBatchSize = 500;

        private readonly IBankingDatabase _database;
        private readonly IBankAccountService _bankAccountService;
        private readonly IBankService _bankService;
        private readonly ICurrencyService _currencyService;
        private readonly IBankCardService _bankCardService;
        private readonly ILogger<BankTransactionService> _logger;

        public BankTransactionService(IBankingDatabase database, IBankAccountService bankAccountService, IBankService bankService, ICurrencyService currencyService,
            IBankCardService bankCardService, ILogger<BankTransactionService> logger)
        {
            _database = database;
            _bankAccountService = bankAccountService;
            _bankService = bankService;
            _currencyService = currencyService;
            _bankCardService = bankCardService;
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
            return BankCashErrors.Get(result, currencyName, isTaking);
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
            return _database.UseInTransaction(db => BankLedger.Credit(db, accountId, amount, new BankLedgerEntry(type, characterId, otherOban)));
        }

        /// <summary>
        /// Takes the money from an open account that holds at least that much, and logs it. False otherwise.
        /// </summary>
        private bool TryDebit(ulong accountId, decimal amount, BankTransactionType type, ulong? characterId, string? otherOban)
        {
            return _database.UseInTransaction(db => BankLedger.Debit(db, accountId, amount, minimumBalanceAfter: 0, new BankLedgerEntry(type, characterId, otherOban)));
        }

        /// <summary>
        /// Both sides in one transaction: either the money moves or nothing happens.
        /// </summary>
        private bool TryMove(BankAccountModel fromAccount, BankAccountModel toAccount, decimal amount, ulong characterId)
        {
            return _database.UseInTransaction(db =>
                BankLedger.Debit(db, fromAccount.Id, amount, minimumBalanceAfter: 0, new BankLedgerEntry(BankTransactionType.TransferOut, characterId, toAccount.Oban))
                && BankLedger.Credit(db, toAccount.Id, amount, new BankLedgerEntry(BankTransactionType.TransferIn, characterId, fromAccount.Oban)));
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
            decimal fee = GetFeeToCharge(bank);
            _database.UseInTransaction(db =>
            {
                if (!ClaimFee(db, account))
                {
                    return false;
                }

                decimal balance = db.BankAccounts.Where(row => row.Id == account.Id).Select(row => row.Balance).First();
                decimal charge = Math.Min(fee, balance - BankingRules.LowestBalance);
                if (charge > 0 && houseAccount != null
                    && BankLedger.Debit(db, account.Id, charge, BankingRules.LowestBalance, new BankLedgerEntry(BankTransactionType.Fee, null, houseAccount.Oban)))
                {
                    BankLedger.Credit(db, houseAccount.Id, charge, new BankLedgerEntry(BankTransactionType.FeeIncome, null, account.Oban));
                }
                return true;
            });
        }

        /// <summary>
        /// The bank's fee, held to the cap its jurisdiction has now. The cap may have been lowered since the bank set its fee.
        /// </summary>
        private decimal GetFeeToCharge(BankModel? bank)
        {
            return bank == null ? 0 : Math.Min(bank.MonthlyFee, _bankService.GetMaxMonthlyFee(bank));
        }

        private static bool ClaimFee(BaseDataContext db, BankAccountModel account)
        {
            DateTime? dueAt = account.NextFeeAt;
            return db.BankAccounts
                .Where(row => row.Id == account.Id && row.NextFeeAt == dueAt)
                .ExecuteUpdate(setters => setters.SetProperty(row => row.NextFeeAt, dueAt + BankingRules.FeeInterval)) == 1;
        }
    }
}
