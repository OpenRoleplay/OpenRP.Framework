using OpenRP.Framework.Database.Entities;
using System;

namespace OpenRP.Framework.Database.Models
{
    /// <summary>
    /// A text in a phone's memory, received or sent. Each phone keeps its own copy.
    /// </summary>
    public class PhoneMessageModel : BaseModel
    {
        public ulong PhoneId { get; set; }
        // The number it came from, or went to.
        public uint OtherNumber { get; set; }
        public string Text { get; set; }
        public bool IsIncoming { get; set; }
        public bool IsRead { get; set; }
        public DateTime SentAt { get; set; }

        // Navigational Properties
        public PhoneModel? Phone { get; set; }

        public PhoneMessageModel()
        {
            Text = string.Empty;
        }
    }
}
