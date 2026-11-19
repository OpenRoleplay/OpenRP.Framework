using OpenRP.Framework.Features.Fishing.Entities;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;

namespace OpenRP.Framework.Features.Fishing.Components
{
    /// <summary>
    /// Marks the driver of a Reefer whose net is lowered, and counts the time sailed at trawling speed.
    /// </summary>
    public class PlayerTrawling : Component
    {
        private readonly EntityId _vehicleEntityId;
        private float _countedSeconds;
        private DateTime _nextWarningAt = DateTime.MinValue;

        public PlayerTrawling(Vehicle vehicle)
        {
            _vehicleEntityId = vehicle.Entity;
        }

        public EntityId GetVehicleEntityId()
        {
            return _vehicleEntityId;
        }

        public float GetCountedSeconds()
        {
            return _countedSeconds;
        }

        public void AddCountedSeconds(float seconds)
        {
            _countedSeconds += seconds;
        }

        public bool IsFinished()
        {
            return _countedSeconds >= FishingSettings.TrawlDurationInSeconds;
        }

        /// <summary>
        /// True when the driver hasn't been warned about their speed recently.
        /// </summary>
        public bool IsWarningDue()
        {
            return DateTime.UtcNow >= _nextWarningAt;
        }

        public void SetWarningSent()
        {
            _nextWarningAt = DateTime.UtcNow.AddSeconds(FishingSettings.TrawlWarningIntervalInSeconds);
        }
    }
}
