using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Banking.Entities;
using OpenRP.Framework.Features.Banking.Enums;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Inventories.Components;
using OpenRP.Framework.Features.Inventories.Helpers;
using OpenRP.Framework.Features.Inventories.Services;
using OpenRP.Framework.Features.Items.Components;
using OpenRP.Framework.Features.Items.Entities;
using OpenRP.Framework.Features.Players.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Banking.Services
{
    public class BankCardService : IBankCardService
    {
        private readonly IBankingDatabase _database;
        private readonly IBankAccountService _bankAccountService;
        private readonly IEntityManager _entityManager;
        private readonly IInventoryItemManager _inventoryItemManager;
        private readonly ILogger<BankCardService> _logger;

        public BankCardService(IBankingDatabase database, IBankAccountService bankAccountService, IEntityManager entityManager, IInventoryItemManager inventoryItemManager,
            ILogger<BankCardService> logger)
        {
            _database = database;
            _bankAccountService = bankAccountService;
            _entityManager = entityManager;
            _inventoryItemManager = inventoryItemManager;
            _logger = logger;
        }

        public BankCardDetails? GetCardDetails(ulong cardId)
        {
            BankCardModel? card = _database.Use(db => db.BankCards.AsNoTracking()
                .Include(row => row.BankAccount).ThenInclude(account => account!.Bank)
                .FirstOrDefault(row => row.Id == cardId));
            return ToDetails(card);
        }

        public BankCardDetails? GetCurrentCard(ulong accountId, ulong characterId)
        {
            BankCardModel? card = _database.Use(db => db.BankCards.AsNoTracking()
                .Include(row => row.BankAccount).ThenInclude(account => account!.Bank)
                .Where(row => row.BankAccountId == accountId && row.CharacterId == characterId && row.CancelledAt == null)
                .OrderByDescending(row => row.Id)
                .FirstOrDefault());
            return ToDetails(card);
        }

        public List<InventoryItem> GetCarriedCardItems(Character character)
        {
            Inventory? inventory = character.GetInventory();
            return inventory == null ? new List<InventoryItem>() : GetCardItemsIn(inventory);
        }

        public string? GetUseError(Player player, BankCardDetails card)
        {
            if (!player.IsComponentAlive || !player.IsPlayerPlayingAsCharacter())
                return "You need to be playing a character.";

            BankCardStatus status = card.GetStatus();
            if (status != BankCardStatus.Active)
                return GetStatusError(status);

            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();
            bool isOwner = character.GetId() == card.Card.CharacterId;
            if (isOwner && _bankAccountService.GetAccessLevel(character, card.Account) < BankAccessLevel.Full)
                return "You aren't allowed to use this card's account anymore.";
            return null;
        }

        public async Task<string?> IssueCardAsync(Player player, ulong accountId, string? name, string pin)
        {
            string? error = GetIssueError(player, accountId, name, pin);
            if (error != null)
                return error;

            string pinHash = await HashPinAsync(pin);
            // The player may have left, or filled their inventory, while the PIN was hashed.
            error = GetIssueError(player, accountId, name, pin);
            if (error != null)
                return error;

            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();
            BankCardModel? card = TryInsertReplacingCard(accountId, character.GetId(), GetCleanName(name), pinHash);
            if (card == null)
                return "The card couldn't be made. Try again.";

            return GiveCardItem(character, card) ? null : "The card doesn't fit in your inventory.";
        }

        public async Task<PinCheckResult> CheckPinAsync(ulong cardId, string pin)
        {
            BankCardModel? card = _database.Use(db => db.BankCards.AsNoTracking().FirstOrDefault(row => row.Id == cardId));
            if (card == null || card.BlockedAt != null || card.CancelledAt != null)
                return PinCheckResult.Blocked;

            bool isCorrect = GetPinError(pin) == null && await VerifyPinAsync(pin, card.PinHash);
            if (isCorrect)
            {
                ResetWrongPinAttempts(cardId);
                return PinCheckResult.Correct;
            }
            return CountWrongPin(cardId) ? PinCheckResult.Blocked : PinCheckResult.Wrong;
        }

        public async Task<string?> ChangePinAsync(ulong cardId, string pin)
        {
            string? error = GetPinError(pin);
            if (error != null)
                return error;

            string pinHash = await HashPinAsync(pin);
            int updated = _database.Use(db => db.BankCards
                .Where(row => row.Id == cardId && row.CancelledAt == null)
                .ExecuteUpdate(setters => setters.SetProperty(row => row.PinHash, pinHash).SetProperty(row => row.WrongPinAttempts, 0)));
            return updated == 1 ? null : "That card was replaced and can't be changed anymore.";
        }

        public string? BlockCard(ulong cardId)
        {
            DateTime now = DateTime.UtcNow;
            int updated = _database.Use(db => db.BankCards
                .Where(row => row.Id == cardId && row.CancelledAt == null)
                .ExecuteUpdate(setters => setters.SetProperty(row => row.BlockedAt, row => row.BlockedAt ?? now)));
            return updated == 1 ? null : "That card was replaced and doesn't work anymore anyway.";
        }

        public string? UnblockCard(ulong cardId)
        {
            int updated = _database.Use(db => db.BankCards
                .Where(row => row.Id == cardId && row.CancelledAt == null)
                .ExecuteUpdate(setters => setters.SetProperty(row => row.BlockedAt, (DateTime?)null).SetProperty(row => row.WrongPinAttempts, 0)));
            return updated == 1 ? null : "That card was replaced and can't be unblocked. Order a new one.";
        }

        public string? RenameCard(ulong cardId, string name)
        {
            string? error = GetCardNameError(name);
            if (error != null)
                return error;

            string? cleanName = GetCleanName(name);
            int updated = _database.Use(db => db.BankCards
                .Where(row => row.Id == cardId)
                .ExecuteUpdate(setters => setters.SetProperty(row => row.Name, cleanName)));
            return updated == 1 ? null : "That card doesn't exist anymore.";
        }

        public string? GetPinError(string pin)
        {
            return pin.Length == BankingRules.PinLength && pin.All(char.IsAsciiDigit) ? null : $"A PIN is exactly {BankingRules.PinLength} digits.";
        }

        public string? GetCardNameError(string name)
        {
            string trimmed = name.Trim();
            if (trimmed.Length > BankingRules.CardNameMaxLength)
                return $"A card name is at most {BankingRules.CardNameMaxLength} characters.";
            // Braces would be read as colour codes and tabs or line breaks would break dialog columns.
            if (trimmed.Any(character => character == '{' || character == '}' || char.IsControl(character)))
                return "A card name can't contain { }, tabs or line breaks.";
            return null;
        }

        private static BankCardDetails? ToDetails(BankCardModel? card)
        {
            BankAccountModel? account = card?.BankAccount;
            BankModel? bank = account?.Bank;
            return card == null || account == null || bank == null ? null : new BankCardDetails(card, account, bank);
        }

        private static string GetStatusError(BankCardStatus status)
        {
            return status switch
            {
                BankCardStatus.Cancelled => "This card was replaced and doesn't work anymore.",
                BankCardStatus.Blocked => "This card is blocked. The bank can unblock it.",
                _ => "This card's account is closed."
            };
        }

        private List<InventoryItem> GetCardItemsIn(Inventory inventory)
        {
            List<InventoryItem> cardItems = inventory.GetInventoryItems().Where(inventoryItem => inventoryItem.GetBankCardId() != null).ToList();
            foreach (InventoryItem wallet in inventory.GetWallets())
            {
                Inventory? walletInventory = wallet.GetItemInventory();
                if (walletInventory != null)
                {
                    cardItems.AddRange(GetCardItemsIn(walletInventory));
                }
            }
            return cardItems;
        }

        private string? GetIssueError(Player player, ulong accountId, string? name, string pin)
        {
            if (!player.IsComponentAlive || !player.IsPlayerPlayingAsCharacter())
                return "You need to be playing a character.";
            string? inputError = GetPinError(pin) ?? (name == null ? null : GetCardNameError(name));
            if (inputError != null)
                return inputError;

            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();
            BankAccountModel? account = _bankAccountService.GetAccount(accountId);
            if (account == null || _bankAccountService.GetAccessLevel(character, account) < BankAccessLevel.Full)
                return "You aren't allowed to use that account.";

            Inventory? inventory = character.GetInventory();
            Item? cardItem = GetCardItem();
            if (inventory == null || cardItem == null)
                return "The bank has no cards to hand out right now.";
            return inventory.DoesItemFit(cardItem, 1) ? null : "The card doesn't fit in your inventory.";
        }

        /// <summary>
        /// Cancels the character's earlier card for the account and adds the new one, in one transaction, so there's
        /// only ever one working card per account per character.
        /// </summary>
        private BankCardModel? TryInsertReplacingCard(ulong accountId, ulong characterId, string? name, string pinHash)
        {
            BankCardModel card = new BankCardModel
            {
                BankAccountId = accountId,
                CharacterId = characterId,
                Digits = Random.Shared.Next(0, 10000).ToString("D4"),
                Name = name,
                PinHash = pinHash
            };
            try
            {
                DateTime now = DateTime.UtcNow;
                bool inserted = _database.UseInTransaction(db =>
                {
                    db.BankCards
                        .Where(row => row.BankAccountId == accountId && row.CharacterId == characterId && row.CancelledAt == null)
                        .ExecuteUpdate(setters => setters.SetProperty(row => row.CancelledAt, now));
                    db.BankCards.Add(card);
                    db.SaveChanges();
                    return true;
                });
                return inserted ? card : null;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Making a card for bank account {AccountId} failed.", accountId);
                return null;
            }
        }

        /// <summary>
        /// Saves the item at once, so a crash right after can't leave a card without its item.
        /// </summary>
        private bool GiveCardItem(Character character, BankCardModel card)
        {
            Inventory? inventory = character.GetInventory();
            Item? cardItem = GetCardItem();
            ItemAdditionalData additionalData = ItemAdditionalData.Parse(string.Empty);
            additionalData.SetString(InventoryItem.BankCardIdKey, card.Id.ToString());
            if (inventory == null || cardItem == null || !inventory.AddItem(cardItem, 1, additionalData))
            {
                BlockCard(card.Id);
                return false;
            }
            _inventoryItemManager.SaveNow();
            return true;
        }

        private Item? GetCardItem()
        {
            return _entityManager.GetComponent<Item>(ItemEntities.GetItemId(BankingRules.BankCardItemId));
        }

        private static string? GetCleanName(string? name)
        {
            string? trimmed = name?.Trim();
            return string.IsNullOrEmpty(trimmed) ? null : trimmed;
        }

        private void ResetWrongPinAttempts(ulong cardId)
        {
            _database.Use(db => db.BankCards
                .Where(row => row.Id == cardId && row.WrongPinAttempts > 0)
                .ExecuteUpdate(setters => setters.SetProperty(row => row.WrongPinAttempts, 0)));
        }

        /// <summary>
        /// Adds a wrong PIN in one statement and blocks the card when that reaches the limit. True when it's blocked.
        /// </summary>
        private bool CountWrongPin(ulong cardId)
        {
            DateTime now = DateTime.UtcNow;
            return _database.Use(db =>
            {
                db.BankCards
                    .Where(row => row.Id == cardId)
                    .ExecuteUpdate(setters => setters.SetProperty(row => row.WrongPinAttempts, row => row.WrongPinAttempts + 1));
                db.BankCards
                    .Where(row => row.Id == cardId && row.BlockedAt == null && row.WrongPinAttempts >= BankingRules.MaxWrongPinAttempts)
                    .ExecuteUpdate(setters => setters.SetProperty(row => row.BlockedAt, now));
                return db.BankCards.Where(row => row.Id == cardId).Select(row => row.BlockedAt).First() != null;
            });
        }

        /// <summary>
        /// BCrypt takes a noticeable moment, so it runs on a worker thread; the code after the await is back on the main
        /// thread.
        /// </summary>
        private static Task<string> HashPinAsync(string pin)
        {
            return Task.Run(() => BCrypt.Net.BCrypt.HashPassword(pin, BankingRules.PinHashWorkFactor));
        }

        private static Task<bool> VerifyPinAsync(string pin, string pinHash)
        {
            return Task.Run(() => BCrypt.Net.BCrypt.Verify(pin, pinHash));
        }
    }
}
