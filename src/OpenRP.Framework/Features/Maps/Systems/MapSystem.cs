using OpenRP.Framework.Features.Maps.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Maps.Systems
{
    public class MapSystem : ISystem
    {
        private const int SyncIntervalInMilliseconds = 60000;

        /// <summary>
        /// Maps go first because objects and removed buildings are their child entities, and the objects of a loaded
        /// map are spawned once all of them are loaded.
        /// </summary>
        [Event]
        public void OnGameModeInit(IMapManager mapManager, IMapObjectManager mapObjectManager, IMapObjectMaterialManager mapObjectMaterialManager,
            IMapRemovedBuildingManager mapRemovedBuildingManager, IMapService mapService)
        {
            mapManager.LoadAll();
            mapObjectManager.LoadAll();
            mapObjectMaterialManager.LoadAll();
            mapRemovedBuildingManager.LoadAll();
            mapService.SpawnLoadedMaps();
        }

        /// <summary>
        /// Children are saved before the maps they belong to, so their deletes run before the database cascades
        /// a map's delete to them.
        /// </summary>
        [Timer(SyncIntervalInMilliseconds)]
        public void ProcessChanges(IMapManager mapManager, IMapObjectManager mapObjectManager, IMapObjectMaterialManager mapObjectMaterialManager,
            IMapRemovedBuildingManager mapRemovedBuildingManager)
        {
            mapRemovedBuildingManager.ProcessChanges();
            mapObjectMaterialManager.ProcessChanges();
            mapObjectManager.ProcessChanges();
            mapManager.ProcessChanges();
        }

        [Event]
        public void OnGameModeExit(IMapManager mapManager, IMapObjectManager mapObjectManager, IMapObjectMaterialManager mapObjectMaterialManager,
            IMapRemovedBuildingManager mapRemovedBuildingManager)
        {
            mapRemovedBuildingManager.SaveNow();
            mapObjectMaterialManager.SaveNow();
            mapObjectManager.SaveNow();
            mapManager.SaveNow();
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
