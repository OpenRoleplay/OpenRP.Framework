using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Features.Jurisdictions.Components;
using OpenRP.Framework.Features.Jurisdictions.Entities;
using System.Linq;

namespace OpenRP.Framework.Features.Banking.Services
{
    /// <summary>
    /// A jurisdiction with a bank in it stays: its tag is part of the bank's OBANs and banks can't move.
    /// </summary>
    public class BankJurisdictionGuard : IJurisdictionDeletionGuard
    {
        private readonly IBankingDatabase _database;

        public BankJurisdictionGuard(IBankingDatabase database)
        {
            _database = database;
        }

        public string? GetDeletionError(Jurisdiction jurisdiction)
        {
            ulong jurisdictionId = jurisdiction.GetId();
            string? bankName = _database.Use(db => db.Banks.AsNoTracking()
                .Where(bank => bank.JurisdictionId == jurisdictionId)
                .Select(bank => bank.Name)
                .FirstOrDefault());
            return bankName == null ? null : $"{bankName} is in this jurisdiction. Banks can't move, so it stays as long as they exist.";
        }
    }
}
