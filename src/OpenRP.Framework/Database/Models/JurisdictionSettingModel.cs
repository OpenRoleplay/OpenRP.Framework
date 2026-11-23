using OpenRP.Framework.Database.Entities;

namespace OpenRP.Framework.Database.Models
{
    /// <summary>
    /// A setting of a jurisdiction: its own value, and for a country, whether it binds its sub-jurisdictions to it.
    /// </summary>
    public class JurisdictionSettingModel : BaseModel
    {
        public ulong JurisdictionId { get; set; }
        // For example banking.maxMonthlyFee. Unique per jurisdiction.
        public string Key { get; set; }
        // Null when the jurisdiction has no value of its own and inherits one.
        public string? Value { get; set; }
        // Only a country can bind its sub-jurisdictions.
        public bool BindsSubJurisdictions { get; set; }

        // Navigational Properties
        public JurisdictionModel? Jurisdiction { get; set; }

        public JurisdictionSettingModel()
        {
            Key = string.Empty;
        }
    }
}
