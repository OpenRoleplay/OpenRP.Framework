using System.Numerics;

namespace OpenRP.Framework.Features.Fishing.Services
{
    public interface IFishHotspotService
    {
        /// <summary>
        /// Places all hotspots on the water. Called once at startup.
        /// </summary>
        void CreateHotspots();

        /// <summary>
        /// Moves every hotspot to a new spot on the water.
        /// </summary>
        void MoveHotspots();

        /// <summary>
        /// True when the position is within range of a hotspot.
        /// </summary>
        bool IsNearHotspot(Vector3 position);
    }
}
