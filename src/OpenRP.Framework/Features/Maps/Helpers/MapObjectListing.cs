using OpenRP.Framework.Features.Maps.Components;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Maps.Helpers
{
    /// <summary>
    /// Picks the objects an object list shows, and the object nearest to a point. Both make one pass over the objects,
    /// because a map can hold hundreds of thousands of them.
    /// </summary>
    public static class MapObjectListing
    {
        /// <summary>
        /// The most objects one list shows. A longer list keeps the ones nearest to the editor.
        /// </summary>
        public const int MaxListedObjects = 200;

        /// <summary>
        /// The objects a list shows, ordered by id, and how many objects matched in all. When there are more than the
        /// maximum, the nearest to the origin are kept, in a queue that never holds more than the maximum.
        /// </summary>
        public static (List<MapObject> ListedObjects, int MatchingCount) GetListedObjects(IEnumerable<MapObject> mapObjects, Vector3 origin)
        {
            // The queue's lowest priority leaves first, so the priority is the negated distance: the farthest object leaves.
            PriorityQueue<MapObject, float> nearestObjects = new PriorityQueue<MapObject, float>(MaxListedObjects + 1);
            int matchingCount = 0;
            foreach (MapObject mapObject in mapObjects)
            {
                matchingCount++;
                nearestObjects.Enqueue(mapObject, -Vector3.DistanceSquared(mapObject.GetPosition(), origin));
                if (nearestObjects.Count > MaxListedObjects)
                {
                    nearestObjects.Dequeue();
                }
            }

            List<MapObject> listedObjects = nearestObjects.UnorderedItems
                .Select(item => item.Element)
                .OrderBy(mapObject => mapObject.GetId())
                .ToList();
            return (listedObjects, matchingCount);
        }

        /// <summary>
        /// The list title, saying how many of the matching objects it shows when it can't show them all.
        /// </summary>
        public static string GetListTitle(string title, int matchingCount)
        {
            return matchingCount > MaxListedObjects ? $"{title} ({MaxListedObjects} nearest of {matchingCount})" : title;
        }

        /// <summary>
        /// The object nearest to the origin within the radius, or null when none is that close. When two are equally near,
        /// the one met first wins.
        /// </summary>
        public static MapObject? FindNearest(IEnumerable<MapObject> mapObjects, Vector3 origin, float radius)
        {
            float radiusSquared = radius * radius;
            float nearestDistanceSquared = float.MaxValue;
            MapObject? nearest = null;
            foreach (MapObject mapObject in mapObjects)
            {
                float distanceSquared = Vector3.DistanceSquared(mapObject.GetPosition(), origin);
                if (distanceSquared <= radiusSquared && distanceSquared < nearestDistanceSquared)
                {
                    nearest = mapObject;
                    nearestDistanceSquared = distanceSquared;
                }
            }
            return nearest;
        }
    }
}
