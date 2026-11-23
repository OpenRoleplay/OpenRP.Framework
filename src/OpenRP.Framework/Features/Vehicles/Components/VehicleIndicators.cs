using OpenRP.Framework.Features.Vehicles.Enums;
using OpenRP.OpenMp.Streamer;
using SampSharp.Entities;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Vehicles.Components
{
    /// <summary>
    /// Marks a vehicle whose turn signals are on. It lists the blinker objects it put on the car, so they can be removed
    /// with it. The blinkers are children of the vehicle's entity, so they go with the vehicle too; the component is
    /// removed when the signals go off.
    /// </summary>
    public class VehicleIndicators : Component
    {
        private readonly EntityId _driverEntityId;
        private readonly IndicatorState _state;
        private readonly List<DynamicObject> _blinkers = new List<DynamicObject>();

        public VehicleIndicators(EntityId driverEntityId, IndicatorState state)
        {
            _driverEntityId = driverEntityId;
            _state = state;
        }

        /// <summary>
        /// Which signals are on.
        /// </summary>
        public IndicatorState GetState()
        {
            return _state;
        }

        /// <summary>
        /// True when this player is the one who switched the signals on. The driver id can be stale, for hazards that stayed on after the driver left.
        /// </summary>
        public bool IsDrivenBy(EntityId driverEntityId)
        {
            return _driverEntityId == driverEntityId;
        }

        /// <summary>
        /// Keeps a blinker object of these signals, so it can be removed with them.
        /// </summary>
        public void AddBlinker(DynamicObject blinker)
        {
            _blinkers.Add(blinker);
        }

        /// <summary>
        /// The blinker objects these signals put on the vehicle.
        /// </summary>
        public List<DynamicObject> GetBlinkers()
        {
            return new List<DynamicObject>(_blinkers);
        }
    }
}
