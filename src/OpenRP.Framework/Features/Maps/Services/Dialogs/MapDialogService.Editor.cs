using OpenRP.Framework.Features.Maps.Components;
using OpenRP.Framework.Features.Maps.Helpers;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Helpers;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Maps.Services.Dialogs
{
    public partial class MapDialogService
    {
        private const float NearestObjectDistance = 20f;

        private void OpenEditorMenu(Player player, Map map)
        {
            if (!EnsureMapExists(player, map))
            {
                return;
            }

            Action reopenEditor = () => OpenEditorMenu(player, map);
            BetterListDialog editorDialog = new BetterListDialog("Choose", "Close");
            editorDialog.SetTitle(TitleType.Children, MapsTitle, $"Editing: {map.GetName()}");
            Dictionary<int, Action> actions = new Dictionary<int, Action>();
            actions[editorDialog.AddRow("Select object (click)")] = () => BeginSelecting(player, map);
            actions[editorDialog.AddRow("Nearest object")] = () => SelectNearestObject(player, map, reopenEditor);
            actions[editorDialog.AddRow("Object list")] = () => OpenObjectList(player, map);
            actions[editorDialog.AddRow("Search by model")] = () => OpenModelSearch(player, map);
            actions[editorDialog.AddRow("Add object")] = () => OpenAddObject(player, map);
            actions[editorDialog.AddRow($"Fly mode: {DialogHelper.GetBooleanAsOnOrOff(_flyModeService.IsFlying(player))}")] = () => ToggleFlying(player, map);
            actions[editorDialog.AddRow("Removed buildings")] = () => OpenRemovedBuildings(player, map, reopenEditor);
            actions[editorDialog.AddRow("Map menu")] = () => OpenMapMenu(player, map, () => OpenMapList(player));
            actions[editorDialog.AddRow("Stop editing")] = () => StopEditing(player, map);
            ShowActionList(player, editorDialog, actions, () => { });
        }

        /// <summary>
        /// The admin clicks an object; the click handler takes it from there.
        /// </summary>
        private void BeginSelecting(Player player, Map map)
        {
            if (!EnsureMapNotBusy(player, map))
            {
                return;
            }

            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, "Click an object of this map. Press Escape to stop.");
            player.Select();
        }

        private void SelectNearestObject(Player player, Map map, Action onBack)
        {
            if (!EnsureMapExists(player, map) || !EnsureMapNotBusy(player, map))
            {
                return;
            }

            Vector3 origin = _mapEditingService.GetEditorOrigin(player);
            MapObject? nearestObject = MapObjectListing.FindNearest(_mapStore.GetObjects(map), origin, NearestObjectDistance);
            if (nearestObject == null)
            {
                ShowMessage(player, "Nearest object", $"No object of {map.GetName()} is within {NearestObjectDistance} m of you.", onBack);
                return;
            }
            SelectAndOpenObject(player, nearestObject, onBack);
        }

        private void OpenObjectList(Player player, Map map)
        {
            if (!EnsureMapExists(player, map) || !EnsureMapNotBusy(player, map))
            {
                return;
            }
            OpenFilteredObjectList(player, map, "Objects", _ => true);
        }

        private void OpenModelSearch(Player player, Map map)
        {
            if (!EnsureMapExists(player, map))
            {
                return;
            }
            OpenTextInput(player, "Search by model", "Enter the model ID of the objects to list, for example 3374:",
                text => SearchByModel(player, map, text), () => OpenEditorMenu(player, map));
        }

        private void SearchByModel(Player player, Map map, string text)
        {
            if (!MapDialogInput.TryParseModelId(text, out int modelId))
            {
                ShowMessage(player, "Search by model", "Enter a whole model ID, not 0.", () => OpenModelSearch(player, map));
                return;
            }
            OpenFilteredObjectList(player, map, $"Model {modelId}", mapObject => mapObject.GetModelId() == modelId);
        }

        /// <summary>
        /// Lists the map's objects that match, ordered by id. Picking one selects it and opens its menu.
        /// </summary>
        private void OpenFilteredObjectList(Player player, Map map, string title, Func<MapObject, bool> matches)
        {
            if (!EnsureMapExists(player, map) || !EnsureMapNotBusy(player, map))
            {
                return;
            }

            Action reopenEditor = () => OpenEditorMenu(player, map);
            (List<MapObject> listedObjects, int matchingCount) = MapObjectListing.GetListedObjects(
                _mapStore.GetObjects(map).Where(matches), _mapEditingService.GetEditorOrigin(player));
            if (matchingCount == 0)
            {
                ShowMessage(player, title, $"No objects of {map.GetName()} match.", reopenEditor);
                return;
            }

            BetterListDialog listDialog = new BetterListDialog("Choose", "Back");
            listDialog.SetTitle(TitleType.Children, MapsTitle, map.GetName(), MapObjectListing.GetListTitle(title, matchingCount));
            Dictionary<int, Action> actions = new Dictionary<int, Action>();
            foreach (MapObject mapObject in listedObjects)
            {
                actions[listDialog.AddRow(MapDialogText.GetObjectLabel(mapObject))] = () => SelectAndOpenObject(player, mapObject, reopenEditor);
            }
            ShowActionList(player, listDialog, actions, reopenEditor);
        }

        private void OpenAddObject(Player player, Map map)
        {
            if (!EnsureMapExists(player, map))
            {
                return;
            }

            OpenTextInput(player, "Add object",
                "Enter the model ID and an optional description, for example 3374 Shop sign. The object is placed in front of you.",
                input => AddObjectFromInput(player, map, input), () => OpenEditorMenu(player, map));
        }

        private void AddObjectFromInput(Player player, Map map, string input)
        {
            (string modelText, string? description) = MapDialogInput.SplitModelAndDescription(input);
            if (!MapDialogInput.TryParseModelId(modelText, out int modelId))
            {
                ShowMessage(player, "Add object", "The model ID must be a whole number, not 0. The description comes after it.", () => OpenAddObject(player, map));
                return;
            }

            Vector3 position = _mapEditingService.GetPointInFrontOfEditor(player);
            string? error = _mapEditorService.AddObject(map, modelId, position, player.VirtualWorld, player.Interior, description, out MapObject? mapObject);
            ContinueAfter(player, error, () => SelectAndOpenObject(player, mapObject!, () => OpenEditorMenu(player, map)), () => OpenAddObject(player, map));
        }

        private void ToggleFlying(Player player, Map map)
        {
            if (!EnsureMapExists(player, map))
            {
                return;
            }

            string? error = _flyModeService.ToggleFlying(player);
            Action reopenEditor = () => OpenEditorMenu(player, map);
            ContinueAfter(player, error, reopenEditor, reopenEditor);
        }

        private void StopEditing(Player player, Map map)
        {
            if (!EnsureMapExists(player, map))
            {
                return;
            }

            _mapEditingService.StopEditing(player);
            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"You stopped editing {map.GetName()}.");
        }
    }
}
