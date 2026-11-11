using OpenRP.Framework.Database.Entities;
using System;

namespace OpenRP.Framework.Database.Models
{
    /// <summary>
    /// A mobile phone with its own number. The phone item in an inventory points to it with [PHONE_ID=x].
    /// </summary>
    public class PhoneModel : BaseModel
    {
        // A random 7-digit number; numbers below 1000 are reserved for the server, such as 911.
        public uint Number { get; set; }
        public bool IsOn { get; set; }
        // A silent phone doesn't ring or beep out loud; only its carrier notices.
        public bool IsSilent { get; set; }
        // Percent, 0 to 100.
        public double BatteryLevel { get; set; }
        // When the battery was last updated while in play; null when never. See Shared/Batteries.
        public DateTime? BatteryUpdatedAt { get; set; }

        public PhoneModel()
        {
            IsOn = true;
            IsSilent = false;
            BatteryLevel = 100;
        }
    }
}
