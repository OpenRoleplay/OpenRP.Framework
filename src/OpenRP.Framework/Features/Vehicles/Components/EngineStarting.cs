using SampSharp.Entities;
using System;

namespace OpenRP.Framework.Features.Vehicles.Components
{
    /// <summary>
    /// Marks a vehicle whose engine is being started; EngineStartSystem finishes the start once it's due.
    /// </summary>
    public class EngineStarting : Component
    {
        private readonly EntityId _driverEntityId;
        private readonly DateTime _startAt;

        public EngineStarting(EntityId driverEntityId, DateTime startAt)
        {
            _driverEntityId = driverEntityId;
            _startAt = startAt;
        }

        /// <summary>
        /// The player who turned the key.
        /// </summary>
        public EntityId GetDriverEntityId()
        {
            return _driverEntityId;
        }

        /// <summary>
        /// True once the engine should come to a start.
        /// </summary>
        public bool IsDue()
        {
            return DateTime.UtcNow >= _startAt;
        }
    }
}
