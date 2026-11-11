using OpenRP.Framework.Database.Entities;
using System;

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
        // Percent, 0 to 100. Vehicle radios run on the car and ignore it.
        public double BatteryLevel { get; set; }
        // When the battery was last updated while in play; null when never. See Shared/Batteries.
        public DateTime? BatteryUpdatedAt { get; set; }
        // The vehicle the radio is installed in; null for a handheld.
        public ulong? VehicleId { get; set; }

        // Navigational Properties
        public VehicleModel? Vehicle { get; set; }

        public RadioModel()
        {
            Frequency = 1000;
            IsOn = true;
            UsesEarpiece = false;
            BatteryLevel = 100;
        }
    }
}
