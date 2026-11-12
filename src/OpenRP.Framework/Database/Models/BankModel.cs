using OpenRP.Framework.Database.Entities;

namespace OpenRP.Framework.Database.Models
{
    /// <summary>
    /// A bank: its 4-letter code and its jurisdiction make up the OBANs of its accounts. Its house account (owner type
    /// Bank) receives the monthly fees.
    /// </summary>
    public class BankModel : BaseModel
    {
        public string Name { get; set; }
        // Four capital letters, part of every OBAN of the bank.
        public string Code { get; set; }
        public ulong JurisdictionId { get; set; }
        // The faction running the bank; its members with faction.bank use the house account.
        public ulong? RunByFactionId { get; set; }
        // Per 30 days, at most BankingLimits.MaxMonthlyFee.
        public decimal MonthlyFee { get; set; }

        // Navigational Properties
        public JurisdictionModel? Jurisdiction { get; set; }
        public FactionModel? RunByFaction { get; set; }

        public BankModel()
        {
            Name = string.Empty;
            Code = string.Empty;
        }
    }
}
