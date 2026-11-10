using SampSharp.Entities.SAMP;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Shared.Locations.Helpers
{
    /// <summary>
    /// The name of the San Andreas area a position lies in: the three cities and the counties around them.
    /// </summary>
    public static class ZoneNames
    {
        private const string UnknownArea = "San Andreas";

        // Areas as boxes on the map (minimum X, minimum Y, maximum X, maximum Y). They don't overlap, so the order
        // doesn't matter.
        private static readonly List<(string Name, float MinX, float MinY, float MaxX, float MaxY)> Areas = new()
        {
            ("Los Santos", 44.60f, -2892.90f, 2997.00f, -768.00f),
            ("Las Venturas", 869.40f, 596.30f, 2997.00f, 2993.80f),
            ("Bone County", -480.50f, 596.30f, 869.40f, 2993.80f),
            ("Tierra Robada", -2997.40f, 1659.60f, -480.50f, 2993.80f),
            ("Tierra Robada", -1213.90f, 596.30f, -480.50f, 1659.60f),
            ("San Fierro", -2997.40f, -1115.50f, -1213.90f, 1659.60f),
            ("Red County", -1213.90f, -768.00f, 2997.00f, 596.30f),
            ("Flint County", -1213.90f, -2892.90f, 44.60f, -768.00f),
            ("Whetstone", -2997.40f, -2892.90f, -1213.90f, -1115.50f),
        };

        /// <summary>
        /// For example "Red County", or "San Andreas" outside the known areas.
        /// </summary>
        public static string GetZoneName(Vector3 position)
        {
            return Areas.FirstOrDefault(area => position.X >= area.MinX && position.X <= area.MaxX && position.Y >= area.MinY && position.Y <= area.MaxY).Name
                ?? UnknownArea;
        }
    }
}
