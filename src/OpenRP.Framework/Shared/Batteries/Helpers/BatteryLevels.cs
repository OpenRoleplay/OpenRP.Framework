using System;
using System.Linq;

namespace OpenRP.Framework.Shared.Batteries.Helpers
{
    /// <summary>
    /// The battery math of phones and handheld radios, in percent (0 to 100) and real time.
    /// </summary>
    public static class BatteryLevels
    {
        public const double Full = 100;

        /// <summary>
        /// Each of these gives a warning when the level drops past it.
        /// </summary>
        public static readonly double[] WarningLevels = { 10, 5, 2 };

        /// <summary>
        /// Recharging from empty to full takes this long; a partly charged battery takes proportionally less.
        /// </summary>
        public static readonly TimeSpan MaxChargeDuration = TimeSpan.FromSeconds(60);

        /// <summary>
        /// A device in an offline character's inventory starts recharging after this long...
        /// </summary>
        public static readonly TimeSpan OfflineRechargeDelay = TimeSpan.FromHours(1);

        /// <summary>
        /// ...and then goes from empty to full in this long.
        /// </summary>
        public static readonly TimeSpan OfflineRechargeDuration = TimeSpan.FromHours(24);

        /// <summary>
        /// A gap between updates up to this long is time in play (a few update ticks); a longer one is time away.
        /// </summary>
        public static readonly TimeSpan LongestInPlayGap = TimeSpan.FromMinutes(2);

        /// <summary>
        /// The level after the time since the last update: drained while in play and on, recharged after an hour or
        /// more away, and unchanged after a shorter time away. A device never updated counts as updated now.
        /// </summary>
        public static double GetLevelAfter(double level, DateTime? updatedAt, DateTime now, bool isOn, TimeSpan fullLife)
        {
            TimeSpan gap = now - (updatedAt ?? now);
            if (gap >= OfflineRechargeDelay)
            {
                return Clamp(level + (gap - OfflineRechargeDelay) / OfflineRechargeDuration * Full);
            }
            if (gap <= LongestInPlayGap && isOn)
            {
                return Clamp(level - gap / fullLife * Full);
            }
            return level;
        }

        public static double GetDrained(double level, double percentage)
        {
            return Clamp(level - percentage);
        }

        public static TimeSpan GetChargeDuration(double level)
        {
            return MaxChargeDuration * ((Full - Clamp(level)) / Full);
        }

        /// <summary>
        /// The level part-way through a charge, rising evenly from where it started to full.
        /// </summary>
        public static double GetChargingLevel(double startLevel, DateTime startedAt, TimeSpan duration, DateTime now)
        {
            if (duration <= TimeSpan.Zero)
            {
                return Full;
            }
            double progress = Math.Clamp((now - startedAt) / duration, 0, 1);
            return Clamp(startLevel + (Full - startLevel) * progress);
        }

        /// <summary>
        /// The warning level the battery just dropped past, or null. Dropping past several at once gives the lowest.
        /// </summary>
        public static double? GetCrossedWarningLevel(double oldLevel, double newLevel)
        {
            double[] crossed = WarningLevels.Where(warningLevel => oldLevel > warningLevel && newLevel <= warningLevel).ToArray();
            return crossed.Length == 0 ? null : crossed.Min();
        }

        public static bool IsEmpty(double level)
        {
            return level <= 0;
        }

        public static bool IsFull(double level)
        {
            return level >= Full;
        }

        /// <summary>
        /// "73%", rounded up so a battery only reads 0% when it's really empty, or "empty".
        /// </summary>
        public static string Format(double level)
        {
            return IsEmpty(level) ? "empty" : $"{Math.Ceiling(level)}%";
        }

        private static double Clamp(double level)
        {
            return Math.Clamp(level, 0, Full);
        }
    }
}
