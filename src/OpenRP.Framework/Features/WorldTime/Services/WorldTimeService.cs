using Microsoft.Extensions.Options;
using OpenRP.Framework.Features.WorldTime.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.WorldTime.Services
{
    public class WorldTimeService : IWorldTimeService
    {
        private readonly WorldTimeOptions _options;

        public WorldTimeService(IOptions<WorldTimeOptions> options)
        {
            _options = options.Value;
        }

        public DateTime GetCurrentIngameDateTime()
        {
            // Use current UTC date, then subtract 33 years.
            DateTime ingameDate = DateTime.UtcNow.AddYears(-33);
            TimeSpan ingameTime = GetCurrentIngameTime();

            return new DateTime(ingameDate.Year, ingameDate.Month, ingameDate.Day, ingameTime.Hours, ingameTime.Minutes, ingameTime.Seconds);
        }

        public TimeSpan GetCurrentIngameTime()
        {
            DateTime nowUtc = DateTime.UtcNow;

            // If today is an exempt date, return the real UTC time-of-day.
            if (IsExemptDate(nowUtc))
            {
                return nowUtc.TimeOfDay;
            }

            // Compute the seconds since midnight in real time.
            double realSeconds = nowUtc.TimeOfDay.TotalSeconds;

            // Scale the time by the number of in-game cycles per real day.
            // For example, if cycles = 3, then one real day (24h) corresponds to 3 in-game days.
            double scaledSeconds = realSeconds * _options.IngameCyclesPerRealDay;

            // Wrap around a 24-hour in-game day (24 * 3600 seconds).
            double inGameSeconds = scaledSeconds % (24 * 3600);

            return TimeSpan.FromSeconds(inGameSeconds);
        }

        public TimeSpan GetCurrentSimulationTime(float positionX)
        {
            // Start with the base in-game time.
            TimeSpan baseTime = GetCurrentIngameTime();

            // Limit Simulation time between -3000 and 3000
            double clampedX = Math.Max(_options.MapXMin, Math.Min(positionX, _options.MapXMax));

            // Compute a fraction where MapXMin (e.g. -3000) corresponds to 0 and MapXMax (e.g. 3000) corresponds to 1.
            double fraction = (clampedX - _options.MapXMin) / (_options.MapXMax - _options.MapXMin);

            // Convert fraction to a value between -1 (west edge) and +1 (east edge).
            double factor = (fraction - 0.5) * 2;

            // Multiply the factor by your configured offset (in seconds).
            // To have a total difference of 30 minutes, set TotalPositionOffsetInSeconds to 900.
            double offsetSeconds = factor * _options.TotalPositionOffsetInSeconds;

            // Compute simulation time, wrapping around at 24 hours.
            double simSeconds = (baseTime.TotalSeconds + offsetSeconds) % (24 * 3600);
            return TimeSpan.FromSeconds(simSeconds);
        }

        private bool IsExemptDate(DateTime utcNow)
        {
            // Compare Month and Day of each exempt date.
            return _options.ExemptDates.Any(exempt =>
                utcNow.Month == exempt.Month && utcNow.Day == exempt.Day);
        }
    }
}
