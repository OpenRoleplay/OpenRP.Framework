using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.WorldTime.Services
{
    public interface IWorldTimeService
    {
        /// <summary>
        /// Gets the current simulation datetime.
        /// This is the current UTC date minus 33 years with the ingame time.
        /// </summary>
        DateTime GetCurrentIngameDateTime();

        /// <summary>
        /// Gets the current in-game time.
        /// The in-game time is scaled by the configured number of cycles per real day.
        /// If today is an exempt date, the in-game time equals the real UTC time.
        /// </summary>
        TimeSpan GetCurrentIngameTime();

        /// <summary>
        /// Gets the current simulation time for a given x-coordinate.
        /// The further west (i.e. lower x), the later the time, based on a linear interpolation.
        /// </summary>
        /// <param name="positionX">The x-coordinate on the map.</param>
        TimeSpan GetCurrentSimulationTime(float positionX);
    }
}
