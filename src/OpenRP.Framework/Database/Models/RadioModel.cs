using OpenRP.Framework.Database.Entities;

namespace OpenRP.Framework.Database.Models
{
    /// <summary>
    /// A radio: a handheld whose item points to it with [RADIO_ID=x], or one installed in a vehicle (VehicleId).
    /// </summary>
    public class RadioModel : BaseModel
    {
        public uint Frequency { get; set; }
        public bool IsOn { get; set; }
        // Only counts while the holder carries an earpiece item; otherwise the radio plays out loud.
        public bool UsesEarpiece { get; set; }
        // The vehicle the radio is installed in; null for a handheld.
        public ulong? VehicleId { get; set; }

        // Navigational Properties
        public VehicleModel? Vehicle { get; set; }

        public RadioModel()
        {
            Frequency = 1000;
            IsOn = true;
            UsesEarpiece = false;
        }
    }
}
