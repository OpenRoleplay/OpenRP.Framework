using OpenRP.Framework.Shared.Batteries.Helpers;
using SampSharp.Entities;
using System;

namespace OpenRP.Framework.Shared.Batteries.Entities
{
    /// <summary>
    /// A recharge in progress: from which level, since when, for how long, and by whom. It only lives while the server
    /// runs; a restart ends it at the level it had reached at the last battery update.
    /// </summary>
    public sealed class BatteryCharge
    {
        public double StartLevel { get; }
        public DateTime StartedAt { get; }
        public TimeSpan Duration { get; }
        public EntityId Charger { get; }

        public BatteryCharge(double startLevel, DateTime startedAt, EntityId charger)
        {
            StartLevel = startLevel;
            StartedAt = startedAt;
            Duration = BatteryLevels.GetChargeDuration(startLevel);
            Charger = charger;
        }

        public double GetLevel(DateTime now)
        {
            return BatteryLevels.GetChargingLevel(StartLevel, StartedAt, Duration, now);
        }

        public bool IsDone(DateTime now)
        {
            return now - StartedAt >= Duration;
        }
    }
}
