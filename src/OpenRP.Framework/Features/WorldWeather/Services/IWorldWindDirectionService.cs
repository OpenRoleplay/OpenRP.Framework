using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.WorldWeather.Services
{
    public interface IWorldWindDirectionService
    {
        /// <summary>
        /// Gets the wind direction (e.g., "N", "NE", etc.) for the specified time.
        /// </summary>
        string GetWindDirection(DateTime time);

        /// <summary>
        /// Calculates a world-space offset based on the current wind direction and provided wind speed.
        /// The x-axis spans -3000 (west) to 3000 (east) and y-axis from -3000 (south) to 3000 (north).
        /// </summary>
        Vector2 GetWindOffset(double windSpeed);
    }
}
