using OpenRP.Framework.Features.Maps.Components;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Maps.Helpers
{
    /// <summary>
    /// Picks the objects an object list shows, and the object nearest to a point.
    /// </summary>
    public static class MapObjectListing
    {
        /// <summary>
        /// The most objects one list shows. A longer list keeps the ones nearest to the editor.
        /// </summary>
        public const int MaxListedObjects = 200;

        /// <summary>
        /// The objects a list shows, ordered by id. When there are more than the maximum, the nearest to the origin are kept.
        /// </summary>
        public static List<MapObject> GetListedObjects(IEnumerable<MapObject> mapObjects, Vector3 origin)
        {
            return mapObjects
                .OrderBy(mapObject => Vector3.DistanceSquared(mapObject.GetPosition(), origin))
                .Take(MaxListedObjects)
                .OrderBy(mapObject => mapObject.GetId())
                .ToList();
        }

        /// <summary>
        /// The list title, saying how many of the matching objects it shows when it can't show them all.
        /// </summary>
        public static string GetListTitle(string title, int matchingCount)
        {
            return matchingCount > MaxListedObjects ? $"{title} ({MaxListedObjects} nearest of {matchingCount})" : title;
        }

        /// <summary>
        /// The object nearest to the origin within the radius, or null when none is that close.
        /// </summary>
        public static MapObject? FindNearest(IEnumerable<MapObject> mapObjects, Vector3 origin, float radius)
        {
            return mapObjects
                .Where(mapObject => Vector3.Distance(mapObject.GetPosition(), origin) <= radius)
                .OrderBy(mapObject => Vector3.DistanceSquared(mapObject.GetPosition(), origin))
                .FirstOrDefault();
        }
    }
}
