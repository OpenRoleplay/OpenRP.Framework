using OpenRP.Framework.Features.Maps.Components;
using OpenRP.Framework.Features.Maps.Enums;

namespace OpenRP.Framework.Features.Maps.Services
{
    /// <summary>
    /// Which maps are busy. A busy map refuses every change to it, and its objects are not clicked or edited in the game.
    /// Main thread only.
    /// </summary>
    public interface IMapBusyTracker
    {
        /// <summary>
        /// Why the map is busy, or null when it isn't. Null for a null map.
        /// </summary>
        MapBusyReason? GetReason(Map? map);

        /// <summary>
        /// Marks the map busy for this reason, replacing the reason it had.
        /// </summary>
        void Set(Map map, MapBusyReason reason);

        /// <summary>
        /// Lets changes to the map through again.
        /// </summary>
        void Clear(Map map);
    }
}
