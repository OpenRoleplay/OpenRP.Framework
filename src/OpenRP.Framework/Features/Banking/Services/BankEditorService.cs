using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Actors.Components;
using OpenRP.Framework.Features.Banking.Entities;
using OpenRP.Framework.Features.Banking.Enums;
using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Features.Jurisdictions.Components;
using OpenRP.Framework.Features.Jurisdictions.Services;
using OpenRP.Framework.Features.Properties.Components;
using System;
using System.Globalization;
using System.Linq;

namespace OpenRP.Framework.Features.Banking.Services
{
    public class BankEditorService : IBankEditorService
    {
        private const string HouseAccountName = "House account";

        private readonly IBankingDatabase _database;
        private readonly IBankService _bankService;
        private readonly IBankAccountService _bankAccountService;
        private readonly IJurisdictionService _jurisdictionService;

        public BankEditorService(IBankingDatabase database, IBankService bankService, IBankAccountService bankAccountService, IJurisdictionService jurisdictionService)
        {
            _database = database;
            _bankService = bankService;
            _bankAccountService = bankAccountService;
            _jurisdictionService = jurisdictionService;
        }

        public (BankModel? Bank, string? Error) CreateBank(string name, string code, Jurisdiction jurisdiction)
        {
            string upperCode = code.Trim().ToUpperInvariant();
            string? error = GetNameError(name) ?? GetCodeError(upperCode)
                ?? (_jurisdictionService.GetCurrencyId(jurisdiction) == null ? $"{jurisdiction.GetName()} has no currency, so a bank there can't hold accounts." : null);
            if (error != null)
            {
                return (null, error);
            }

            BankModel bank = new BankModel { Name = name.Trim(), Code = upperCode, JurisdictionId = jurisdiction.GetId(), MonthlyFee = 0 };
            try
            {
                _database.Use(db =>
                {
                    db.Banks.Add(bank);
                    db.SaveChanges();
                });
            }
            catch (DbUpdateException)
            {
                return (null, $"The code {upperCode} was just taken.");
            }

            (_, string? accountError) = _bankAccountService.OpenAccount(null, bank, BankAccountOwnerType.Bank, bank.Id, HouseAccountName);
            return (bank, accountError);
        }

        public string? RenameBank(BankModel bank, string name)
        {
            string? error = GetNameError(name);
            if (error != null)
            {
                return error;
            }

            string trimmedName = name.Trim();
            _database.Use(db => db.Banks.Where(row => row.Id == bank.Id).ExecuteUpdate(setters => setters.SetProperty(row => row.Name, trimmedName)));
            return null;
        }

        public string? SetMonthlyFee(BankModel bank, string feeText)
        {
            if (!decimal.TryParse(feeText.Trim().TrimStart('$'), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal fee)
                || fee < 0 || fee > BankingRules.MaxMonthlyFee || decimal.Round(fee, 2) != fee)
            {
                return $"The fee is an amount from 0 to {BankingRules.MaxMonthlyFee:0}, with at most 2 decimals.";
            }

            _database.Use(db => db.Banks.Where(row => row.Id == bank.Id).ExecuteUpdate(setters => setters.SetProperty(row => row.MonthlyFee, fee)));
            return null;
        }

        public void SetRunByFaction(BankModel bank, Faction? faction)
        {
            ulong? factionId = faction?.GetId();
            _database.Use(db => db.Banks.Where(row => row.Id == bank.Id).ExecuteUpdate(setters => setters.SetProperty(row => row.RunByFactionId, factionId)));
        }

        public string? AddBranch(BankModel bank, Property property)
        {
            BankModel? currentBank = _bankService.GetBankOfBranch(property.GetId());
            if (currentBank != null)
            {
                return $"{property.GetName()} is already a branch of {currentBank.Name}.";
            }

            _database.Use(db =>
            {
                db.BankBranches.Add(new BankBranchModel { BankId = bank.Id, PropertyId = property.GetId() });
                db.SaveChanges();
            });
            return null;
        }

        public void RemoveBranch(ulong branchId)
        {
            _database.Use(db => db.BankBranches.Where(branch => branch.Id == branchId).ExecuteDelete());
        }

        public string? AddTeller(BankModel bank, ServerActor actor)
        {
            BankModel? currentBank = _bankService.GetTellerBank(actor);
            if (currentBank != null)
            {
                return $"{actor.GetName()} already works for {currentBank.Name}.";
            }

            _database.Use(db =>
            {
                db.BankTellers.Add(new BankTellerModel { BankId = bank.Id, ActorId = actor.GetId() });
                db.SaveChanges();
            });
            return null;
        }

        public void RemoveTeller(ulong tellerId)
        {
            _database.Use(db => db.BankTellers.Where(teller => teller.Id == tellerId).ExecuteDelete());
        }

        public (BankAccountModel? Account, string? Error) OpenPropertyAccount(BankModel bank, Property property, string name)
        {
            return _bankAccountService.OpenAccount(null, bank, BankAccountOwnerType.Property, property.GetId(), name);
        }

        private static string? GetNameError(string name)
        {
            string trimmedName = name.Trim();
            if (trimmedName.Length == 0)
                return "The name can't be empty.";
            if (trimmedName.Length > BankingRules.BankNameMaxLength)
                return $"A name can be at most {BankingRules.BankNameMaxLength} characters long.";
            return null;
        }

        private string? GetCodeError(string code)
        {
            if (code.Length != 4 || !code.All(char.IsAsciiLetterUpper))
                return "The code is 4 letters, for example PALO. It's part of every account number of the bank.";
            if (_database.Use(db => db.Banks.Any(bank => bank.Code == code)))
                return $"Another bank already uses the code {code}.";
            return null;
        }
    }
}
