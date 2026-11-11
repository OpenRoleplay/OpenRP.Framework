using OpenRP.Framework.Database.Entities;

namespace OpenRP.Framework.Database.Models
{
    /// <summary>
    /// A faction whose members may use a protected or encrypted frequency, together with the factions it controls.
    /// </summary>
    public class RadioFrequencyFactionModel : BaseModel
    {
        public ulong RadioFrequencyId { get; set; }
        public ulong FactionId { get; set; }

        // Navigational Properties
        public RadioFrequencyModel? RadioFrequency { get; set; }
        public FactionModel? Faction { get; set; }
    }
}
