using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Maps.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Maps.Services
{
    /// <summary>
    /// The database rows of maps. Every call uses its own short-lived database context, and none of them touch the
    /// game, so they can run on any thread. Single-row changes return whether a row was affected; false means the row is
    /// gone, for example because another server deleted it.
    /// </summary>
    public interface IMapDatabase
    {
        /// <summary>
        /// Every map, object, material and removed building. Read once at startup.
        /// </summary>
        MapRows LoadAllRows();

        MapModel InsertMap(string name);

        bool SaveMapName(ulong mapId, string name);

        bool SaveMapIsLoaded(ulong mapId, bool isLoaded);

        /// <summary>
        /// Deletes the map with its materials, objects and removed buildings, in one transaction.
        /// </summary>
        Task DeleteMapAsync(ulong mapId);

        /// <summary>
        /// Bulk replace: in one transaction, deletes the map's materials and objects, inserts the given objects with their
        /// textures, and sets the map loaded. Returns the new ids. Throws, with nothing changed, when a step fails. Runs on a
        /// worker thread, so it never touches the game.
        /// </summary>
        MapBulkRowIds ReplaceObjectRows(ulong mapId, IReadOnlyList<MapObjectSpec> specs, IProgress<MapBulkProgress>? progress);

        /// <summary>
        /// Inserts the object together with its materials, in one save.
        /// </summary>
        MapObjectModel InsertObject(MapObjectModel model);

        bool SaveObjectTransform(ulong mapObjectId, Vector3 position, Vector3 rotation);

        bool SaveObjectModel(ulong mapObjectId, int modelId);

        bool SaveObjectWorld(ulong mapObjectId, int virtualWorld, int interior);

        bool SaveObjectDescription(ulong mapObjectId, string? description);

        bool SaveObjectStreamDistance(ulong mapObjectId, float? streamDistance);

        /// <summary>
        /// Deletes the object. Its materials go with it, through the database's cascade.
        /// </summary>
        bool DeleteObject(ulong mapObjectId);

        MapObjectMaterialModel InsertMaterial(MapObjectMaterialModel material);

        bool SaveMaterialContent(ulong materialId, MapObjectMaterialModel content);

        bool DeleteMaterial(ulong materialId);

        MapRemovedBuildingModel InsertRemovedBuilding(MapRemovedBuildingModel removal);

        bool DeleteRemovedBuilding(ulong removalId);
    }
}
