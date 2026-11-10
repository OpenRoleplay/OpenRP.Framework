using OpenRP.Framework.Database.Entities;

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

        public PhoneModel()
        {
            IsOn = true;
            IsSilent = false;
        }
    }
}
