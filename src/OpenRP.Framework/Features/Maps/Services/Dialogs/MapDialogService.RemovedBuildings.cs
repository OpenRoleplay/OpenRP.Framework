using OpenRP.Framework.Features.Maps.Components;
using OpenRP.Framework.Features.Maps.Helpers;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Maps.Services.Dialogs
{
    public partial class MapDialogService
    {
        private const string RemovedBuildingPrompt = "Enter a building to remove for this map's players. Either model [radius], which removes the building "
            + "around where you stand or fly, for example 3374 or 3374 2.5; or model x y z radius, for example 3374 1.5 2.0 10.0 5. "
            + "The radius defaults to 0.25.";

        private void OpenRemovedBuildings(Player player, Map map, Action onBack)
        {
            if (!EnsureMapExists(player, map))
            {
                return;
            }

            Action reopenList = () => OpenRemovedBuildings(player, map, onBack);
            BetterListDialog removalDialog = new BetterListDialog("Choose", "Back");
            removalDialog.SetTitle(TitleType.Children, MapsTitle, $"Removed buildings: {map.GetName()}");
            Dictionary<int, Action> actions = new Dictionary<int, Action>();
            actions[removalDialog.AddRow("Add")] = () => OpenAddRemovedBuilding(player, map, reopenList);
            foreach (MapRemovedBuilding removal in map.GetRemovedBuildings())
            {
                actions[removalDialog.AddRow(MapDialogText.GetRemovedBuildingLabel(removal))] = () => OpenDeleteRemovedBuilding(player, removal, reopenList);
            }
            ShowActionList(player, removalDialog, actions, onBack);
        }

        private void OpenAddRemovedBuilding(Player player, Map map, Action reopenList)
        {
            if (!EnsureMapExists(player, map))
            {
                return;
            }

            OpenTextInput(player, "Add removed building", RemovedBuildingPrompt,
                text => AddRemovedBuildingFromInput(player, map, text, reopenList), reopenList);
        }

        private void AddRemovedBuildingFromInput(Player player, Map map, string text, Action reopenList)
        {
            string? error = _mapEditorService.AddRemovedBuilding(map, text, _mapEditingService.GetEditorOrigin(player), out _);
            ContinueAfter(player, error, reopenList, () => OpenAddRemovedBuilding(player, map, reopenList));
        }

        private void OpenDeleteRemovedBuilding(Player player, MapRemovedBuilding removal, Action reopenList)
        {
            ShowConfirmation(player, "Removed buildings", "Bring back this building? Players see it again after they reconnect.",
                () => DeleteRemovedBuilding(player, removal, reopenList), reopenList);
        }

        private void DeleteRemovedBuilding(Player player, MapRemovedBuilding removal, Action reopenList)
        {
            string? error = _mapEditorService.DeleteRemovedBuilding(removal);
            ContinueAfter(player, error, reopenList, reopenList);
        }
    }
}
