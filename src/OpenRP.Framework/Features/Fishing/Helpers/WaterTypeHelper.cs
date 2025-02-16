using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Fishing.Helpers
{
    public static class WaterZoneHelper
    {
        private readonly struct WaterZone
        {
            public Vector2 Min { get; }
            public Vector2 Max { get; }

            public WaterZone(float x1, float y1, float x2, float y2)
            {
                // Normalize coordinates to ensure Min/Max are correct
                Min = new Vector2(
                    x1 < x2 ? x1 : x2,
                    y1 < y2 ? y1 : y2
                );
                Max = new Vector2(
                    x1 > x2 ? x1 : x2,
                    y1 > y2 ? y1 : y2
                );
            }

            public bool Contains(Vector2 point)
            {
                return point.X >= Min.X &&
                       point.X <= Max.X &&
                       point.Y >= Min.Y &&
                       point.Y <= Max.Y;
            }
        }

        // Freshwater zones (7 zones)
        private static readonly List<WaterZone> _freshWaterZones = new()
        {
            new WaterZone(1621.0f, -689.0f, 2621.0f, 311.0f),    // Freshwater1
            new WaterZone(-783.0f, -1388.0f, 2717.0f, 112.0f),   // Freshwater2
            new WaterZone(620.0f, -1817.0f, 2620.0f, -1417.0f),  // Freshwater3
            new WaterZone(1188.0f, -2431.0f, 1288.0f, -2331.0f), // Freshwater4
            new WaterZone(-1483.0f, 2023.0f, -283.0f, 2848.0f),  // Freshwater5
            new WaterZone(-2882.0f, -1220.0f, -1860.0f, 1220.0f),// Freshwater6
            new WaterZone(-2260.0f, -2634.0f, -910.0f, -1194.0f) // Freshwater7
        };

        // Muddywater zone (1 zone)
        private static readonly List<WaterZone> _muddyWaterZones = new()
        {
            new WaterZone(-876.0f, -2108.0f, -461.0f, -1808.0f)  // Muddywater
        };

        public static bool IsInFreshWaterZone(this Player player)
        {
            return IsInZone(player.Position.XY, _freshWaterZones);
        }

        public static bool IsInMuddyWaterZone(this Player player)
        {
            return IsInZone(player.Position.XY, _muddyWaterZones);
        }

        private static bool IsInZone(Vector2 position, IEnumerable<WaterZone> zones)
        {
            foreach (var zone in zones)
            {
                if (zone.Contains(position))
                    return true;
            }
            return false;
        }
    }
}
