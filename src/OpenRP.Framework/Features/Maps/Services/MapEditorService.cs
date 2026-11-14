using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Maps.Components;
using OpenRP.Framework.Features.Maps.Entities;
using OpenRP.Framework.Features.Maps.Helpers;
using System;

namespace OpenRP.Framework.Features.Maps.Services
{
    public class MapEditorService : IMapEditorService
    {
        private const string JustDeletedNameMessage = "A map with that name was just deleted. Wait a minute for it to be saved, then try again.";

        private readonly IMapService _mapService;
        private readonly IMapEditingService _mapEditingService;
        private readonly IMapManager _mapManager;
        private readonly IMapObjectManager _mapObjectManager;
        private readonly IMapObjectMaterialManager _mapObjectMaterialManager;
        private readonly IMapRemovedBuildingManager _mapRemovedBuildingManager;
        private readonly ILogger<MapEditorService> _logger;

        public MapEditorService(IMapService mapService, IMapEditingService mapEditingService, IMapManager mapManager, IMapObjectManager mapObjectManager,
            IMapObjectMaterialManager mapObjectMaterialManager, IMapRemovedBuildingManager mapRemovedBuildingManager, ILogger<MapEditorService> logger)
        {
            _mapService = mapService;
            _mapEditingService = mapEditingService;
            _mapManager = mapManager;
            _mapObjectManager = mapObjectManager;
            _mapObjectMaterialManager = mapObjectMaterialManager;
            _mapRemovedBuildingManager = mapRemovedBuildingManager;
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

            (Map? createdMap, string? createError) = TryCreate(() => _mapManager.CreateAndLoad(new MapModel { Name = trimmedName }), "map");
            map = createdMap;
            return createError;
        }

        public string? RenameMap(Map map, string name)
        {
            string trimmedName = name.Trim();
            string? nameError = GetMapNameError(trimmedName, map);
            if (nameError != null)
            {
                return nameError;
            }

            map.SetName(trimmedName);
            return null;
        }

        public string? DeleteMap(Map map)
        {
            _mapService.DespawnMap(map);
            _mapEditingService.StopEditingMap(map);
            foreach (MapObject mapObject in map.GetObjects())
            {
                MarkObjectDeleted(mapObject);
            }
            foreach (MapRemovedBuilding removal in map.GetRemovedBuildings())
            {
                removal.ProcessDeletion();
            }
            map.ProcessDeletion();
            return null;
        }

        public string? LoadMap(Map map)
        {
            if (map.IsLoaded())
            {
                return $"{map.GetName()} is loaded already.";
            }

            map.SetIsLoaded(true);
            _mapService.SpawnMap(map);
            foreach (MapRemovedBuilding removal in map.GetRemovedBuildings())
            {
                _mapService.ApplyRemovedBuilding(removal);
            }
            return null;
        }

        public string? UnloadMap(Map map)
        {
            if (!map.IsLoaded())
            {
                return $"{map.GetName()} isn't loaded.";
            }

            map.SetIsLoaded(false);
            _mapService.DespawnMap(map);
            return null;
        }

        public string? AddObject(Map map, int modelId, Vector3 position, int virtualWorld, int interior, string? description, out MapObject? mapObject)
        {
            mapObject = null;
            string? valuesError = GetObjectValuesError(modelId, virtualWorld, interior, description);
            if (valuesError != null)
            {
                return valuesError;
            }

            // Creating the object spawns it when its map is loaded, see OnComponentLoaded.
            (MapObject? createdObject, string? createError) = TryCreate(() => _mapObjectManager.CreateAndLoad(
                CreateObjectModel(map.GetId(), modelId, position, Vector3.Zero, virtualWorld, interior, description)), "object");
            mapObject = createdObject;
            return createdObject == null ? createError : null;
        }

        public string? DeleteObject(MapObject mapObject)
        {
            _mapService.DespawnObject(mapObject);
            MarkObjectDeleted(mapObject);
            return null;
        }

        public string? DuplicateObject(MapObject mapObject, out MapObject? copy)
        {
            (MapObject? createdCopy, string? createError) = TryCreate(() => CreateObjectCopy(mapObject), "object");
            copy = createdCopy;
            if (createdCopy == null)
            {
                return createError;
            }
            return CopyMaterials(mapObject, createdCopy);
        }

        public string? MoveObject(MapObject mapObject, Vector3 position, Vector3 rotation)
        {
            mapObject.SetPosition(position);
            mapObject.SetRotation(rotation);
            _mapService.MoveSpawnedObject(mapObject);
            return null;
        }

        public string? SetObjectModel(MapObject mapObject, int modelId)
        {
            string? modelError = GetModelIdError(modelId);
            if (modelError != null)
            {
                return modelError;
            }

            mapObject.SetModelId(modelId);
            _mapService.RespawnObject(mapObject);
            return null;
        }

        public string? SetObjectWorld(MapObject mapObject, int virtualWorld, int interior)
        {
            string? worldError = GetWorldError(virtualWorld, interior);
            if (worldError != null)
            {
                return worldError;
            }

            mapObject.SetVirtualWorld(virtualWorld);
            mapObject.SetInterior(interior);
            _mapService.RespawnObject(mapObject);
            return null;
        }

        public string? SetObjectDescription(MapObject mapObject, string? description)
        {
            string? descriptionError = GetDescriptionError(description);
            if (descriptionError != null)
            {
                return descriptionError;
            }

            mapObject.SetDescription(NormalizeDescription(description));
            return null;
        }

        public string? SetTexture(MapObject mapObject, int materialIndex, string input)
        {
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
            (MapObjectMaterialModel? text, string? parseError) = MapInputParser.ParseMaterialText(input);
            if (text == null)
            {
                return parseError;
            }

            return SetMaterialContent(mapObject, text);
        }

        public string? ClearMaterial(MapObject mapObject, int materialIndex)
        {
            string? indexError = GetMaterialIndexError(materialIndex);
            if (indexError != null)
            {
                return indexError;
            }

            MapObjectMaterial? material = mapObject.GetMaterialRow(materialIndex);
            if (material == null || material.IsDeleted())
            {
                return $"Nothing is set at material index {materialIndex}.";
            }

            material.ProcessDeletion();
            _mapService.RespawnObject(mapObject);
            return null;
        }

        public string? AddRemovedBuilding(Map map, string input, Vector3 origin, out MapRemovedBuilding? removal)
        {
            removal = null;
            (MapRemovedBuildingModel? removalModel, string? parseError) = MapInputParser.ParseRemovedBuilding(input, origin);
            if (removalModel == null)
            {
                return parseError;
            }

            removalModel.MapId = map.GetId();
            // Applying the removal to the players when its map is loaded happens in OnComponentLoaded, see MapRemovedBuildingManager.
            (MapRemovedBuilding? createdRemoval, string? createError) = TryCreate(() => _mapRemovedBuildingManager.CreateAndLoad(removalModel), "removed building");
            removal = createdRemoval;
            if (createdRemoval == null)
            {
                return createError;
            }
            return null;
        }

        public string? DeleteRemovedBuilding(MapRemovedBuilding removal)
        {
            removal.ProcessDeletion();
            return null;
        }

        /// <summary>
        /// Writes the content to the slot. The slot's own row is reused when there is one, even a deleted one, because
        /// a second row for the same slot would break the database's unique index.
        /// </summary>
        private string? SetMaterialContent(MapObject mapObject, MapObjectMaterialModel content)
        {
            string? indexError = GetMaterialIndexError(content.MaterialIndex);
            if (indexError != null)
            {
                return indexError;
            }

            MapObjectMaterial? existingMaterial = mapObject.GetMaterialRow(content.MaterialIndex);
            if (existingMaterial == null)
            {
                // Creating the material's row respawns the object, see OnComponentLoaded.
                return TryCreateMaterial(mapObject, content, out string? createError) ? null : createError;
            }

            existingMaterial.ProcessDeletion(false);
            existingMaterial.CopyContentFrom(content);
            _mapService.RespawnObject(mapObject);
            return null;
        }

        private bool TryCreateMaterial(MapObject mapObject, MapObjectMaterialModel content, out string? error)
        {
            content.MapObjectId = mapObject.GetId();
            (MapObjectMaterial? material, error) = TryCreate(() => _mapObjectMaterialManager.CreateAndLoad(content), "material");
            return material != null;
        }

        /// <summary>
        /// The copy goes where the original is, with the same model, world and description. Its materials are copied separately.
        /// </summary>
        private MapObject CreateObjectCopy(MapObject source)
        {
            // Creating the copy spawns it when its map is loaded, see OnComponentLoaded.
            return _mapObjectManager.CreateAndLoad(CreateObjectModel(source.GetMapId(), source.GetModelId(), source.GetPosition(),
                source.GetRotation(), source.GetVirtualWorld(), source.GetInterior(), source.GetDescription()));
        }

        /// <summary>
        /// Copies the original's materials onto the copy, so the copy looks the same before any change. The copy row is
        /// already saved, so when a material fails, the error says the copy exists without all of its materials.
        /// </summary>
        private string? CopyMaterials(MapObject source, MapObject copy)
        {
            foreach (MapObjectMaterial material in source.GetMaterials())
            {
                MapObjectMaterialModel materialCopy = CreateMaterialCopy(material.GetRawMapObjectMaterialModel(), copy.GetId());
                (MapObjectMaterial? createdMaterial, string? createError) = TryCreate(() => _mapObjectMaterialManager.CreateAndLoad(materialCopy), "material");
                if (createdMaterial == null)
                {
                    return $"The copy was made, but without all of its materials, so set the missing ones on it. {createError}";
                }
            }
            return null;
        }

        /// <summary>
        /// Marks the object and its materials deleted. The rows go at the next save.
        /// </summary>
        private static void MarkObjectDeleted(MapObject mapObject)
        {
            foreach (MapObjectMaterial material in mapObject.GetMaterials())
            {
                material.ProcessDeletion();
            }
            mapObject.ProcessDeletion();
        }

        private (T? Component, string? Error) TryCreate<T>(Func<T> create, string thing) where T : class
        {
            try
            {
                return (create(), null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save a {Thing}.", thing);
                return (null, $"The {thing} couldn't be saved. Another server may have changed the same data at the same time; try again in a minute.");
            }
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

        private static MapObjectMaterialModel CreateMaterialCopy(MapObjectMaterialModel source, ulong mapObjectId)
        {
            return new MapObjectMaterialModel
            {
                MapObjectId = mapObjectId,
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
            Map? sameNameMap = _mapService.FindMapByName(trimmedName);
            if (sameNameMap != null && sameNameMap.GetId() != renamedMap?.GetId())
            {
                return $"A map named {sameNameMap.GetName()} exists already.";
            }

            // A deleted map keeps its name in the database until the next save, so the name can't be reused before then.
            return _mapService.FindDeletedMapByName(trimmedName) == null ? null : JustDeletedNameMessage;
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
