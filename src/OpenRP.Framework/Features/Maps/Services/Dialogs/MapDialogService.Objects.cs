using OpenRP.Framework.Features.Maps.Components;
using OpenRP.Framework.Features.Maps.Entities;
using OpenRP.Framework.Features.Maps.Helpers;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.OpenMp.Streamer;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Maps.Services.Dialogs
{
    public partial class MapDialogService
    {
        private static readonly string DescriptionPrompt = $"Enter the description, up to {MapLimits.DescriptionMaxLength} characters. Leave it empty to clear it.";
        private const string ModelNotNumberMessage = "Enter a whole model ID, not 0.";
        private const string WorldInputPrompt = "Enter the virtual world and the interior, separated by a space, for example 0 0. Use -1 -1 for everywhere.";
        private const string WorldInputNotNumbersMessage = "Enter two whole numbers: the virtual world, then the interior.";
        private const string NotLoadedMessage = "This object isn't in the game, so it can't be moved. Load its map first.";
        private const string NotStreamedInMessage = "That object isn't streamed in near you yet. Move closer and try again.";

        public void OnObjectClicked(Player player, MapObject? mapObject)
        {
            SelectAndOpenObject(player, mapObject, () => ReopenEditor(player));
        }

        public void OnObjectEdited(Player player, MapObject mapObject, EditObjectResponse response, Vector3 position, Vector3 rotation)
        {
            if (response == EditObjectResponse.Final)
            {
                SaveMovedObject(player, mapObject, position, rotation);
            }
            else if (response == EditObjectResponse.Cancel)
            {
                OpenObjectMenu(player, mapObject);
            }
        }

        private void SelectAndOpenObject(Player player, MapObject? mapObject, Action onBack)
        {
            // A click can hit a streamer object that isn't a map object, so there's nothing to check then.
            if (mapObject != null && !EnsureObjectExists(player, mapObject))
            {
                return;
            }

            string? error = _mapEditingService.SelectObject(player, mapObject);
            ContinueAfter(player, error, () => OpenObjectMenu(player, mapObject!), onBack);
        }

        private void OpenObjectMenu(Player player, MapObject mapObject)
        {
            if (!EnsureObjectExists(player, mapObject))
            {
                return;
            }

            Map? map = mapObject.GetMap();
            if (map == null)
            {
                return;
            }

            BetterListDialog objectDialog = new BetterListDialog("Choose", "Back");
            objectDialog.SetTitle(TitleType.Children, MapsTitle, MapDialogText.GetObjectTitle(mapObject));
            Dictionary<int, Action> actions = new Dictionary<int, Action>();
            actions[objectDialog.AddRow("Move / rotate")] = () => StartMovingObject(player, mapObject);
            actions[objectDialog.AddRow($"Model: {mapObject.GetModelId()}")] = () => OpenModelInput(player, mapObject);
            actions[objectDialog.AddRow("Duplicate")] = () => DuplicateObject(player, mapObject);
            actions[objectDialog.AddRow("Textures")] = () => OpenTextureList(player, mapObject);
            actions[objectDialog.AddRow("Material text")] = () => OpenMaterialTextInput(player, mapObject);
            actions[objectDialog.AddRow(MapDialogText.GetWorldLabel(mapObject))] = () => OpenWorldChoices(player, mapObject);
            actions[objectDialog.AddRow($"Description: {MapDialogText.GetDescriptionOrDash(mapObject)}")] = () => OpenDescriptionInput(player, mapObject);
            actions[objectDialog.AddRow("Delete")] = () => OpenDeleteObject(player, mapObject, map);
            actions[objectDialog.AddRow("Info")] = () => ShowObjectInfo(player, mapObject);
            ShowActionList(player, objectDialog, actions, () => OpenEditorMenu(player, map));
        }

        /// <summary>
        /// Starts the streamer's edit mode on the object. The edit ends in OnObjectEdited, which opens the object's menu again.
        /// </summary>
        private void StartMovingObject(Player player, MapObject mapObject)
        {
            if (!EnsureObjectExists(player, mapObject))
            {
                return;
            }

            string? error = TryStartMovingObject(player, mapObject);
            if (error != null)
            {
                ShowMessage(player, "Move / rotate", error, () => OpenObjectMenu(player, mapObject));
            }
        }

        /// <summary>
        /// The streamer only edits objects it has streamed in for the player, so the nearby objects are streamed in first.
        /// Returns the reason the edit can't start, or null when it has started.
        /// </summary>
        private string? TryStartMovingObject(Player player, MapObject mapObject)
        {
            DynamicObject? dynamicObject = _mapService.GetDynamicObject(mapObject);
            if (dynamicObject == null)
            {
                return NotLoadedMessage;
            }

            _streamerService.Update(player, StreamerType.Object);
            return dynamicObject.Edit(player) ? null : NotStreamedInMessage;
        }

        private void SaveMovedObject(Player player, MapObject mapObject, Vector3 position, Vector3 rotation)
        {
            string? error = _mapEditorService.MoveObject(mapObject, position, rotation);
            ContinueAfter(player, error, () => OpenObjectMenu(player, mapObject), () => OpenObjectMenu(player, mapObject));
        }

        /// <summary>
        /// The copy goes where the original is, so it's selected and put in edit mode to move it off the original.
        /// </summary>
        private void DuplicateObject(Player player, MapObject mapObject)
        {
            if (!EnsureObjectExists(player, mapObject))
            {
                return;
            }

            string? error = _mapEditorService.DuplicateObject(mapObject, out MapObject? copy);
            ContinueAfter(player, error, () => StartMovingCopy(player, copy!), () => OpenObjectMenu(player, mapObject));
        }

        private void StartMovingCopy(Player player, MapObject copy)
        {
            string? selectError = _mapEditingService.SelectObject(player, copy);
            ContinueAfter(player, selectError, () => StartMovingObject(player, copy), () => OpenObjectMenu(player, copy));
        }

        private void OpenModelInput(Player player, MapObject mapObject)
        {
            if (!EnsureObjectExists(player, mapObject))
            {
                return;
            }

            OpenTextInput(player, "Model", $"Enter the new model ID of object #{mapObject.GetId()}:",
                text => SetModelFromInput(player, mapObject, text), () => OpenObjectMenu(player, mapObject));
        }

        private void SetModelFromInput(Player player, MapObject mapObject, string text)
        {
            if (!MapDialogInput.TryParseModelId(text, out int modelId))
            {
                ShowMessage(player, "Model", ModelNotNumberMessage, () => OpenModelInput(player, mapObject));
                return;
            }

            string? error = _mapEditorService.SetObjectModel(mapObject, modelId);
            ContinueAfter(player, error, () => OpenObjectMenu(player, mapObject), () => OpenModelInput(player, mapObject));
        }

        private void OpenDescriptionInput(Player player, MapObject mapObject)
        {
            if (!EnsureObjectExists(player, mapObject))
            {
                return;
            }

            OpenTextInput(player, "Description", DescriptionPrompt,
                text => SetDescriptionFromInput(player, mapObject, text), () => OpenObjectMenu(player, mapObject));
        }

        private void SetDescriptionFromInput(Player player, MapObject mapObject, string text)
        {
            string? error = _mapEditorService.SetObjectDescription(mapObject, text);
            ContinueAfter(player, error, () => OpenObjectMenu(player, mapObject), () => OpenDescriptionInput(player, mapObject));
        }

        private void OpenWorldChoices(Player player, MapObject mapObject)
        {
            if (!EnsureObjectExists(player, mapObject))
            {
                return;
            }

            BetterListDialog worldDialog = new BetterListDialog("Choose", "Back");
            worldDialog.SetTitle(TitleType.Children, MapsTitle, MapDialogText.GetObjectTitle(mapObject), "World and interior");
            Dictionary<int, Action> actions = new Dictionary<int, Action>();
            actions[worldDialog.AddRow("Use my world and interior (w/i)")] = () => SetWorld(player, mapObject, player.VirtualWorld, player.Interior,
                () => OpenWorldChoices(player, mapObject));
            actions[worldDialog.AddRow($"Everywhere ({MapLimits.EverywhereValue}/{MapLimits.EverywhereValue})")] = () => SetWorld(player, mapObject,
                MapLimits.EverywhereValue, MapLimits.EverywhereValue, () => OpenWorldChoices(player, mapObject));
            actions[worldDialog.AddRow("Enter manually")] = () => OpenWorldInput(player, mapObject);
            ShowActionList(player, worldDialog, actions, () => OpenObjectMenu(player, mapObject));
        }

        private void OpenWorldInput(Player player, MapObject mapObject)
        {
            if (!EnsureObjectExists(player, mapObject))
            {
                return;
            }

            OpenTextInput(player, "World and interior", WorldInputPrompt,
                text => SetWorldFromInput(player, mapObject, text), () => OpenWorldChoices(player, mapObject));
        }

        private void SetWorldFromInput(Player player, MapObject mapObject, string text)
        {
            if (!MapDialogInput.TryParseWorldAndInterior(text, out int virtualWorld, out int interior))
            {
                ShowMessage(player, "World and interior", WorldInputNotNumbersMessage, () => OpenWorldInput(player, mapObject));
                return;
            }
            SetWorld(player, mapObject, virtualWorld, interior, () => OpenWorldInput(player, mapObject));
        }

        private void SetWorld(Player player, MapObject mapObject, int virtualWorld, int interior, Action onAfterError)
        {
            if (!EnsureObjectExists(player, mapObject))
            {
                return;
            }

            string? error = _mapEditorService.SetObjectWorld(mapObject, virtualWorld, interior);
            ContinueAfter(player, error, () => OpenObjectMenu(player, mapObject), onAfterError);
        }

        private void OpenDeleteObject(Player player, MapObject mapObject, Map map)
        {
            if (!EnsureObjectExists(player, mapObject))
            {
                return;
            }

            ShowConfirmation(player, "Delete object", $"Delete object #{mapObject.GetId()} ({mapObject.GetModelId()})? This can't be undone.",
                () => DeleteObject(player, mapObject, map), () => OpenObjectMenu(player, mapObject));
        }

        private void DeleteObject(Player player, MapObject mapObject, Map map)
        {
            string? error = _mapEditorService.DeleteObject(mapObject);
            _mapEditingService.ForgetObject(player, mapObject);
            ContinueAfter(player, error, () => OpenEditorMenu(player, map), () => OpenEditorMenu(player, map));
        }

        private void ShowObjectInfo(Player player, MapObject mapObject)
        {
            if (!EnsureObjectExists(player, mapObject))
            {
                return;
            }

            ShowMessage(player, "Info", BuildObjectInfo(mapObject), () => OpenObjectMenu(player, mapObject));
        }

        private static string BuildObjectInfo(MapObject mapObject)
        {
            List<string> lines = new List<string>
            {
                $"Id: {mapObject.GetId()}",
                $"Model: {mapObject.GetModelId()}",
                $"Position: {MapDialogText.FormatPosition(mapObject.GetPosition())}",
                $"Rotation: {MapDialogText.FormatPosition(mapObject.GetRotation())}",
                $"World: {mapObject.GetVirtualWorld()}  Interior: {mapObject.GetInterior()}",
                $"Description: {MapDialogText.GetDescriptionOrDash(mapObject)}",
            };
            lines.AddRange(GetMaterialInfoLines(mapObject));
            return string.Join("\n", lines);
        }

        private static IEnumerable<string> GetMaterialInfoLines(MapObject mapObject)
        {
            List<MapObjectMaterial> materials = mapObject.GetMaterials().OrderBy(material => material.GetMaterialIndex()).ToList();
            if (materials.Count == 0)
            {
                return new[] { "Materials: none" };
            }
            return new[] { "Materials:" }.Concat(materials.Select(material => $"  {material.GetMaterialIndex()}: {MapDialogText.GetMaterialContent(material)}"));
        }
    }
}
