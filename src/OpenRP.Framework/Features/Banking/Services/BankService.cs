using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Actors.Components;
using OpenRP.Framework.Features.Banking.Entities;
using OpenRP.Framework.Features.Banking.Enums;
using OpenRP.Framework.Features.Jurisdictions.Components;
using OpenRP.Framework.Features.Jurisdictions.Services;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenRP.Framework.Features.Banking.Services
{
    public class BankService : IBankService
    {
        private readonly IBankingDatabase _database;
        private readonly IJurisdictionService _jurisdictionService;
        private readonly IJurisdictionSettingService _jurisdictionSettingService;
        private readonly MaxMonthlyFeeSetting _maxMonthlyFeeSetting;

        public BankService(IBankingDatabase database, IJurisdictionService jurisdictionService, IJurisdictionSettingService jurisdictionSettingService,
            MaxMonthlyFeeSetting maxMonthlyFeeSetting)
        {
            _database = database;
            _jurisdictionService = jurisdictionService;
            _jurisdictionSettingService = jurisdictionSettingService;
            _maxMonthlyFeeSetting = maxMonthlyFeeSetting;
        }

        public List<BankModel> GetBanks()
        {
            return _database.Use(db => db.Banks.AsNoTracking().OrderBy(bank => bank.Name).ToList());
        }

        public BankModel? GetBank(ulong bankId)
        {
            return _database.Use(db => db.Banks.AsNoTracking().FirstOrDefault(bank => bank.Id == bankId));
        }

        public decimal GetMaxMonthlyFee(BankModel bank)
        {
            Jurisdiction? jurisdiction = _jurisdictionService.GetJurisdiction(bank.JurisdictionId);
            return jurisdiction == null
                ? BankingRules.DefaultMaxMonthlyFee
                : _jurisdictionSettingService.GetValue(jurisdiction, _maxMonthlyFeeSetting);
        }

        public string FormatFee(BankModel bank, decimal amount)
        {
            Jurisdiction? jurisdiction = _jurisdictionService.GetJurisdiction(bank.JurisdictionId);
            return jurisdiction == null
                ? amount.ToString("0.00", CultureInfo.InvariantCulture)
                : _maxMonthlyFeeSetting.Format(amount, jurisdiction);
        }

        public BankModel? GetTellerBank(ServerActor actor)
        {
            ulong actorId = actor.GetId();
            return _database.Use(db => db.BankTellers.AsNoTracking()
                .Where(teller => teller.ActorId == actorId)
                .Select(teller => teller.Bank)
                .FirstOrDefault());
        }

        public bool IsInsideBranch(ServerActor actor, ulong bankId)
        {
            ulong? propertyId = actor.GetInsidePropertyId();
            return propertyId.HasValue && _database.Use(db => db.BankBranches.Any(branch => branch.BankId == bankId && branch.PropertyId == propertyId.Value));
        }

        public List<BankBranchModel> GetBranches(ulong bankId)
        {
            return _database.Use(db => db.BankBranches.AsNoTracking().Where(branch => branch.BankId == bankId).ToList());
        }

        public BankModel? GetBankOfBranch(ulong propertyId)
        {
            return _database.Use(db => db.BankBranches.AsNoTracking()
                .Where(branch => branch.PropertyId == propertyId)
                .Select(branch => branch.Bank)
                .FirstOrDefault());
        }

        public List<BankTellerModel> GetTellers(ulong bankId)
        {
            return _database.Use(db => db.BankTellers.AsNoTracking().Where(teller => teller.BankId == bankId).ToList());
        }
    }
}
