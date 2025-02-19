using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.WorldTime.Entities
{
    public class WorldTimeOptions
    {
        /// <summary>
        /// Number of in-game day cycles per real day.
        /// For example, 3 means three in-game days occur during one real day.
        /// </summary>
        public int IngameCyclesPerRealDay { get; set; } = 3;

        /// <summary>
        /// The total simulation offset (in seconds) across the entire map horizontally.
        /// For example, 900 means a 15 minute offset from 0, so 30 minutes in total.
        /// </summary>
        public double TotalPositionOffsetInSeconds { get; set; } = 900;

        /// <summary>
        /// The minimum x-coordinate of the map.
        /// </summary>
        public double MapXMin { get; set; } = -3000;

        /// <summary>
        /// The maximum x-coordinate of the map.
        /// </summary>
        public double MapXMax { get; set; } = 3000;

        /// <summary>
        /// A list of exempt dates. On these dates the in-game time matches real UTC time.
        /// Only the Month and Day are used for comparison.
        /// </summary>
        public List<DateTime> ExemptDates { get; set; } = new List<DateTime>();
    }
}
