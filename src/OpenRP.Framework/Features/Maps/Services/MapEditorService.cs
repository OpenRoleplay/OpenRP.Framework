using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Maps.Components;
using OpenRP.Framework.Features.Maps.Entities;
using OpenRP.Framework.Features.Maps.Enums;
using OpenRP.Framework.Features.Maps.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Maps.Services
{
    public class MapEditorService : IMapEditorService
    {
        private const string MapGoneMessage = "That map no longer exists.";

        private readonly IMapStore _mapStore;
        private readonly IMapService _mapService;
        private readonly IMapEditingService _mapEditingService;
        private readonly IMapBusyTracker _mapBusyTracker;
        private readonly ILogger<MapEditorService> _logger;

        public MapEditorService(IMapStore mapStore, IMapService mapService, IMapEditingService mapEditingService,
            IMapBusyTracker mapBusyTracker, ILogger<MapEditorService> logger)
        {
            _mapStore = mapStore;
            _mapService = mapService;
            _mapEditingService = mapEditingService;
            _mapBusyTracker = mapBusyTracker;
            _logger = logger;
        }

        public string? CreateMap(string name, out Map? map)
        {
            map = null;
            string trimmedName = name.Trim();
            string? nameError = GetMapNameError(trimmedName, null);
            if (nameError != null)
            {
                return nameError;
            }

            (Map? createdMap, string? createError) = TryWrite(() => _mapStore.CreateMap(trimmedName), "map");
            map = createdMap;
            return createError;
        }

        public string? RenameMap(Map map, string name)
        {
            if (GetBusyMessage(map) is string busyMessage)
            {
                return busyMessage;
            }
            string trimmedName = name.Trim();
            string? nameError = GetMapNameError(trimmedName, map);
            if (nameError != null)
            {
                return nameError;
            }

            return TryWrite(() => _mapStore.RenameMap(map, trimmedName), "map");
        }

        public async Task<string?> DeleteMapAsync(Map map)
        {
            if (!map.IsComponentAlive)
            {
                return MapGoneMessage;
            }
            if (GetBusyMessage(map) is string busyMessage)
            {
                return busyMessage;
            }

            _mapBusyTracker.Set(map, MapBusyReason.Deleting);
            try
            {
                return await DeleteMapWhileDeletingAsync(map);
            }
            finally
            {
                _mapBusyTracker.Clear(map);
            }
        }

        private async Task<string?> DeleteMapWhileDeletingAsync(Map map)
        {
            _mapEditingService.StopEditingMap(map);
            bool wasLoaded = map.IsLoaded();
            await _mapService.DespawnMapAsync(map);

            string? error = await TryWriteAsync(() => _mapStore.DeleteMapAsync(map), "map");
            if (error != null && wasLoaded)
            {
                // The map is still there, so its objects go back in the game.
                await _mapService.SpawnMapAsync(map);
            }
            return error;
        }

        public async Task<string?> LoadMap(Map map)
        {
            if (GetBusyMessage(map) is string busyMessage)
            {
                return busyMessage;
            }
            if (map.IsLoaded())
            {
                return $"{map.GetName()} is loaded already.";
            }

            string? error = TryWrite(() => _mapStore.SetMapLoaded(map, true), "map");
            if (error != null)
            {
                return error;
            }

            await _mapService.SpawnMapAsync(map);
            if (!map.IsComponentAlive)
            {
                return MapGoneMessage;
            }
            _mapService.ApplyRemovedBuildings(map);
            return null;
        }

        public async Task<string?> UnloadMap(Map map)
        {
            if (GetBusyMessage(map) is string busyMessage)
            {
                return busyMessage;
            }
            if (!map.IsLoaded())
            {
                return $"{map.GetName()} isn't loaded.";
            }

            string? error = TryWrite(() => _mapStore.SetMapLoaded(map, false), "map");
            if (error != null)
            {
                return error;
            }

            await _mapService.DespawnMapAsync(map);
            return null;
        }

        public string? AddObject(Map map, int modelId, Vector3 position, int virtualWorld, int interior, string? description, out MapObject? mapObject)
        {
            mapObject = null;
            if (GetBusyMessage(map) is string busyMessage)
            {
                return busyMessage;
            }
            string? valuesError = GetObjectValuesError(modelId, virtualWorld, interior, description);
            if (valuesError != null)
            {
                return valuesError;
            }

            (MapObject? createdObject, string? createError) = TryWrite(() => _mapStore.CreateObject(map,
                CreateObjectModel(map.GetId(), modelId, position, Vector3.Zero, virtualWorld, interior, description)), "object");
            mapObject = createdObject;
            if (createdObject != null)
            {
                _mapService.RespawnObject(createdObject);
            }
            return createError;
        }

        public string? DeleteObject(MapObject mapObject)
        {
            if (GetBusyMessage(mapObject.GetMap()) is string busyMessage)
            {
                return busyMessage;
            }
            _mapService.DespawnObject(mapObject);
            try
            {
                _mapStore.DeleteObject(mapObject);
                return null;
            }
            catch (MapRowMissingException ex)
            {
                return ForgetMissingObject(mapObject, ex);
            }
            catch (Exception ex)
            {
                // The object is still there, so it goes back in the game.
                _mapService.RespawnObject(mapObject);
                return GetSaveFailure(ex, "object");
            }
        }

        /// <summary>
        /// The row is gone, so the object can't be saved or shown again. It's forgotten rather than respawned, and the
        /// message says why the delete didn't happen.
        /// </summary>
        private string ForgetMissingObject(MapObject mapObject, MapRowMissingException ex)
        {
            _logger.LogWarning(ex, "Map object {MapObjectId} was already deleted elsewhere. Forgetting it.", mapObject.GetId());
            _mapStore.ForgetObject(mapObject);
            return ex.Message;
        }

        public string? DuplicateObject(MapObject mapObject, out MapObject? copy)
        {
            copy = null;
            if (GetBusyMessage(mapObject.GetMap()) is string busyMessage)
            {
                return busyMessage;
            }
            Map? map = mapObject.GetMap();
            if (map == null)
            {
                return MapGoneMessage;
            }

            (MapObject? createdCopy, string? createError) = TryWrite(() => _mapStore.CreateObject(map, CreateCopyModel(mapObject)), "object");
            copy = createdCopy;
            if (createdCopy != null)
            {
                _mapService.RespawnObject(createdCopy);
            }
            return createError;
        }

        public string? MoveObject(MapObject mapObject, Vector3 position, Vector3 rotation)
        {
            if (GetBusyMessage(mapObject.GetMap()) is string busyMessage)
            {
                return busyMessage;
            }
            string? error = TryWrite(() => _mapStore.MoveObject(mapObject, position, rotation), "object");
            // Whether or not the save worked, the streamer object goes where the object is saved.
            _mapService.MoveSpawnedObject(mapObject);
            return error;
        }

        public string? SetObjectModel(MapObject mapObject, int modelId)
        {
            if (GetBusyMessage(mapObject.GetMap()) is string busyMessage)
            {
                return busyMessage;
            }
            string? modelError = GetModelIdError(modelId);
            if (modelError != null)
            {
                return modelError;
            }

            return WriteAndRespawn(mapObject, () => _mapStore.SetObjectModel(mapObject, modelId), "object");
        }

        public string? SetObjectWorld(MapObject mapObject, int virtualWorld, int interior)
        {
            if (GetBusyMessage(mapObject.GetMap()) is string busyMessage)
            {
                return busyMessage;
            }
            string? worldError = GetWorldError(virtualWorld, interior);
            if (worldError != null)
            {
                return worldError;
            }

            return WriteAndRespawn(mapObject, () => _mapStore.SetObjectWorld(mapObject, virtualWorld, interior), "object");
        }

        public string? SetObjectDescription(MapObject mapObject, string? description)
        {
            if (GetBusyMessage(mapObject.GetMap()) is string busyMessage)
            {
                return busyMessage;
            }
            string? descriptionError = GetDescriptionError(description);
            if (descriptionError != null)
            {
                return descriptionError;
            }

            return TryWrite(() => _mapStore.SetObjectDescription(mapObject, NormalizeDescription(description)), "object");
        }

        public string? SetObjectStreamDistance(MapObject mapObject, float? streamDistance)
        {
            if (GetBusyMessage(mapObject.GetMap()) is string busyMessage)
            {
                return busyMessage;
            }
            string? distanceError = GetStreamDistanceError(streamDistance);
            if (distanceError != null)
            {
                return distanceError;
            }

            return WriteAndRespawn(mapObject, () => _mapStore.SetObjectStreamDistance(mapObject, streamDistance), "object");
        }

        public string? SetTexture(MapObject mapObject, int materialIndex, string input)
        {
            if (GetBusyMessage(mapObject.GetMap()) is string busyMessage)
            {
                return busyMessage;
            }
            (MapObjectMaterialModel? texture, string? parseError) = MapInputParser.ParseTexture(input);
            if (texture == null)
            {
                return parseError;
            }

            texture.MaterialIndex = materialIndex;
            return SetMaterialContent(mapObject, texture);
        }

        public string? SetMaterialText(MapObject mapObject, string input)
        {
            if (GetBusyMessage(mapObject.GetMap()) is string busyMessage)
            {
                return busyMessage;
            }
            (MapObjectMaterialModel? text, string? parseError) = MapInputParser.ParseMaterialText(input);
            if (text == null)
            {
                return parseError;
            }

            return SetMaterialContent(mapObject, text);
        }

        public string? ClearMaterial(MapObject mapObject, int materialIndex)
        {
            if (GetBusyMessage(mapObject.GetMap()) is string busyMessage)
            {
                return busyMessage;
            }
            string? indexError = GetMaterialIndexError(materialIndex);
            if (indexError != null)
            {
                return indexError;
            }

            MapObjectMaterial? material = mapObject.GetMaterial(materialIndex);
            if (material == null)
            {
                return $"Nothing is set at material index {materialIndex}.";
            }

            return WriteAndRespawn(mapObject, () => _mapStore.DeleteMaterial(material), "material");
        }

        public string? AddRemovedBuilding(Map map, string input, Vector3 origin, out MapRemovedBuilding? removal)
        {
            removal = null;
            if (GetBusyMessage(map) is string busyMessage)
            {
                return busyMessage;
            }
            (MapRemovedBuildingModel? removalModel, string? parseError) = MapInputParser.ParseRemovedBuilding(input, origin);
            if (removalModel == null)
            {
                return parseError;
            }

            (MapRemovedBuilding? createdRemoval, string? createError) = TryWrite(() => _mapStore.CreateRemovedBuilding(map, removalModel), "removed building");
            removal = createdRemoval;
            if (createdRemoval != null && map.IsLoaded())
            {
                _mapService.ApplyRemovedBuilding(createdRemoval);
            }
            return createError;
        }

        public string? DeleteRemovedBuilding(MapRemovedBuilding removal)
        {
            if (GetBusyMessage(removal.GetComponentInParent<Map>()) is string busyMessage)
            {
                return busyMessage;
            }
            return TryWrite(() => _mapStore.DeleteRemovedBuilding(removal), "removed building");
        }

        /// <summary>
        /// Writes the content to the slot, replacing what the slot showed, or adds it when the slot is empty.
        /// </summary>
        private string? SetMaterialContent(MapObject mapObject, MapObjectMaterialModel content)
        {
            string? indexError = GetMaterialIndexError(content.MaterialIndex);
            if (indexError != null)
            {
                return indexError;
            }

            return WriteAndRespawn(mapObject, () => _mapStore.SetMaterialContent(mapObject, content), "material");
        }

        /// <summary>
        /// Saves the change, then respawns the object so the game shows it. Nothing is respawned when the save failed.
        /// </summary>
        private string? WriteAndRespawn(MapObject mapObject, Action write, string thing)
        {
            string? error = TryWrite(write, thing);
            if (error == null)
            {
                _mapService.RespawnObject(mapObject);
            }
            return error;
        }

        /// <summary>
        /// Runs the write. A row that's gone or a failed save becomes the reason the change didn't happen.
        /// </summary>
        private string? TryWrite(Action write, string thing)
        {
            try
            {
                write();
                return null;
            }
            catch (MapRowMissingException ex)
            {
                return ex.Message;
            }
            catch (Exception ex)
            {
                return GetSaveFailure(ex, thing);
            }
        }

        private (T? Result, string? Error) TryWrite<T>(Func<T> write, string thing) where T : class
        {
            T? result = null;
            string? error = TryWrite(WriteResult, thing);
            return (result, error);

            void WriteResult()
            {
                result = write();
            }
        }

        private async Task<string?> TryWriteAsync(Func<Task> write, string thing)
        {
            try
            {
                await write();
                return null;
            }
            catch (MapRowMissingException ex)
            {
                return ex.Message;
            }
            catch (Exception ex)
            {
                return GetSaveFailure(ex, thing);
            }
        }

        /// <summary>
        /// Why the map can't be changed right now, or null when it can. A busy map refuses every change to it.
        /// </summary>
        private string? GetBusyMessage(Map? map)
        {
            MapBusyReason? reason = _mapBusyTracker.GetReason(map);
            return reason == null ? null : MapBusyMessages.For(reason.Value);
        }

        private string GetSaveFailure(Exception ex, string thing)
        {
            _logger.LogError(ex, "Failed to save a {Thing}.", thing);
            return $"The {thing} couldn't be saved.";
        }

        private static MapObjectModel CreateObjectModel(ulong mapId, int modelId, Vector3 position, Vector3 rotation,
            int virtualWorld, int interior, string? description)
        {
            return new MapObjectModel
            {
                MapId = mapId,
                ModelId = modelId,
                X = position.X,
                Y = position.Y,
                Z = position.Z,
                RotX = rotation.X,
                RotY = rotation.Y,
                RotZ = rotation.Z,
                VirtualWorld = virtualWorld,
                Interior = interior,
                Description = NormalizeDescription(description)
            };
        }

        /// <summary>
        /// The copy goes where the original is, with the same model, world, description and stream distance, and with copies
        /// of its materials.
        /// </summary>
        private static MapObjectModel CreateCopyModel(MapObject source)
        {
            MapObjectModel copy = CreateObjectModel(source.GetMapId(), source.GetModelId(), source.GetPosition(), source.GetRotation(),
                source.GetVirtualWorld(), source.GetInterior(), source.GetDescription());
            copy.StreamDistance = source.GetStreamDistanceSetting();
            copy.Materials = source.GetMaterials()
                .Select(material => CreateMaterialCopy(material.GetRawMapObjectMaterialModel()))
                .ToList();
            return copy;
        }

        private static MapObjectMaterialModel CreateMaterialCopy(MapObjectMaterialModel source)
        {
            return new MapObjectMaterialModel
            {
                MaterialIndex = source.MaterialIndex,
                Type = source.Type,
                ModelId = source.ModelId,
                TxdName = source.TxdName,
                TextureName = source.TextureName,
                Color = source.Color,
                Text = source.Text,
                MaterialSize = source.MaterialSize,
                FontFace = source.FontFace,
                FontSize = source.FontSize,
                Bold = source.Bold,
                FontColor = source.FontColor,
                BackColor = source.BackColor,
                Alignment = source.Alignment
            };
        }

        private string? GetMapNameError(string trimmedName, Map? renamedMap)
        {
            if (trimmedName.Length == 0)
            {
                return "The map name can't be empty.";
            }
            if (trimmedName.Length > MapLimits.NameMaxLength)
            {
                return $"The map name can be at most {MapLimits.NameMaxLength} characters long.";
            }

            // The map being renamed may keep its own name.
            Map? sameNameMap = _mapStore.FindMapByName(trimmedName);
            if (sameNameMap != null && sameNameMap.GetId() != renamedMap?.GetId())
            {
                return $"A map named {sameNameMap.GetName()} exists already.";
            }
            return null;
        }

        private static string? GetObjectValuesError(int modelId, int virtualWorld, int interior, string? description)
        {
            return GetModelIdError(modelId) ?? GetWorldError(virtualWorld, interior) ?? GetDescriptionError(description);
        }

        private static string? GetModelIdError(int modelId)
        {
            return modelId == 0 ? "The model ID can't be 0. Negative IDs are custom models." : null;
        }

        private static string? GetWorldError(int virtualWorld, int interior)
        {
            if (virtualWorld < MapLimits.EverywhereValue)
            {
                return $"The virtual world can't be below {MapLimits.EverywhereValue}; {MapLimits.EverywhereValue} means every virtual world.";
            }
            if (interior < MapLimits.EverywhereValue)
            {
                return $"The interior can't be below {MapLimits.EverywhereValue}; {MapLimits.EverywhereValue} means every interior.";
            }
            return null;
        }

        private static string? GetDescriptionError(string? description)
        {
            return NormalizeDescription(description)?.Length > MapLimits.DescriptionMaxLength
                ? $"The description can be at most {MapLimits.DescriptionMaxLength} characters long."
                : null;
        }

        /// <summary>
        /// Null means the default distance, which needs no check. The comparisons are written so that NaN is refused too.
        /// </summary>
        private static string? GetStreamDistanceError(float? streamDistance)
        {
            if (streamDistance == null)
            {
                return null;
            }

            float distance = streamDistance.Value;
            return distance >= MapLimits.MinStreamDistance && distance <= MapLimits.MaxStreamDistance
                ? null
                : $"The stream distance must be from {MapLimits.MinStreamDistance} to {MapLimits.MaxStreamDistance} metres.";
        }

        private static string? GetMaterialIndexError(int materialIndex)
        {
            return materialIndex is < 0 or > MapObjectMaterial.MaxMaterialIndex
                ? $"The material index must be from 0 to {MapObjectMaterial.MaxMaterialIndex}."
                : null;
        }

        /// <summary>
        /// Trims the description, and turns an empty one into null.
        /// </summary>
        private static string? NormalizeDescription(string? description)
        {
            return string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        }
    }
}
