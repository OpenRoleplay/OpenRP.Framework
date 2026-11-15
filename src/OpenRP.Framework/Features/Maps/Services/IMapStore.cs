using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Maps.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Maps.Services
{
    /// <summary>
    /// The maps in memory, with their objects, materials and removed buildings, and every change to them. A change is
    /// written to the database first and changes memory only once the write succeeded. Writes whose row is gone throw
    /// <see cref="Entities.MapRowMissingException"/>. Main thread only.
    /// </summary>
    public interface IMapStore
    {
        /// <summary>
        /// Reads every map from the database into memory. Called once at startup.
        /// </summary>
        void LoadAll();

        /// <summary>
        /// Every map, ordered by name.
        /// </summary>
        IReadOnlyList<Map> GetMaps();

        /// <summary>
        /// The map with this id, or null when there is none.
        /// </summary>
        Map? GetMap(ulong mapId);

        /// <summary>
        /// The map with this name, ignoring case, or null when there is none.
        /// </summary>
        Map? FindMapByName(string name);

        /// <summary>
        /// The objects of the map. A live view, so copy it before changing the map.
        /// </summary>
        IReadOnlyCollection<MapObject> GetObjects(Map map);

        int GetObjectCount(Map map);

        /// <summary>
        /// The object with this id, or null when there is none.
        /// </summary>
        MapObject? GetObject(ulong mapObjectId);

        /// <summary>
        /// Creates a map, not loaded.
        /// </summary>
        Map CreateMap(string name);

        void RenameMap(Map map, string name);

        void SetMapLoaded(Map map, bool isLoaded);

        /// <summary>
        /// Deletes the map from the database, then from memory, then destroys its entities in slices. Its objects must
        /// already be despawned.
        /// </summary>
        Task DeleteMapAsync(Map map);

        /// <summary>
        /// Creates the object with the materials in its model.
        /// </summary>
        MapObject CreateObject(Map map, MapObjectModel model);

        void MoveObject(MapObject mapObject, Vector3 position, Vector3 rotation);

        void SetObjectModel(MapObject mapObject, int modelId);

        void SetObjectWorld(MapObject mapObject, int virtualWorld, int interior);

        void SetObjectDescription(MapObject mapObject, string? description);

        /// <summary>
        /// Sets the stream distance in metres, or null for the default. The streamer object keeps its old distance until
        /// it's respawned.
        /// </summary>
        void SetObjectStreamDistance(MapObject mapObject, float? streamDistance);

        /// <summary>
        /// Deletes the object and its materials. Its streamer object must already be despawned.
        /// </summary>
        void DeleteObject(MapObject mapObject);

        /// <summary>
        /// Forgets the object in memory and destroys its entity, for an object whose row is already gone. Its streamer
        /// object must already be despawned.
        /// </summary>
        void ForgetObject(MapObject mapObject);

        /// <summary>
        /// Replaces what the material slot shows, or adds a material when the slot is empty.
        /// </summary>
        void SetMaterialContent(MapObject mapObject, MapObjectMaterialModel content);

        void DeleteMaterial(MapObjectMaterial material);

        MapRemovedBuilding CreateRemovedBuilding(Map map, MapRemovedBuildingModel model);

        void DeleteRemovedBuilding(MapRemovedBuilding removal);

        /// <summary>
        /// Bulk replace: adds an object whose rows are already in the database, with its materials, to memory only.
        /// </summary>
        MapObject AddLoadedObject(Map map, MapObjectModel model, IEnumerable<MapObjectMaterialModel> materials);

        /// <summary>
        /// Bulk replace: forgets up to count objects, starting at start in the map's order, and destroys their entities.
        /// Memory only: the rows must already be gone, and the streamer objects despawned.
        /// </summary>
        void ForgetObjects(Map map, int start, int count);

        /// <summary>
        /// Bulk replace: sets the map's loaded flag in memory only. The bulk write has already set it in the database.
        /// </summary>
        void MarkMapLoadedInMemory(Map map);
    }
}
