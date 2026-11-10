using SampSharp.Entities;
using System;

namespace OpenRP.Framework.Features.EmergencyCalls.Components
{
    /// <summary>
    /// On a player whose 911 call was dispatched recently: the call and when. A new call can't dispatch again until a
    /// minute has passed, which stops prank floods; the same call may still send updates.
    /// </summary>
    public class EmergencyDispatchCooldown : Component
    {
        private readonly EntityId _callId;
        private readonly DateTime _dispatchedAt;

        public EmergencyDispatchCooldown(EntityId callId, DateTime dispatchedAt)
        {
            _callId = callId;
            _dispatchedAt = dispatchedAt;
        }

        public bool IsBlocking(EntityId callId, TimeSpan cooldown)
        {
            return callId != _callId && DateTime.UtcNow - _dispatchedAt < cooldown;
        }
    }
}
