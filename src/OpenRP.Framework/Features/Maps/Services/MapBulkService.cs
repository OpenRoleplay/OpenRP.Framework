using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Maps.Components;
using OpenRP.Framework.Features.Maps.Entities;
using OpenRP.Framework.Features.Maps.Enums;
using OpenRP.Framework.Features.Maps.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Maps.Services
{
    public class MapBulkService : IMapBulkService
    {
        private const string MapGoneMessage = "That map no longer exists.";
        private const string DatabaseFailedMessage = "The objects couldn't be saved to the database, so the old objects are unchanged.";
        private const string InGameFailedMessage = "The new objects were saved to the database, but they couldn't all be put in the game. Restart the server to load them. "
            + "The map stays locked until then.";

        private readonly IMapDatabase _mapDatabase;
        private readonly IMapStore _mapStore;
        private readonly IMapService _mapService;
        private readonly IMapEditingService _mapEditingService;
        private readonly IMapBusyTracker _mapBusyTracker;
        private readonly ILogger<MapBulkService> _logger;

        public MapBulkService(IMapDatabase mapDatabase, IMapStore mapStore, IMapService mapService,
            IMapEditingService mapEditingService, IMapBusyTracker mapBusyTracker, ILogger<MapBulkService> logger)
        {
            _mapDatabase = mapDatabase;
            _mapStore = mapStore;
            _mapService = mapService;
            _mapEditingService = mapEditingService;
            _mapBusyTracker = mapBusyTracker;
            _logger = logger;
        }

        public async Task<MapBulkResult> ReplaceObjectsAsync(Map map, IReadOnlyList<MapObjectSpec> objects, IProgress<MapBulkProgress>? progress = null)
        {
            string? refusal = GetRefusal(map, objects);
            if (refusal != null)
            {
                return MapBulkResult.Refuse(refusal);
            }

            _mapBusyTracker.Set(map, MapBusyReason.Regenerating);
            return await ReplaceWhileRegeneratingAsync(map, objects, progress);
        }

        private string? GetRefusal(Map map, IReadOnlyList<MapObjectSpec> objects)
        {
            if (!map.IsComponentAlive)
            {
                return MapGoneMessage;
            }
            MapBusyReason? busyReason = _mapBusyTracker.GetReason(map);
            if (busyReason != null)
            {
                return MapBusyMessages.For(busyReason.Value);
            }
            return MapBulkValidator.GetObjectsError(objects);
        }

        /// <summary>
        /// Replaces the rows, then the objects in the game. The map is busy until this returns. A failure before the rows are
        /// committed unlocks the map, because the old objects are still there. Once they're committed, a failure in the game
        /// locks the map until the server restarts, because the game doesn't show all of its objects.
        /// </summary>
        private async Task<MapBulkResult> ReplaceWhileRegeneratingAsync(Map map, IReadOnlyList<MapObjectSpec> objects, IProgress<MapBulkProgress>? progress)
        {
            ulong mapId = map.GetId();
            Stopwatch databaseTimer = Stopwatch.StartNew();
            MapBulkRowIds rowIds;
            try
            {
                _mapEditingService.StopEditingMap(map);
                rowIds = await WriteDatabaseAsync(mapId, objects, progress);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save the objects of map {MapId}. Nothing was changed.", mapId);
                _mapBusyTracker.Clear(map);
                return MapBulkResult.Failed(DatabaseFailedMessage);
            }

            MapBulkResult result = await ShowReplacedObjectsAsync(map, objects, rowIds, databaseTimer.Elapsed, progress);
            if (result.Error == null)
            {
                _mapBusyTracker.Clear(map);
            }
            else
            {
                _mapBusyTracker.Set(map, MapBusyReason.LockedAfterFailure);
            }
            return result;
        }

        /// <summary>
        /// Writes the rows on a worker thread. The progress is wrapped here, on the main thread, so its reports come back to it.
        /// </summary>
        private Task<MapBulkRowIds> WriteDatabaseAsync(ulong mapId, IReadOnlyList<MapObjectSpec> objects, IProgress<MapBulkProgress>? progress)
        {
            IProgress<MapBulkProgress>? databaseProgress = progress == null ? null : new Progress<MapBulkProgress>(progress.Report);
            return Task.Run(() => _mapDatabase.ReplaceObjectRows(mapId, objects, databaseProgress));
        }

        /// <summary>
        /// Puts the new objects in the game. The rows are already saved, so a failure here is reported as a restart.
        /// </summary>
        private async Task<MapBulkResult> ShowReplacedObjectsAsync(Map map, IReadOnlyList<MapObjectSpec> objects, MapBulkRowIds rowIds,
            TimeSpan databaseTime, IProgress<MapBulkProgress>? progress)
        {
            Stopwatch spawnTimer = Stopwatch.StartNew();
            string? error = await TryShowReplacedObjectsAsync(map, objects, rowIds, progress);
            return new MapBulkResult(error, objects.Count, rowIds.MaterialIds.Count, databaseTime, spawnTimer.Elapsed, DatabaseUnchanged: false, Refused: false);
        }

        private async Task<string?> TryShowReplacedObjectsAsync(Map map, IReadOnlyList<MapObjectSpec> objects, MapBulkRowIds rowIds,
            IProgress<MapBulkProgress>? progress)
        {
            try
            {
                bool wasLoaded = map.IsLoaded();
                await RemoveOldObjectsAsync(map, progress);
                await AddNewObjectsAsync(map, objects, rowIds, progress);
                _mapStore.MarkMapLoadedInMemory(map);
                await _mapService.SpawnMapAsync(map, ToSliceProgress(MapBulkProgress.SpawningObjects, objects.Count, progress));
                if (!wasLoaded)
                {
                    ApplyRemovedBuildings(map);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "The objects of map {MapId} were saved, but the game couldn't be updated.", map.GetId());
                return InGameFailedMessage;
            }
        }

        /// <summary>
        /// A replace loads a map that wasn't in the game, so its removed buildings go too, as when the map is loaded.
        /// A map that was already in the game has them applied; applying them twice would count twice against the
        /// player's limit.
        /// </summary>
        private void ApplyRemovedBuildings(Map map)
        {
            if (!map.IsComponentAlive)
            {
                return;
            }

            foreach (MapRemovedBuilding removal in map.GetRemovedBuildings())
            {
                _mapService.ApplyRemovedBuilding(removal);
            }
        }

        /// <summary>
        /// Despawns the old objects, then forgets them, a slice at a time. Forgetting destroys their entities, so they must be despawned first.
        /// </summary>
        private async Task RemoveOldObjectsAsync(Map map, IProgress<MapBulkProgress>? progress)
        {
            await _mapService.DespawnMapAsync(map);
            int total = _mapStore.GetObjectCount(map);
            while (_mapStore.GetObjectCount(map) > 0)
            {
                _mapStore.ForgetObjects(map, 0, MapSlices.SliceSize);
                progress?.Report(new MapBulkProgress(MapBulkProgress.RemovingOldObjects, total - _mapStore.GetObjectCount(map), total));
                await Task.Delay(1);
            }
        }

        private async Task AddNewObjectsAsync(Map map, IReadOnlyList<MapObjectSpec> objects, MapBulkRowIds rowIds, IProgress<MapBulkProgress>? progress)
        {
            Queue<ulong> materialIds = new Queue<ulong>(rowIds.MaterialIds);
            List<int> objectIndexes = Enumerable.Range(0, objects.Count).ToList();
            await MapSlices.RunAsync(objectIndexes,
                index => AddObjectToMemory(map, objects[index], rowIds.ObjectIds[index], materialIds),
                ToSliceProgress(MapBulkProgress.AddingObjects, objects.Count, progress));
        }

        /// <summary>
        /// Adds the object and its materials to memory, with the ids the database gave them. The material ids are taken in
        /// the order the materials were inserted, which is the order of the objects and their textures.
        /// </summary>
        private MapObject AddObjectToMemory(Map map, MapObjectSpec spec, ulong objectId, Queue<ulong> materialIds)
        {
            List<MapObjectMaterialModel> materials = new List<MapObjectMaterialModel>();
            foreach (MapObjectTextureSpec texture in spec.Textures ?? Array.Empty<MapObjectTextureSpec>())
            {
                materials.Add(CreateMaterialModel(objectId, texture, materialIds.Dequeue()));
            }
            return _mapStore.AddLoadedObject(map, CreateObjectModel(map.GetId(), spec, objectId), materials);
        }

        private static IProgress<int>? ToSliceProgress(string stage, int total, IProgress<MapBulkProgress>? progress)
        {
            return progress == null ? null : new Progress<int>(done => progress.Report(new MapBulkProgress(stage, done, total)));
        }

        private static MapObjectModel CreateObjectModel(ulong mapId, MapObjectSpec spec, ulong objectId)
        {
            return new MapObjectModel
            {
                Id = objectId,
                MapId = mapId,
                ModelId = spec.ModelId,
                X = spec.Position.X,
                Y = spec.Position.Y,
                Z = spec.Position.Z,
                RotX = spec.Rotation.X,
                RotY = spec.Rotation.Y,
                RotZ = spec.Rotation.Z,
                VirtualWorld = spec.VirtualWorld,
                Interior = spec.Interior,
                Description = spec.Description,
                StreamDistance = spec.StreamDistance
            };
        }

        /// <summary>
        /// A texture has no text, so the text fields keep the same neutral values the database got.
        /// </summary>
        private static MapObjectMaterialModel CreateMaterialModel(ulong objectId, MapObjectTextureSpec texture, ulong materialId)
        {
            return new MapObjectMaterialModel
            {
                Id = materialId,
                MapObjectId = objectId,
                MaterialIndex = texture.MaterialIndex,
                Type = MapObjectMaterialType.Texture,
                ModelId = texture.ModelId,
                TxdName = texture.TxdName,
                TextureName = texture.TextureName,
                Color = texture.ArgbColor
            };
        }
    }
}
