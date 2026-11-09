using OpenRP.Framework.Database.Entities;

namespace OpenRP.Framework.Database.Models
{
    /// <summary>
    /// A faction registered in a jurisdiction, where it pays taxes.
    /// </summary>
    public class FactionTaxRegistrationModel : BaseModel
    {
        public ulong FactionId { get; set; }
        public ulong JurisdictionId { get; set; }

        // Navigational Properties
        public FactionModel? Faction { get; set; }
        public JurisdictionModel? Jurisdiction { get; set; }
    }
}
