using OpenRP.Framework.Database.Entities;
using OpenRP.Framework.Features.Phones.Enums;
using System;

namespace OpenRP.Framework.Database.Models
{
    /// <summary>
    /// A call in a phone's call log: received, dialed or missed.
    /// </summary>
    public class PhoneCallLogModel : BaseModel
    {
        public ulong PhoneId { get; set; }
        public uint OtherNumber { get; set; }
        public PhoneCallLogType Type { get; set; }
        public DateTime StartedAt { get; set; }
        public int DurationSeconds { get; set; }
        // A missed call stays new until the call log is opened.
        public bool IsSeen { get; set; }

        // Navigational Properties
        public PhoneModel? Phone { get; set; }
    }
}
