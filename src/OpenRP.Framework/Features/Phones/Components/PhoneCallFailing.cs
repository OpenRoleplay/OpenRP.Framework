using SampSharp.Entities;
using System;

namespace OpenRP.Framework.Features.Phones.Components
{
    /// <summary>
    /// On a player who dialed a number that can't be reached. They hold the phone for a moment before hearing why,
    /// like a real call, which also gives the phone animation time to start before it's stopped again.
    /// </summary>
    public class PhoneCallFailing : Component
    {
        private readonly DateTime _dueAt;
        private readonly string _reason;

        public PhoneCallFailing(DateTime dueAt, string reason)
        {
            _dueAt = dueAt;
            _reason = reason;
        }

        public bool IsDue()
        {
            return DateTime.UtcNow >= _dueAt;
        }

        /// <summary>
        /// What the caller hears, for example "The number you have dialed is not in service."
        /// </summary>
        public string GetReason()
        {
            return _reason;
        }
    }
}
