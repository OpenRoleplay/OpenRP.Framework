using OpenRP.Framework.Database.Entities;

namespace OpenRP.Framework.Database.Models
{
    /// <summary>
    /// A place with its own rules: a country, a state or county, or a city. Jurisdictions nest through their parent.
    /// </summary>
    public class JurisdictionModel : BaseModel
    {
        public string Name { get; set; }
        // Like ISO country codes: SA, SAN and USSA. Required, and unique per length.
        public string TwoLetterTag { get; set; }
        public string ThreeLetterTag { get; set; }
        public string FourLetterTag { get; set; }
        // Null for a country.
        public ulong? ParentJurisdictionId { get; set; }
        // Null to use the parent's currency.
        public ulong? CurrencyId { get; set; }
        // Null to be governed by the parent's government.
        public ulong? GovernmentFactionId { get; set; }

        // Navigational Properties
        public JurisdictionModel? ParentJurisdiction { get; set; }
        public CurrencyModel? Currency { get; set; }
        public FactionModel? GovernmentFaction { get; set; }

        public JurisdictionModel()
        {
            Name = string.Empty;
            TwoLetterTag = string.Empty;
            ThreeLetterTag = string.Empty;
            FourLetterTag = string.Empty;
        }
    }
}
