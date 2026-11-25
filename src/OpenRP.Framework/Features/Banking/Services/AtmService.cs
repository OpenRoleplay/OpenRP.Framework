using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Banking.Components;
using OpenRP.Framework.Features.Banking.Entities;
using OpenRP.Framework.Features.Banking.Enums;
using OpenRP.Framework.Features.Banking.Helpers;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Currencies.Enums;
using OpenRP.Framework.Features.Currencies.Services;
using OpenRP.Framework.Features.Maps.Components;
using OpenRP.Framework.Features.Players.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Banking.Services
{
    public class AtmService : IAtmService
    {
        private const int DepositBatchSize = 500;
        // Marks money put back after cash didn't fit, so it doesn't count against the withdrawal limit.
        private const string WithdrawalReturnedDescription = "ATM withdrawal returned";
        private const string AtmDepositDescription = "ATM deposit";
        private const string AtmWithdrawalDescription = "ATM withdrawal";

        private readonly IBankingDatabase _database;
        private readonly IBankCardService _bankCardService;
        private readonly ICurrencyService _currencyService;
        private readonly IEntityManager _entityManager;
        private readonly ILogger<AtmService> _logger;

        public AtmService(IBankingDatabase database, IBankCardService bankCardService, ICurrencyService currencyService, IEntityManager entityManager,
            ILogger<AtmService> logger)
        {
            _database = database;
            _bankCardService = bankCardService;
            _currencyService = currencyService;
            _entityManager = entityManager;
            _logger = logger;
        }

        public void AddAtm(EntityId objectEntity, EntityId mapObjectEntity)
        {
            _entityManager.AddComponent<Atm>(objectEntity, new object[] { mapObjectEntity });
        }

        public Atm? GetNearbyAtm(Player player)
        {
            return _entityManager.GetComponents<Atm>()
                .Where(atm => IsPlayerAtAtm(player, atm))
                .OrderBy(atm => Vector3.Distance(atm.GetMapObject()!.GetPosition(), player.Position))
                .FirstOrDefault();
        }

        public bool IsPlayerAtAtm(Player player, Atm atm)
        {
            MapObject? mapObject = atm.IsComponentAlive ? atm.GetMapObject() : null;
            return mapObject != null
                && player.IsComponentAlive
                && IsInSameSpace(mapObject.GetInterior(), player.Interior)
                && IsInSameSpace(mapObject.GetVirtualWorld(), player.VirtualWorld)
                && Vector3.Distance(mapObject.GetPosition(), player.Position) <= BankingRules.AtmRange;
        }

        public decimal GetRemainingWithdrawal(ulong cardId)
        {
            decimal withdrawn = _database.Use(db => GetWithdrawnInPeriod(db, cardId));
            return Math.Max(0, BankingRules.AtmWithdrawalLimit - withdrawn);
        }

        public async Task<string?> WithdrawAsync(Player player, ulong cardId, decimal amount)
        {
            (BankCardDetails? card, string? error) = GetUsableCard(player, cardId, amount);
            if (error != null)
                return error;

            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();
            error = TryDebitWithinLimit(card!, amount, character.GetId());
            if (error != null)
                return error;

            CurrencyChangeResult giveResult = await _currencyService.GiveCharacterCurrency(character, card!.Account.CurrencyId, amount);
            if (giveResult == CurrencyChangeResult.Success)
                return null;

            PutWithdrawalBack(card, amount, character.GetId());
            return GetCashError(giveResult, card.Account.CurrencyId, isTaking: false) + " The money stays in your account.";
        }

        public async Task<string?> DepositAsync(Player player, ulong cardId, Atm atm, decimal amount)
        {
            (BankCardDetails? card, string? error) = GetUsableCard(player, cardId, amount);
            if (error != null)
                return error;

            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();
            CurrencyChangeResult takeResult = await _currencyService.TakeCharacterCurrency(character, card!.Account.CurrencyId, amount);
            if (takeResult != CurrencyChangeResult.Success)
                return GetCashError(takeResult, card.Account.CurrencyId, isTaking: true);

            if (TryAddPendingDeposit(card, amount, character.GetId(), atm))
                return null;

            // The deposit couldn't be stored: the cash goes back to the player.
            await _currencyService.GiveCharacterCurrency(character, card.Account.CurrencyId, amount);
            return "The ATM couldn't take the deposit. Your cash was handed back.";
        }

        public List<BankPendingDepositModel> GetPendingDeposits(ulong accountId)
        {
            return _database.Use(db => db.BankPendingDeposits.AsNoTracking()
                .Where(deposit => deposit.BankAccountId == accountId && deposit.CreditedAt == null)
                .OrderBy(deposit => deposit.CreditAt)
                .ToList());
        }

        public void CreditDueDeposits()
        {
            DateTime now = DateTime.UtcNow;
            List<BankPendingDepositModel> dueDeposits = _database.Use(db => db.BankPendingDeposits.AsNoTracking()
                .Where(deposit => deposit.CreditedAt == null && deposit.CreditAt <= now)
                .OrderBy(deposit => deposit.CreditAt)
                .Take(DepositBatchSize)
                .ToList());

            foreach (BankPendingDepositModel deposit in dueDeposits)
            {
                try
                {
                    CreditDeposit(deposit);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Crediting ATM deposit {DepositId} failed.", deposit.Id);
                }
            }
        }

        /// <summary>
        /// -1 in a map object means every interior or virtual world.
        /// </summary>
        private static bool IsInSameSpace(int objectSpace, int playerSpace)
        {
            return objectSpace == -1 || objectSpace == playerSpace;
        }

        private (BankCardDetails? Card, string? Error) GetUsableCard(Player player, ulong cardId, decimal amount)
        {
            if (!BankAmounts.IsValid(amount))
                return (null, $"Enter an amount above 0, with at most 2 decimals, up to {BankAmounts.Maximum:N0}.");

            BankCardDetails? card = _bankCardService.GetCardDetails(cardId);
            if (card == null)
                return (null, "The ATM doesn't recognise this card.");
            string? error = _bankCardService.GetUseError(player, card);
            return error == null ? (card, null) : (null, error);
        }

        /// <summary>
        /// The limit check and the debit in one transaction, after locking the card row, so two withdrawals at once
        /// (even on two servers) can't both pass the limit.
        /// </summary>
        private string? TryDebitWithinLimit(BankCardDetails card, decimal amount, ulong characterId)
        {
            string? error = null;
            _database.UseInTransaction(db =>
            {
                error = null;
                LockCard(db, card.Card.Id);
                if (GetWithdrawnInPeriod(db, card.Card.Id) + amount > BankingRules.AtmWithdrawalLimit)
                {
                    error = $"That's over this card's ATM limit of {FormatMoney(card.Account.CurrencyId, BankingRules.AtmWithdrawalLimit)} in {BankingRules.AtmWithdrawalLimitPeriod.TotalHours:0} hours.";
                    return false;
                }
                BankLedgerEntry entry = new BankLedgerEntry(BankTransactionType.Withdrawal, characterId, BankCardId: card.Card.Id, Description: AtmWithdrawalDescription);
                if (!BankLedger.Debit(db, card.Account.Id, amount, minimumBalanceAfter: 0, entry))
                {
                    error = "There isn't enough money in the account.";
                    return false;
                }
                return true;
            });
            return error;
        }

        /// <summary>
        /// A no-op update still takes the row lock, so a second withdrawal with this card waits until the first commits.
        /// It must come before the limit's read: MySQL takes a transaction's snapshot at its first plain read.
        /// </summary>
        private static void LockCard(BaseDataContext db, ulong cardId)
        {
            db.BankCards.Where(row => row.Id == cardId)
                .ExecuteUpdate(setters => setters.SetProperty(row => row.WrongPinAttempts, row => row.WrongPinAttempts));
        }

        /// <summary>
        /// Withdrawals minus money put back after cash didn't fit, in the limit period.
        /// </summary>
        private static decimal GetWithdrawnInPeriod(BaseDataContext db, ulong cardId)
        {
            DateTime since = DateTime.UtcNow - BankingRules.AtmWithdrawalLimitPeriod;
            decimal net = db.BankTransactions
                .Where(transaction => transaction.BankCardId == cardId && transaction.CreatedOn >= since
                    && (transaction.Type == BankTransactionType.Withdrawal
                        || (transaction.Type == BankTransactionType.Deposit && transaction.Description == WithdrawalReturnedDescription)))
                .Sum(transaction => (decimal?)transaction.Amount) ?? 0;
            return -net;
        }

        private void PutWithdrawalBack(BankCardDetails card, decimal amount, ulong characterId)
        {
            try
            {
                BankLedgerEntry entry = new BankLedgerEntry(BankTransactionType.Deposit, characterId, BankCardId: card.Card.Id, Description: WithdrawalReturnedDescription);
                if (_database.UseInTransaction(db => BankLedger.Credit(db, card.Account.Id, amount, entry)))
                    return;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Putting back an ATM withdrawal of {Amount} on bank account {Oban} failed.", amount, card.Account.Oban);
            }
            _logger.LogError("An ATM withdrawal of {Amount} from bank account {Oban} couldn't be handed out or put back. Refund it by hand.", amount, card.Account.Oban);
        }

        private bool TryAddPendingDeposit(BankCardDetails card, decimal amount, ulong characterId, Atm atm)
        {
            MapObject? mapObject = atm.IsComponentAlive ? atm.GetMapObject() : null;
            Vector3 position = mapObject?.GetPosition() ?? Vector3.Zero;
            try
            {
                _database.Use(db =>
                {
                    db.BankPendingDeposits.Add(new BankPendingDepositModel
                    {
                        BankAccountId = card.Account.Id,
                        BankCardId = card.Card.Id,
                        CharacterId = characterId,
                        Amount = amount,
                        CreditAt = DateTime.UtcNow + BankingRules.AtmDepositDelay,
                        AtmX = position.X,
                        AtmY = position.Y,
                        AtmZ = position.Z,
                        AtmInterior = mapObject?.GetInterior() ?? 0,
                        AtmVirtualWorld = mapObject?.GetVirtualWorld() ?? 0
                    });
                    db.SaveChanges();
                });
                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Storing an ATM deposit of {Amount} for bank account {Oban} failed.", amount, card.Account.Oban);
                return false;
            }
        }

        /// <summary>
        /// Claims the deposit by marking it credited in the same transaction as the credit, so two servers never credit
        /// it twice. When the account can't take it, nothing changes and it's tried again later.
        /// </summary>
        private void CreditDeposit(BankPendingDepositModel deposit)
        {
            DateTime now = DateTime.UtcNow;
            BankLedgerEntry entry = new BankLedgerEntry(BankTransactionType.Deposit, deposit.CharacterId, BankCardId: deposit.BankCardId, Description: AtmDepositDescription);
            bool credited = _database.UseInTransaction(db =>
                db.BankPendingDeposits
                    .Where(row => row.Id == deposit.Id && row.CreditedAt == null)
                    .ExecuteUpdate(setters => setters.SetProperty(row => row.CreditedAt, now)) == 1
                && BankLedger.Credit(db, deposit.BankAccountId, deposit.Amount, entry));
            if (!credited)
            {
                _logger.LogWarning("ATM deposit {DepositId} couldn't be credited to bank account {AccountId}; it stays pending.", deposit.Id, deposit.BankAccountId);
            }
        }

        private string GetCashError(CurrencyChangeResult result, ulong currencyId, bool isTaking)
        {
            string currencyName = _currencyService.GetCurrencies().FirstOrDefault(currency => currency.Id == currencyId)?.Name ?? "this currency";
            return BankCashErrors.Get(result, currencyName, isTaking);
        }

        private string FormatMoney(ulong currencyId, decimal amount)
        {
            return _currencyService.GetCurrencyFormat(currencyId, amount);
        }
    }
}
