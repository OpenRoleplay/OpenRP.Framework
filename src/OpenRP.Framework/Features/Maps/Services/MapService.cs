using Microsoft.Extensions.Logging;
using OpenRP.Framework.Features.Maps.Components;
using OpenRP.Framework.Features.Maps.Entities;
using OpenRP.Framework.Features.Maps.Enums;
using OpenRP.Framework.Features.Maps.Helpers;
using OpenRP.OpenMp.Streamer;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Maps.Services
{
    public class MapService : IMapService
    {
        private readonly IEntityManager _entityManager;
        private readonly IStreamerService _streamerService;
        private readonly IMapStore _mapStore;
        private readonly IMapObjectSpawnHandler[] _spawnHandlers;
        private readonly ILogger<MapService> _logger;

        // Kept apart from the objects, because a loaded map with no objects is spawned too. A map is in this set while its
        // objects are being spawned or are spawned.
        private readonly HashSet<ulong> _spawnedMapIds = new HashSet<ulong>();

        // The handlers that take each model, filled the first time the model spawns. Cached per model, because checking
        // every handler for each of the ~400k objects would be slow.
        private readonly Dictionary<int, IMapObjectSpawnHandler[]> _spawnHandlersByModelId = new Dictionary<int, IMapObjectSpawnHandler[]>();

        public MapService(IEntityManager entityManager, IStreamerService streamerService, IMapStore mapStore,
            IEnumerable<IMapObjectSpawnHandler> spawnHandlers, ILogger<MapService> logger)
        {
            _entityManager = entityManager;
            _streamerService = streamerService;
            _mapStore = mapStore;
            _spawnHandlers = spawnHandlers.ToArray();
            _logger = logger;
        }

        public IReadOnlyList<Map> GetMaps()
        {
            return _mapStore.GetMaps();
        }

        public Map? GetMap(ulong id)
        {
            return _mapStore.GetMap(id);
        }

        public Map? FindMapByName(string name)
        {
            return _mapStore.FindMapByName(name);
        }

        public void SpawnLoadedMaps()
        {
            foreach (Map map in GetMaps().Where(map => map.IsLoaded()))
            {
                SpawnMap(map);
            }
        }

        public void SpawnMap(Map map)
        {
            DespawnMap(map);
            _spawnedMapIds.Add(map.GetId());
            foreach (MapObject mapObject in _mapStore.GetObjects(map))
            {
                SpawnIfOnLoadedMap(mapObject);
            }
        }

        public async Task SpawnMapAsync(Map map, IProgress<int>? progress = null)
        {
            await DespawnMapAsync(map);
            _spawnedMapIds.Add(map.GetId());
            List<MapObject> mapObjects = _mapStore.GetObjects(map).ToList();
            await MapSlices.RunAsync(mapObjects, SpawnIfOnLoadedMap, progress);
        }

        public void DespawnMap(Map map)
        {
            if (!_spawnedMapIds.Remove(map.GetId()))
            {
                return;
            }
            foreach (MapObject mapObject in _mapStore.GetObjects(map))
            {
                DespawnObject(mapObject);
            }
        }

        public async Task DespawnMapAsync(Map map)
        {
            if (!_spawnedMapIds.Contains(map.GetId()))
            {
                return;
            }
            List<MapObject> mapObjects = _mapStore.GetObjects(map).ToList();
            await MapSlices.RunAsync(mapObjects, DespawnObject);
            _spawnedMapIds.Remove(map.GetId());
        }

        public void RespawnObject(MapObject mapObject)
        {
            DespawnObject(mapObject);
            SpawnIfOnLoadedMap(mapObject);
        }

        public void DespawnObject(MapObject mapObject)
        {
            DestroyDynamicObjects(mapObject);
        }

        public void MoveSpawnedObject(MapObject mapObject)
        {
            DynamicObject? dynamicObject = GetDynamicObject(mapObject);
            if (dynamicObject == null)
            {
                return;
            }
            dynamicObject.Position = mapObject.GetPosition();
            dynamicObject.Rotation = mapObject.GetRotation();
            NotifyMoved(mapObject, dynamicObject);
        }

        public DynamicObject? GetDynamicObject(MapObject mapObject)
        {
            return mapObject.GetComponentInChildren<DynamicObject>();
        }

        public MapObject? GetMapObject(DynamicObject dynamicObject)
        {
            return dynamicObject.GetComponentInParent<MapObject>();
        }

        public void ApplyRemovedBuildings(Player player)
        {
            foreach (MapRemovedBuilding removal in GetLoadedRemovedBuildings())
            {
                ApplyRemovedBuildingTo(player, removal);
            }
        }

        public void ApplyRemovedBuilding(MapRemovedBuilding removal)
        {
            foreach (Player player in _entityManager.GetComponents<Player>().ToList())
            {
                ApplyRemovedBuildingTo(player, removal);
            }
        }

        private IEnumerable<MapRemovedBuilding> GetLoadedRemovedBuildings()
        {
            return GetMaps().Where(map => map.IsLoaded()).SelectMany(map => map.GetRemovedBuildings());
        }

        /// <summary>
        /// Spawns the object when its map is loaded and it isn't spawned yet. The check on every object lets a slice spawn
        /// stop short when its map was unloaded or its object deleted meanwhile.
        /// </summary>
        private void SpawnIfOnLoadedMap(MapObject mapObject)
        {
            if (mapObject.IsComponentAlive && IsOnLoadedMap(mapObject) && GetDynamicObject(mapObject) == null)
            {
                SpawnObject(mapObject);
            }
        }

        private static bool IsOnLoadedMap(MapObject mapObject)
        {
            Map? map = mapObject.GetMap();
            return map != null && map.IsLoaded();
        }

        private void SpawnObject(MapObject mapObject)
        {
            DynamicObject dynamicObject = _streamerService.CreateDynamicObject(
                mapObject.GetModelId(), mapObject.GetPosition(), mapObject.GetRotation(),
                mapObject.GetVirtualWorld(), mapObject.GetInterior(), streamDistance: mapObject.GetStreamDistance(),
                parent: mapObject.Entity);

            foreach (MapObjectMaterial material in mapObject.GetMaterials())
            {
                ApplyMaterial(dynamicObject, material);
            }
            NotifySpawned(mapObject, dynamicObject);
        }

        /// <summary>
        /// Tells the handlers of the object's model that its streamer object now exists, so they can attach behaviour to it.
        /// </summary>
        private void NotifySpawned(MapObject mapObject, DynamicObject dynamicObject)
        {
            foreach (IMapObjectSpawnHandler handler in GetSpawnHandlers(mapObject.GetModelId()))
            {
                RunHandler(handler, mapObject, () => handler.OnMapObjectSpawned(mapObject, dynamicObject));
            }
        }

        private void NotifyMoved(MapObject mapObject, DynamicObject dynamicObject)
        {
            foreach (IMapObjectSpawnHandler handler in GetSpawnHandlers(mapObject.GetModelId()))
            {
                RunHandler(handler, mapObject, () => handler.OnMapObjectMoved(mapObject, dynamicObject));
            }
        }

        private IMapObjectSpawnHandler[] GetSpawnHandlers(int modelId)
        {
            if (!_spawnHandlersByModelId.TryGetValue(modelId, out IMapObjectSpawnHandler[]? handlers))
            {
                handlers = _spawnHandlers.Where(handler => TryHandlesModel(handler, modelId)).ToArray();
                _spawnHandlersByModelId[modelId] = handlers;
            }
            return handlers;
        }

        /// <summary>
        /// A handler that throws while it checks the model is treated as not handling it, so the spawns go on.
        /// </summary>
        private bool TryHandlesModel(IMapObjectSpawnHandler handler, int modelId)
        {
            try
            {
                return handler.Handles(modelId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Map object spawn handler {Handler} failed while checking model {ModelId}.", handler.GetType().Name, modelId);
                return false;
            }
        }

        /// <summary>
        /// One handler's call. A handler that throws is logged, and the other handlers and the spawns still go on.
        /// </summary>
        private void RunHandler(IMapObjectSpawnHandler handler, MapObject mapObject, Action callHandler)
        {
            try
            {
                callHandler();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Map object spawn handler {Handler} failed on map object {MapObjectId}.", handler.GetType().Name, mapObject.GetId());
            }
        }

        /// <summary>
        /// Destroys the streamer objects below the component. The components are copied first, because destroying
        /// one changes the entity tree.
        /// </summary>
        private static void DestroyDynamicObjects(Component owner)
        {
            foreach (DynamicObject dynamicObject in owner.GetComponentsInChildren<DynamicObject>().ToList())
            {
                dynamicObject.DestroyEntity();
            }
        }

        private static void ApplyMaterial(DynamicObject dynamicObject, MapObjectMaterial material)
        {
            if (material.GetMaterialType() == MapObjectMaterialType.Text)
            {
                ApplyMaterialText(dynamicObject, material);
            }
            else
            {
                ApplyMaterialTexture(dynamicObject, material);
            }
        }

        private static void ApplyMaterialTexture(DynamicObject dynamicObject, MapObjectMaterial material)
        {
            int? modelId = material.GetTextureModelId();
            string? txdName = material.GetTxdName();
            string? textureName = material.GetTextureName();
            if (modelId == null || txdName == null || textureName == null)
            {
                return;
            }

            dynamicObject.SetMaterial(material.GetMaterialIndex(), modelId.Value, txdName, textureName, material.GetColor());
        }

        private static void ApplyMaterialText(DynamicObject dynamicObject, MapObjectMaterial material)
        {
            dynamicObject.SetMaterialText(
                material.GetMaterialIndex(), material.GetText() ?? string.Empty,
                material.GetMaterialSize(), material.GetFontFace() ?? string.Empty, material.GetFontSize(), material.GetBold(),
                material.GetFontColor(), material.GetBackColor(), material.GetAlignment());
        }

        private static void ApplyRemovedBuildingTo(Player player, MapRemovedBuilding removal)
        {
            player.RemoveDefaultObjects(removal.GetModelId(), removal.GetPosition(), removal.GetRadius());
        }
    }
}
