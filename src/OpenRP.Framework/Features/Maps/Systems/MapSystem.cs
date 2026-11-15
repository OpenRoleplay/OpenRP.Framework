using OpenRP.Framework.Features.Maps.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Maps.Systems
{
    public class MapSystem : ISystem
    {
        /// <summary>
        /// The maps are loaded first, so every object finds its map when the objects are spawned.
        /// </summary>
        [Event]
        public void OnGameModeInit(IMapStore mapStore, IMapService mapService)
        {
            mapStore.LoadAll();
            mapService.SpawnLoadedMaps();
        }

        /// <summary>
        /// Players get the removed buildings of every loaded map as they connect.
        /// </summary>
        [Event]
        public void OnPlayerConnect(Player player, IMapService mapService)
        {
            mapService.ApplyRemovedBuildings(player);
        }
    }
}
