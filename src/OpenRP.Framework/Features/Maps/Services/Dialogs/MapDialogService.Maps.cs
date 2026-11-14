using OpenRP.Framework.Features.Maps.Components;
using OpenRP.Framework.Features.Maps.Entities;
using OpenRP.Framework.Features.Maps.Helpers;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Extensions;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Maps.Services.Dialogs
{
    public partial class MapDialogService
    {
        private const float TeleportHeightOffset = 1f;
        private const string CreateMapLabel = "Create map";
        private const int MapListColumnCount = 3;

        public void OpenMapEditor(Player player)
        {
            Map? editedMap = _mapEditingService.GetEditedMap(player);
            if (editedMap != null)
            {
                OpenEditorMenu(player, editedMap);
                return;
            }
            OpenMapList(player);
        }

        private void OpenMapList(Player player)
        {
            BetterTablistDialog mapsDialog = new BetterTablistDialog("Open", "Close", MapListColumnCount);
            mapsDialog.SetTitle(TitleType.Children, MapsTitle);
            mapsDialog.AddHeaders("Map", "Objects", "State");
            int createRow = mapsDialog.AddRow(CreateMapLabel, string.Empty, string.Empty);
            Dictionary<int, Map> mapsByRow = new Dictionary<int, Map>();
            foreach (Map map in _mapService.GetMaps())
            {
                mapsByRow[mapsDialog.AddRow(map.GetName(), map.GetObjects().Count.ToString(), GetStateLabel(map))] = map;
            }

            void OnMapChosen(TablistDialogResponse response)
            {
                Action reopenList = () => OpenMapList(player);
                if (response.Response != DialogResponse.LeftButton)
                    return;
                if (response.ItemIndex == createRow)
                    OpenCreateMap(player, reopenList);
                else if (mapsByRow.TryGetValue(response.ItemIndex, out Map? map))
                    OpenMapMenu(player, map, reopenList);
            }

            _dialogService.ShowIgnoringDisconnect(player, mapsDialog, OnMapChosen);
        }

        private void OpenCreateMap(Player player, Action onBack)
        {
            void CreateMap(string name)
            {
                string? error = _mapEditorService.CreateMap(name, out Map? map);
                ContinueAfter(player, error, () => OpenMapMenu(player, map!, onBack), () => OpenCreateMap(player, onBack));
            }

            OpenTextInput(player, "Create map", $"Enter the name of the new map, up to {MapLimits.NameMaxLength} characters:", CreateMap, onBack);
        }

        private void OpenMapMenu(Player player, Map map, Action onBack)
        {
            if (!EnsureMapExists(player, map))
            {
                return;
            }

            Action reopenMenu = () => OpenMapMenu(player, map, onBack);
            BetterListDialog mapDialog = new BetterListDialog("Choose", "Back");
            mapDialog.SetTitle(TitleType.Children, MapsTitle, map.GetName());
            Dictionary<int, Action> actions = new Dictionary<int, Action>();
            actions[mapDialog.AddRow("Edit objects")] = () => OpenEditObjects(player, map, reopenMenu);
            actions[mapDialog.AddRow(map.IsLoaded() ? "Unload" : "Load")] = () => ToggleLoaded(player, map, reopenMenu);
            actions[mapDialog.AddRow("Teleport to map")] = () => TeleportToMap(player, map, reopenMenu);
            actions[mapDialog.AddRow("Rename")] = () => OpenRenameMap(player, map, reopenMenu);
            actions[mapDialog.AddRow("Removed buildings")] = () => OpenRemovedBuildings(player, map, reopenMenu);
            actions[mapDialog.AddRow("Delete")] = () => OpenDeleteMap(player, map, reopenMenu);
            ShowActionList(player, mapDialog, actions, onBack);
        }

        private void OpenEditObjects(Player player, Map map, Action onBack)
        {
            if (!EnsureMapExists(player, map))
            {
                return;
            }

            if (map.IsLoaded())
            {
                BeginEditing(player, map);
                return;
            }

            // Objects of an unloaded map aren't in the game, so there is nothing to see or move until it's loaded.
            ShowConfirmation(player, map.GetName(), $"{map.GetName()} isn't loaded, so its objects aren't in the game. Load it now?",
                () => LoadThenBeginEditing(player, map, onBack), onBack);
        }

        private void LoadThenBeginEditing(Player player, Map map, Action onBack)
        {
            string? error = _mapEditorService.LoadMap(map);
            ContinueAfter(player, error, () => BeginEditing(player, map), onBack);
        }

        private void BeginEditing(Player player, Map map)
        {
            _mapEditingService.StartEditing(player, map);
            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"You're editing {map.GetName()}. Use /editmaps to open the editor.");
            OpenEditorMenu(player, map);
        }

        private void ToggleLoaded(Player player, Map map, Action reopenMenu)
        {
            if (!EnsureMapExists(player, map))
            {
                return;
            }

            string? error = map.IsLoaded() ? _mapEditorService.UnloadMap(map) : _mapEditorService.LoadMap(map);
            ContinueAfter(player, error, reopenMenu, reopenMenu);
        }

        private void TeleportToMap(Player player, Map map, Action onBack)
        {
            if (!EnsureMapExists(player, map))
            {
                return;
            }

            MapObject? firstObject = map.GetObjects().OrderBy(mapObject => mapObject.GetId()).FirstOrDefault();
            if (firstObject == null)
            {
                ShowMessage(player, "Teleport", $"{map.GetName()} has no objects to teleport to.", onBack);
                return;
            }

            if (_flyModeService.IsFlying(player))
            {
                TeleportWhileFlying(player, firstObject, onBack);
                return;
            }
            MovePlayerToObject(player, firstObject);
        }

        /// <summary>
        /// Ends fly mode at the object, so the player lands there instead of where they took off.
        /// </summary>
        private void TeleportWhileFlying(Player player, MapObject mapObject, Action onBack)
        {
            string? error = _flyModeService.StopFlyingAt(player, GetTeleportPosition(mapObject),
                GetTeleportInterior(player, mapObject), GetTeleportVirtualWorld(player, mapObject));
            if (error != null)
            {
                ShowMessage(player, "Teleport", error, onBack);
            }
        }

        /// <summary>
        /// Puts the player just above the object, in the object's world and interior. A world or interior of -1 keeps the player's own.
        /// </summary>
        private static void MovePlayerToObject(Player player, MapObject mapObject)
        {
            player.Interior = GetTeleportInterior(player, mapObject);
            player.VirtualWorld = GetTeleportVirtualWorld(player, mapObject);
            player.Position = GetTeleportPosition(mapObject);
        }

        private static int GetTeleportInterior(Player player, MapObject mapObject)
        {
            return mapObject.GetInterior() == MapLimits.EverywhereValue ? player.Interior : mapObject.GetInterior();
        }

        private static int GetTeleportVirtualWorld(Player player, MapObject mapObject)
        {
            return mapObject.GetVirtualWorld() == MapLimits.EverywhereValue ? player.VirtualWorld : mapObject.GetVirtualWorld();
        }

        private static Vector3 GetTeleportPosition(MapObject mapObject)
        {
            return mapObject.GetPosition() + new Vector3(0f, 0f, TeleportHeightOffset);
        }

        private void OpenRenameMap(Player player, Map map, Action onBack)
        {
            if (!EnsureMapExists(player, map))
            {
                return;
            }

            void Rename(string newName)
            {
                string? error = _mapEditorService.RenameMap(map, newName);
                ContinueAfter(player, error, onBack, () => OpenRenameMap(player, map, onBack));
            }

            OpenTextInput(player, $"Rename {map.GetName()}", $"Enter the new name for {ChatColor.Highlight}{map.GetName()}{ChatColor.White}:", Rename, onBack);
        }

        private void OpenDeleteMap(Player player, Map map, Action reopenMenu)
        {
            if (!EnsureMapExists(player, map))
            {
                return;
            }

            string objects = MapDialogText.GetObjectCountText(map.GetObjects().Count);
            ShowConfirmation(player, "Delete map", $"Delete {map.GetName()} and its {objects}? This can't be undone.",
                () => DeleteMap(player, map), reopenMenu);
        }

        private void DeleteMap(Player player, Map map)
        {
            string? error = _mapEditorService.DeleteMap(map);
            ContinueAfter(player, error, () => OpenMapList(player), () => OpenMapList(player));
        }

        private static string GetStateLabel(Map map)
        {
            return map.IsLoaded() ? "Loaded" : "Unloaded";
        }
    }
}
