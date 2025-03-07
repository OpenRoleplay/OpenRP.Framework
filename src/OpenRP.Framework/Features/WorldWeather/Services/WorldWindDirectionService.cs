using OpenRP.Framework.Features.WorldTime.Services;
using OpenRP.Framework.Features.WorldWeather.Enums;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.WorldWeather.Services
{
    public class WorldWindDirectionService : IWorldWindDirectionService
    {
        // Total hours scheduled (7 days).
        private const int TotalHoursScheduled = 7 * 24; // 168 hours

        private List<WindDirection> _schedule;
        private DateTime _scheduleStart;
        private readonly Random _random;
        private readonly IWorldTimeService _worldTimeService;

        public WorldWindDirectionService(IWorldTimeService worldTimeService)
        {
            _random = new Random();
            _worldTimeService = worldTimeService;
            _scheduleStart = RoundDownToHour(_worldTimeService.GetCurrentIngameDateTime());
            _schedule = new List<WindDirection>(TotalHoursScheduled);
            GenerateInitialSchedule();
        }

        /// <summary>
        /// Rounds the given DateTime down to the nearest whole hour.
        /// </summary>
        private DateTime RoundDownToHour(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0, dt.Kind);
        }

        /// <summary>
        /// Generates the initial wind direction schedule covering the next 168 hours.
        /// </summary>
        private void GenerateInitialSchedule()
        {
            // Pick an initial random wind direction.
            WindDirection initial = (WindDirection)_random.Next(8);
            _schedule.Add(initial);

            // Fill the schedule with gradual changes: each hour changes by -1, 0, or +1 steps.
            for (int i = 1; i < TotalHoursScheduled; i++)
            {
                int change = _random.Next(3) - 1; // yields -1, 0, or 1.
                int newIndex = (((int)_schedule[i - 1] + change) + 8) % 8;
                _schedule.Add((WindDirection)newIndex);
            }
        }

        /// <summary>
        /// Updates the schedule by removing any hours that have passed and appending new ones
        /// so that the schedule always covers the next 7 days from the current time.
        /// </summary>
        private void UpdateRollingSchedule()
        {
            DateTime now = RoundDownToHour(_worldTimeService.GetCurrentIngameDateTime());
            int hoursPassed = (int)((now - _scheduleStart).TotalHours);
            if (hoursPassed > 0)
            {
                // Remove the outdated entries.
                _schedule.RemoveRange(0, hoursPassed);
                // Update the schedule start.
                _scheduleStart = _scheduleStart.AddHours(hoursPassed);
                // Append new entries to maintain a total of 168 hours.
                for (int i = 0; i < hoursPassed; i++)
                {
                    // Use the last available direction and vary it slightly.
                    WindDirection lastDirection = _schedule.Last();
                    int change = _random.Next(3) - 1;
                    int newIndex = ((int)lastDirection + change + 8) % 8;
                    _schedule.Add((WindDirection)newIndex);
                }
            }
        }

        /// <summary>
        /// Returns the wind direction as a string (e.g., "N", "NE", etc.) for the provided time.
        /// </summary>
        public string GetWindDirection(DateTime time)
        {
            // Ensure the schedule is updated.
            UpdateRollingSchedule();

            // Calculate the index corresponding to the provided time.
            double hoursDiff = (time - _scheduleStart).TotalHours;
            int index = (int)Math.Floor(hoursDiff) % TotalHoursScheduled;
            return _schedule[index].ToString();
        }

        /// <summary>
        /// Returns a world-space offset (Vector2) for the provided wind speed,
        /// based on the current wind direction.
        /// </summary>
        public Vector2 GetWindOffset(double windSpeed)
        {
            // Get the current wind direction based on UTC time.
            string windDirStr = GetWindDirection(_worldTimeService.GetCurrentIngameDateTime());
            WindDirection windDir = (WindDirection)Enum.Parse(typeof(WindDirection), windDirStr);
            double angleDeg = GetAngleForDirection(windDir);
            double angleRad = angleDeg * (Math.PI / 180);

            // Assume a maximum wind speed of 12 m/s corresponds to an offset of 3000 units.
            double scale = (windSpeed / 12.0) * 0.25;
            double offsetX = Math.Cos(angleRad) * scale;
            double offsetY = Math.Sin(angleRad) * scale;
            return new Vector2((float)offsetX, (float)offsetY);
        }

        /// <summary>
        /// Maps a WindDirection to an angle in degrees.
        /// In this coordinate system, 0° = East, 90° = North, 180° = West, and -90° = South.
        /// </summary>
        private double GetAngleForDirection(WindDirection direction)
        {
            switch (direction)
            {
                case WindDirection.N: return 90;
                case WindDirection.NE: return 45;
                case WindDirection.E: return 0;
                case WindDirection.SE: return -45;
                case WindDirection.S: return -90;
                case WindDirection.SW: return -135;
                case WindDirection.W: return 180;
                case WindDirection.NW: return 135;
                default: return 0;
            }
        }
    }
}
