using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Maps.Components;
using OpenRP.Framework.Features.Maps.Entities;
using OpenRP.Framework.Features.Maps.Helpers;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Maps.Services
{
    public class MapStore : IMapStore
    {
        private readonly IMapDatabase _database;
        private readonly IEntityManager _entityManager;

        private readonly Dictionary<ulong, Map> _mapsById = new Dictionary<ulong, Map>();
        private readonly Dictionary<ulong, MapObject> _objectsById = new Dictionary<ulong, MapObject>();
        private readonly Dictionary<ulong, Dictionary<ulong, MapObject>> _objectsByMapId = new Dictionary<ulong, Dictionary<ulong, MapObject>>();

        public MapStore(IMapDatabase database, IEntityManager entityManager)
        {
            _database = database;
            _entityManager = entityManager;
        }

        public void LoadAll()
        {
            MapRows rows = _database.LoadAllRows();
            foreach (MapModel map in rows.Maps)
            {
                CreateMapComponent(map);
            }

            ILookup<ulong, MapObjectMaterialModel> materialsByObjectId = rows.Materials.ToLookup(material => material.MapObjectId);
            foreach (MapObjectModel mapObject in rows.Objects)
            {
                CreateObjectComponent(_mapsById[mapObject.MapId], mapObject, materialsByObjectId[mapObject.Id]);
            }

            foreach (MapRemovedBuildingModel removal in rows.RemovedBuildings)
            {
                CreateRemovedBuildingComponent(_mapsById[removal.MapId], removal);
            }
        }

        public IReadOnlyList<Map> GetMaps()
        {
            return _mapsById.Values.OrderBy(map => map.GetName(), StringComparer.OrdinalIgnoreCase).ToList();
        }

        public Map? GetMap(ulong mapId)
        {
            return _mapsById.GetValueOrDefault(mapId);
        }

        public Map? FindMapByName(string name)
        {
            return _mapsById.Values.FirstOrDefault(map => string.Equals(map.GetName(), name, StringComparison.OrdinalIgnoreCase));
        }

        public IReadOnlyCollection<MapObject> GetObjects(Map map)
        {
            return _objectsByMapId.TryGetValue(map.GetId(), out Dictionary<ulong, MapObject>? objects)
                ? objects.Values
                : Array.Empty<MapObject>();
        }

        public int GetObjectCount(Map map)
        {
            return GetObjects(map).Count;
        }

        public MapObject? GetObject(ulong mapObjectId)
        {
            return _objectsById.GetValueOrDefault(mapObjectId);
        }

        public Map CreateMap(string name)
        {
            return CreateMapComponent(_database.InsertMap(name));
        }

        public void RenameMap(Map map, string name)
        {
            RequireRow(_database.SaveMapName(map.GetId(), name), "map");
            map.SetName(name);
        }

        public void SetMapLoaded(Map map, bool isLoaded)
        {
            RequireRow(_database.SaveMapIsLoaded(map.GetId(), isLoaded), "map");
            map.SetIsLoaded(isLoaded);
        }

        public async Task DeleteMapAsync(Map map)
        {
            await _database.DeleteMapAsync(map.GetId());
            List<MapObject> mapObjects = ForgetMap(map);
            await MapSlices.RunAsync(mapObjects, mapObject => mapObject.DestroyEntity());
            map.DestroyEntity();
        }

        public MapObject CreateObject(Map map, MapObjectModel model)
        {
            model.MapId = map.GetId();
            MapObjectModel savedModel = _database.InsertObject(model);
            return CreateObjectComponent(map, savedModel, savedModel.Materials);
        }

        public void MoveObject(MapObject mapObject, Vector3 position, Vector3 rotation)
        {
            RequireRow(_database.SaveObjectTransform(mapObject.GetId(), position, rotation), "object");
            mapObject.SetPosition(position);
            mapObject.SetRotation(rotation);
        }

        public void SetObjectModel(MapObject mapObject, int modelId)
        {
            RequireRow(_database.SaveObjectModel(mapObject.GetId(), modelId), "object");
            mapObject.SetModelId(modelId);
        }

        public void SetObjectWorld(MapObject mapObject, int virtualWorld, int interior)
        {
            RequireRow(_database.SaveObjectWorld(mapObject.GetId(), virtualWorld, interior), "object");
            mapObject.SetVirtualWorld(virtualWorld);
            mapObject.SetInterior(interior);
        }

        public void SetObjectDescription(MapObject mapObject, string? description)
        {
            RequireRow(_database.SaveObjectDescription(mapObject.GetId(), description), "object");
            mapObject.SetDescription(description);
        }

        public void SetObjectStreamDistance(MapObject mapObject, float? streamDistance)
        {
            RequireRow(_database.SaveObjectStreamDistance(mapObject.GetId(), streamDistance), "object");
            mapObject.SetStreamDistanceSetting(streamDistance);
        }

        public void DeleteObject(MapObject mapObject)
        {
            RequireRow(_database.DeleteObject(mapObject.GetId()), "object");
            ForgetObject(mapObject);
        }

        /// <summary>
        /// Takes the object out of memory and destroys its entity. Used when its row is gone, or was just deleted.
        /// Its streamer object must already be despawned.
        /// </summary>
        public void ForgetObject(MapObject mapObject)
        {
            _objectsById.Remove(mapObject.GetId());
            if (_objectsByMapId.TryGetValue(mapObject.GetMapId(), out Dictionary<ulong, MapObject>? objects))
            {
                objects.Remove(mapObject.GetId());
            }
            mapObject.DestroyEntity();
        }

        public void SetMaterialContent(MapObject mapObject, MapObjectMaterialModel content)
        {
            MapObjectMaterial? material = mapObject.GetMaterial(content.MaterialIndex);
            if (material == null)
            {
                content.MapObjectId = mapObject.GetId();
                CreateMaterialComponent(mapObject, _database.InsertMaterial(content));
                return;
            }

            RequireRow(_database.SaveMaterialContent(material.GetId(), content), "material");
            material.CopyContentFrom(content);
        }

        public void DeleteMaterial(MapObjectMaterial material)
        {
            RequireRow(_database.DeleteMaterial(material.GetId()), "material");
            material.DestroyEntity();
        }

        public MapRemovedBuilding CreateRemovedBuilding(Map map, MapRemovedBuildingModel model)
        {
            model.MapId = map.GetId();
            return CreateRemovedBuildingComponent(map, _database.InsertRemovedBuilding(model));
        }

        public void DeleteRemovedBuilding(MapRemovedBuilding removal)
        {
            RequireRow(_database.DeleteRemovedBuilding(removal.GetId()), "removed building");
            removal.DestroyEntity();
        }

        public MapObject AddLoadedObject(Map map, MapObjectModel model, IEnumerable<MapObjectMaterialModel> materials)
        {
            return CreateObjectComponent(map, model, materials);
        }

        public void ForgetObjects(Map map, int start, int count)
        {
            List<MapObject> forgotten = GetObjects(map).Skip(start).Take(count).ToList();
            foreach (MapObject mapObject in forgotten)
            {
                ForgetObject(mapObject);
            }
        }

        public void MarkMapLoadedInMemory(Map map)
        {
            map.SetIsLoaded(true);
        }

        private Map CreateMapComponent(MapModel model)
        {
            Map map = _entityManager.AddComponent<Map>(EntityId.NewEntityId(), new object[] { model });
            _mapsById[model.Id] = map;
            _objectsByMapId[model.Id] = new Dictionary<ulong, MapObject>();
            return map;
        }

        /// <summary>
        /// The object is a root entity, not a child of its map, so a map with hundreds of thousands of objects has no huge parent.
        /// </summary>
        private MapObject CreateObjectComponent(Map map, MapObjectModel model, IEnumerable<MapObjectMaterialModel> materials)
        {
            MapObject mapObject = _entityManager.AddComponent<MapObject>(EntityId.NewEntityId(), new object[] { model, map.Entity });
            _objectsById[model.Id] = mapObject;
            _objectsByMapId[map.GetId()][model.Id] = mapObject;
            foreach (MapObjectMaterialModel material in materials)
            {
                CreateMaterialComponent(mapObject, material);
            }
            return mapObject;
        }

        private MapObjectMaterial CreateMaterialComponent(MapObject mapObject, MapObjectMaterialModel model)
        {
            return _entityManager.AddComponent<MapObjectMaterial>(EntityId.NewEntityId(), mapObject.Entity, new object[] { model });
        }

        private MapRemovedBuilding CreateRemovedBuildingComponent(Map map, MapRemovedBuildingModel model)
        {
            return _entityManager.AddComponent<MapRemovedBuilding>(EntityId.NewEntityId(), map.Entity, new object[] { model });
        }

        /// <summary>
        /// Takes the map and its objects out of memory. Their entities are left for the caller to destroy.
        /// </summary>
        private List<MapObject> ForgetMap(Map map)
        {
            List<MapObject> mapObjects = GetObjects(map).ToList();
            foreach (MapObject mapObject in mapObjects)
            {
                _objectsById.Remove(mapObject.GetId());
            }
            _objectsByMapId.Remove(map.GetId());
            _mapsById.Remove(map.GetId());
            return mapObjects;
        }

        private static void RequireRow(bool wasSaved, string thing)
        {
            if (!wasSaved)
            {
                throw new MapRowMissingException(thing);
            }
        }
    }
}
