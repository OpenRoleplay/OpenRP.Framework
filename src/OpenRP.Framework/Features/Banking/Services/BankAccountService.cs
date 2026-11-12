using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Banking.Entities;
using OpenRP.Framework.Features.Banking.Enums;
using OpenRP.Framework.Features.Banking.Helpers;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Features.Factions.Entities;
using OpenRP.Framework.Features.Factions.Services;
using OpenRP.Framework.Features.Jurisdictions.Components;
using OpenRP.Framework.Features.Jurisdictions.Services;
using OpenRP.Framework.Features.Properties.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Banking.Services
{
    public class BankAccountService : IBankAccountService
    {
        private const int ObanAttempts = 50;

        private readonly IBankingDatabase _database;
        private readonly IBankService _bankService;
        private readonly IFactionService _factionService;
        private readonly IFactionMembershipService _factionMembershipService;
        private readonly IJurisdictionService _jurisdictionService;
        private readonly IPropertyEditorService _propertyEditorService;
        private readonly ILogger<BankAccountService> _logger;

        public BankAccountService(IBankingDatabase database, IBankService bankService, IFactionService factionService, IFactionMembershipService factionMembershipService,
            IJurisdictionService jurisdictionService, IPropertyEditorService propertyEditorService, ILogger<BankAccountService> logger)
        {
            _database = database;
            _bankService = bankService;
            _factionService = factionService;
            _factionMembershipService = factionMembershipService;
            _jurisdictionService = jurisdictionService;
            _propertyEditorService = propertyEditorService;
            _logger = logger;
        }

        public BankAccountModel? GetAccount(ulong accountId)
        {
            return _database.Use(db => db.BankAccounts.AsNoTracking().FirstOrDefault(account => account.Id == accountId));
        }

        public BankAccountModel? GetAccountByOban(string oban)
        {
            return _database.Use(db => db.BankAccounts.AsNoTracking().FirstOrDefault(account => account.Oban == oban));
        }

        public BankAccessLevel GetAccessLevel(Character character, BankAccountModel account)
        {
            if (account.ClosedAt != null)
            {
                return BankAccessLevel.None;
            }

            BankAccessLevel ownerLevel = GetOwnerAccessLevel(character, account);
            BankAccessLevel listedLevel = GetListedAccessLevel(character.GetId(), account.Id);
            return ownerLevel > listedLevel ? ownerLevel : listedLevel;
        }

        public List<BankAccountModel> GetAccountsAt(Character character, ulong bankId, BankAccessLevel minimumLevel)
        {
            ulong characterId = character.GetId();
            List<ulong> factionIds = _factionMembershipService.GetMemberships(character).Select(membership => membership.GetFactionId()).ToList();
            BankModel? bank = _bankService.GetBank(bankId);
            bool runsBank = bank?.RunByFactionId != null && factionIds.Contains(bank.RunByFactionId.Value);

            List<BankAccountModel> candidates = _database.Use(db => db.BankAccounts.AsNoTracking()
                .Where(account => account.BankId == bankId && account.ClosedAt == null)
                .Where(account => (account.OwnerType == BankAccountOwnerType.Character && account.OwnerId == characterId)
                    || (account.OwnerType == BankAccountOwnerType.Faction && factionIds.Contains(account.OwnerId))
                    || (account.OwnerType == BankAccountOwnerType.Bank && runsBank)
                    || db.BankAccountAccess.Any(access => access.BankAccountId == account.Id && access.CharacterId == characterId))
                .OrderBy(account => account.Name)
                .ToList());

            return candidates.Where(account => GetAccessLevel(character, account) >= minimumLevel).ToList();
        }

        public string GetOwnerName(BankAccountModel account)
        {
            return account.OwnerType switch
            {
                BankAccountOwnerType.Character => GetCharacterName(account.OwnerId) ?? "Unknown",
                BankAccountOwnerType.Faction => _factionService.GetFaction(account.OwnerId)?.GetName() ?? "Unknown faction",
                BankAccountOwnerType.Property => _propertyEditorService.GetProperty(account.OwnerId)?.GetName() ?? "Unknown property",
                _ => _bankService.GetBank(account.OwnerId)?.Name ?? "Unknown bank"
            };
        }

        public List<Faction> GetFactionsToBankFor(Character character)
        {
            return _factionMembershipService.GetMemberships(character)
                .Select(membership => _factionService.GetFaction(membership.GetFactionId()))
                .OfType<Faction>()
                .Where(faction => _factionMembershipService.HasFactionPermission(character, faction, FactionPermissions.Bank))
                .ToList();
        }

        public (BankAccountModel? Account, string? Error) OpenAccount(Character? opener, BankModel bank, BankAccountOwnerType ownerType, ulong ownerId, string name)
        {
            string? error = GetAccountNameError(name) ?? (opener == null ? null : GetOpenAccountError(opener, ownerType, ownerId));
            Jurisdiction? jurisdiction = _jurisdictionService.GetJurisdiction(bank.JurisdictionId);
            ulong? currencyId = jurisdiction == null ? null : _jurisdictionService.GetCurrencyId(jurisdiction);
            error ??= currencyId == null ? "This bank's jurisdiction has no currency, so it can't hold accounts." : null;
            if (error != null)
            {
                return (null, error);
            }

            BankAccountModel? account = TryInsertAccount(bank, GetCountryTag(jurisdiction!), currencyId!.Value, ownerType, ownerId, name.Trim());
            return account == null ? (null, "The account couldn't be opened. Try again.") : (account, null);
        }

        public BankAccountModel? GetOrOpenHouseAccount(BankModel bank, ulong currencyId)
        {
            BankAccountModel? houseAccount = _database.Use(db => db.BankAccounts.AsNoTracking()
                .FirstOrDefault(account => account.OwnerType == BankAccountOwnerType.Bank && account.OwnerId == bank.Id && account.CurrencyId == currencyId && account.ClosedAt == null));
            if (houseAccount != null)
            {
                return houseAccount;
            }

            Jurisdiction? jurisdiction = _jurisdictionService.GetJurisdiction(bank.JurisdictionId);
            string currencyCode = _database.Use(db => db.Currencies.AsNoTracking().Where(currency => currency.Id == currencyId).Select(currency => currency.CurrencyCode).FirstOrDefault()) ?? "?";
            return jurisdiction == null ? null : TryInsertAccount(bank, GetCountryTag(jurisdiction), currencyId, BankAccountOwnerType.Bank, bank.Id, $"House account ({currencyCode})");
        }

        public string? CloseAccount(Character character, BankAccountModel account)
        {
            if (account.OwnerType == BankAccountOwnerType.Bank)
                return "A bank's own account can't be closed.";
            if (GetAccessLevel(character, account) < BankAccessLevel.Manage)
                return "Only the owner can close this account.";

            // In one statement, so money that arrives in the meantime can't be lost by closing.
            int closed = _database.Use(db => db.BankAccounts
                .Where(row => row.Id == account.Id && row.ClosedAt == null && row.Balance == 0)
                .ExecuteUpdate(setters => setters.SetProperty(row => row.ClosedAt, DateTime.UtcNow)));
            return closed == 1 ? null : "An account can only be closed with a balance of exactly 0. Withdraw or transfer the rest first.";
        }

        public List<BankAccessEntry> GetAccessList(BankAccountModel account)
        {
            return _database.Use(db => db.BankAccountAccess.AsNoTracking()
                .Where(access => access.BankAccountId == account.Id)
                .Select(access => new BankAccessEntry(access.Id, access.CharacterId, access.Character!.FirstName + " " + access.Character.LastName, access.Level))
                .ToList());
        }

        public string? GrantAccess(Character manager, BankAccountModel account, string characterName, BankAccessLevel level)
        {
            if (GetAccessLevel(manager, account) < BankAccessLevel.Manage)
                return "Only the owner can change who has access.";
            if (level != BankAccessLevel.View && level != BankAccessLevel.Full)
                return "Access is either view only or full.";

            (ulong? characterId, string? error) = FindCharacterByName(characterName);
            if (error != null)
                return error;
            if (account.OwnerType == BankAccountOwnerType.Character && account.OwnerId == characterId)
                return "The owner already has full access.";

            SetListedAccess(account.Id, characterId!.Value, level);
            return null;
        }

        public string? RevokeAccess(Character manager, BankAccountModel account, ulong accessId)
        {
            if (GetAccessLevel(manager, account) < BankAccessLevel.Manage)
                return "Only the owner can change who has access.";

            _database.Use(db => db.BankAccountAccess.Where(access => access.Id == accessId && access.BankAccountId == account.Id).ExecuteDelete());
            return null;
        }

        public List<BankTransactionModel> GetHistory(BankAccountModel account)
        {
            return _database.Use(db => db.BankTransactions.AsNoTracking()
                .Where(transaction => transaction.BankAccountId == account.Id)
                .OrderByDescending(transaction => transaction.Id)
                .Take(BankingRules.HistoryLength)
                .ToList());
        }

        private BankAccessLevel GetOwnerAccessLevel(Character character, BankAccountModel account)
        {
            switch (account.OwnerType)
            {
                case BankAccountOwnerType.Character:
                    return account.OwnerId == character.GetId() ? BankAccessLevel.Manage : BankAccessLevel.None;
                case BankAccountOwnerType.Faction:
                    return GetFactionAccessLevel(character, _factionService.GetFaction(account.OwnerId), canManage: true);
                case BankAccountOwnerType.Bank:
                    ulong? runByFactionId = _bankService.GetBank(account.OwnerId)?.RunByFactionId;
                    return runByFactionId.HasValue ? GetFactionAccessLevel(character, _factionService.GetFaction(runByFactionId.Value), canManage: false) : BankAccessLevel.None;
                default:
                    // Properties have no owners yet, so only their access list counts.
                    return BankAccessLevel.None;
            }
        }

        /// <summary>
        /// The leader manages a faction's account; members with faction.bank use it. A bank's house account is never
        /// managed by players.
        /// </summary>
        private BankAccessLevel GetFactionAccessLevel(Character character, Faction? faction, bool canManage)
        {
            if (faction == null)
                return BankAccessLevel.None;
            if (_factionMembershipService.IsLeader(character, faction))
                return canManage ? BankAccessLevel.Manage : BankAccessLevel.Full;
            return _factionMembershipService.HasFactionPermission(character, faction, FactionPermissions.Bank) ? BankAccessLevel.Full : BankAccessLevel.None;
        }

        private BankAccessLevel GetListedAccessLevel(ulong characterId, ulong accountId)
        {
            return _database.Use(db => db.BankAccountAccess.AsNoTracking()
                .Where(access => access.BankAccountId == accountId && access.CharacterId == characterId)
                .Select(access => access.Level)
                .FirstOrDefault());
        }

        private string? GetOpenAccountError(Character opener, BankAccountOwnerType ownerType, ulong ownerId)
        {
            return ownerType switch
            {
                BankAccountOwnerType.Character => ownerId == opener.GetId() ? null : "You can only open an account for yourself.",
                BankAccountOwnerType.Faction => GetFactionsToBankFor(opener).Any(faction => faction.GetId() == ownerId) ? null : "You can't open accounts for that faction.",
                _ => "Only admins open accounts for properties."
            };
        }

        /// <summary>
        /// The 2-letter tag of the country the jurisdiction lies in, which starts every new OBAN, as on licence plates.
        /// After an annexation new accounts get the new country's tag; existing OBANs keep theirs.
        /// </summary>
        private string GetCountryTag(Jurisdiction jurisdiction)
        {
            return _jurisdictionService.GetCountry(jurisdiction).GetTwoLetterTag();
        }

        private static string? GetAccountNameError(string name)
        {
            string trimmedName = name.Trim();
            if (trimmedName.Length == 0)
                return "The account needs a name, for example Checking.";
            if (trimmedName.Length > BankingRules.AccountNameMaxLength)
                return $"An account name can be at most {BankingRules.AccountNameMaxLength} characters long.";
            return null;
        }

        /// <summary>
        /// Inserts the account with a random OBAN. The unique index catches an OBAN another server just took.
        /// </summary>
        private BankAccountModel? TryInsertAccount(BankModel bank, string jurisdictionTag, ulong currencyId, BankAccountOwnerType ownerType, ulong ownerId, string name)
        {
            for (int attempt = 0; attempt < ObanAttempts; attempt++)
            {
                BankAccountModel account = new BankAccountModel
                {
                    Oban = Helpers.Oban.Create(jurisdictionTag, bank.Code, Helpers.Oban.CreateRandomAccountNumber()),
                    BankId = bank.Id,
                    CurrencyId = currencyId,
                    Balance = 0,
                    OwnerType = ownerType,
                    OwnerId = ownerId,
                    Name = name,
                    // A bank's own account pays no fee.
                    NextFeeAt = ownerType == BankAccountOwnerType.Bank ? null : DateTime.UtcNow + BankingRules.FeeInterval
                };

                try
                {
                    _database.Use(db =>
                    {
                        db.BankAccounts.Add(account);
                        db.SaveChanges();
                    });
                    return account;
                }
                catch (DbUpdateException exception)
                {
                    _logger.LogWarning(exception, "Opening a bank account with OBAN {Oban} failed; trying another number.", account.Oban);
                }
            }
            return null;
        }

        /// <summary>
        /// The one character with exactly this first and last name.
        /// </summary>
        private (ulong? CharacterId, string? Error) FindCharacterByName(string characterName)
        {
            string[] parts = characterName.Trim().Replace('_', ' ').Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
                return (null, "Enter the full name, first and last, for example John Smith.");

            string firstName = parts[0];
            string lastName = parts[^1];
            List<ulong> characterIds = _database.Use(db => db.Characters.AsNoTracking()
                .Where(character => character.FirstName == firstName && character.LastName == lastName)
                .Select(character => character.Id)
                .Take(2)
                .ToList());

            return characterIds.Count switch
            {
                0 => (null, $"There's nobody called {firstName} {lastName}."),
                1 => (characterIds[0], null),
                _ => (null, $"Several people are called {firstName} {lastName}. Ask an admin for help.")
            };
        }

        private void SetListedAccess(ulong accountId, ulong characterId, BankAccessLevel level)
        {
            _database.Use(db =>
            {
                int updated = db.BankAccountAccess
                    .Where(access => access.BankAccountId == accountId && access.CharacterId == characterId)
                    .ExecuteUpdate(setters => setters.SetProperty(access => access.Level, level));
                if (updated == 0)
                {
                    db.BankAccountAccess.Add(new BankAccountAccessModel { BankAccountId = accountId, CharacterId = characterId, Level = level });
                    db.SaveChanges();
                }
            });
        }

        private string? GetCharacterName(ulong characterId)
        {
            return _database.Use(db => db.Characters.AsNoTracking()
                .Where(character => character.Id == characterId)
                .Select(character => character.FirstName + " " + character.LastName)
                .FirstOrDefault());
        }
    }
}
