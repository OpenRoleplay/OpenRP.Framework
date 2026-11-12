using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Actors.Components;
using OpenRP.Framework.Features.Banking.Enums;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Banking.Services
{
    public class BankService : IBankService
    {
        private readonly IBankingDatabase _database;

        public BankService(IBankingDatabase database)
        {
            _database = database;
        }

        public List<BankModel> GetBanks()
        {
            return _database.Use(db => db.Banks.AsNoTracking().OrderBy(bank => bank.Name).ToList());
        }

        public BankModel? GetBank(ulong bankId)
        {
            return _database.Use(db => db.Banks.AsNoTracking().FirstOrDefault(bank => bank.Id == bankId));
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
