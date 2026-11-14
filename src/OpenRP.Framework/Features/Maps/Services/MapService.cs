using OpenRP.Framework.Features.Maps.Components;
using OpenRP.Framework.Features.Maps.Enums;
using OpenRP.OpenMp.Streamer;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Maps.Services
{
    public class MapService : IMapService
    {
        private readonly IEntityManager _entityManager;
        private readonly IStreamerService _streamerService;

        // Set by SpawnLoadedMaps. Until then, the startup load is still running and spawns everything itself.
        private bool _hasSpawnedLoadedMaps;

        // Kept apart from the objects, because a loaded map with no objects is spawned too.
        private readonly HashSet<ulong> _spawnedMapIds = new HashSet<ulong>();

        public MapService(IEntityManager entityManager, IStreamerService streamerService)
        {
            _entityManager = entityManager;
            _streamerService = streamerService;
        }

        public IReadOnlyList<Map> GetMaps()
        {
            return GetLiveMaps()
                .OrderBy(map => map.GetName(), StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public Map? GetMap(ulong id)
        {
            return GetLiveMaps().FirstOrDefault(map => map.GetId() == id);
        }

        public Map? FindMapByName(string name)
        {
            return GetLiveMaps().FirstOrDefault(map => string.Equals(map.GetName(), name, StringComparison.OrdinalIgnoreCase));
        }

        public Map? FindDeletedMapByName(string name)
        {
            return _entityManager.GetComponents<Map>()
                .FirstOrDefault(map => map.IsDeleted() && string.Equals(map.GetName(), name, StringComparison.OrdinalIgnoreCase));
        }

        public void SpawnLoadedMaps()
        {
            foreach (Map map in GetMaps().Where(map => map.IsLoaded()))
            {
                SpawnMap(map);
            }
            _hasSpawnedLoadedMaps = true;
        }

        public void SpawnMap(Map map)
        {
            DespawnMap(map);
            foreach (MapObject mapObject in map.GetObjects())
            {
                SpawnObject(mapObject);
            }
            _spawnedMapIds.Add(map.GetId());
        }

        public void DespawnMap(Map map)
        {
            DestroyDynamicObjects(map);
            _spawnedMapIds.Remove(map.GetId());
        }

        public void RespawnObject(MapObject mapObject)
        {
            DespawnObject(mapObject);
            if (IsOnLoadedMap(mapObject))
            {
                SpawnObject(mapObject);
            }
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
        }

        public void RespawnChangedObject(MapObject mapObject)
        {
            if (_hasSpawnedLoadedMaps)
            {
                RespawnObject(mapObject);
            }
        }

        /// <summary>
        /// A map that was loaded or unloaded on another server is spawned or despawned to match. A map whose objects are
        /// already in the state its flag asks for is left alone. A map that gets spawned also applies its removed
        /// buildings to the online players, as if it were loaded here.
        /// </summary>
        public void SyncChangedMap(Map map)
        {
            if (!_hasSpawnedLoadedMaps)
            {
                return;
            }

            bool isSpawned = _spawnedMapIds.Contains(map.GetId());
            if (map.IsLoaded() && !isSpawned)
            {
                SpawnMap(map);
                ApplyMapRemovedBuildings(map);
            }
            else if (!map.IsLoaded() && isSpawned)
            {
                DespawnMap(map);
            }
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

        /// <summary>
        /// A removed building that was loaded or changed from the database is applied to every online player when its map
        /// is loaded. Does nothing until SpawnLoadedMaps has run.
        /// </summary>
        public void ApplyChangedRemovedBuilding(MapRemovedBuilding removal)
        {
            if (_hasSpawnedLoadedMaps && IsOnLoadedMap(removal))
            {
                ApplyRemovedBuilding(removal);
            }
        }

        /// <summary>
        /// Maps marked deleted stay in memory until the next save, but are already gone from the game.
        /// </summary>
        private IEnumerable<Map> GetLiveMaps()
        {
            return _entityManager.GetComponents<Map>().Where(map => !map.IsDeleted());
        }

        private IEnumerable<MapRemovedBuilding> GetLoadedRemovedBuildings()
        {
            return GetMaps().Where(map => map.IsLoaded()).SelectMany(map => map.GetRemovedBuildings());
        }

        private void ApplyMapRemovedBuildings(Map map)
        {
            foreach (MapRemovedBuilding removal in map.GetRemovedBuildings())
            {
                ApplyRemovedBuilding(removal);
            }
        }

        private static bool IsOnLoadedMap(MapObject mapObject)
        {
            Map? map = mapObject.GetMap();
            return !mapObject.IsDeleted() && map != null && !map.IsDeleted() && map.IsLoaded();
        }

        private static bool IsOnLoadedMap(MapRemovedBuilding removal)
        {
            Map? map = removal.GetMap();
            return !removal.IsDeleted() && map != null && !map.IsDeleted() && map.IsLoaded();
        }

        private void SpawnObject(MapObject mapObject)
        {
            DynamicObject dynamicObject = _streamerService.CreateDynamicObject(
                mapObject.GetModelId(), mapObject.GetPosition(), mapObject.GetRotation(),
                mapObject.GetVirtualWorld(), mapObject.GetInterior(), parent: mapObject.Entity);

            foreach (MapObjectMaterial material in mapObject.GetMaterials())
            {
                ApplyMaterial(dynamicObject, material);
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
