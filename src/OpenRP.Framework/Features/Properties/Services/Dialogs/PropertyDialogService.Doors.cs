using OpenRP.Framework.Features.Properties.Components;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Properties.Services.Dialogs
{
    public partial class PropertyDialogService
    {
        private void OpenDoorActions(Player player, PropertyDoor door, Action onBack)
        {
            if (door.IsDeleted())
            {
                ShowMessage(player, door.GetName(), "This door has been deleted.", onBack);
                return;
            }

            BetterListDialog actionsDialog = new BetterListDialog("Choose", "Back");
            actionsDialog.SetTitle(TitleType.Children, PropertiesTitle, door.GetProperty()?.GetName() ?? "No property", door.GetName());
            int goToRow = actionsDialog.AddRow("Go to");
            int moveRow = actionsDialog.AddRow("Move here");
            int renameRow = actionsDialog.AddRow("Rename");
            int linkRow = actionsDialog.AddRow(GetLinkRowText(door));
            int? unlinkRow = AddRowIf(actionsDialog, door.GetLinkedDoorId().HasValue, "Unlink");
            int deleteRow = actionsDialog.AddRow("Delete");

            void OnActionChosen(ListDialogResponse response)
            {
                Action reopenActions = () => OpenDoorActions(player, door, onBack);
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (response.ItemIndex == goToRow)
                    _propertyService.MovePlayerToDoor(player, door);
                else if (response.ItemIndex == moveRow)
                    MoveDoorToPlayer(player, door, reopenActions);
                else if (response.ItemIndex == renameRow)
                    OpenRenameDoor(player, door, reopenActions);
                else if (response.ItemIndex == linkRow)
                    OpenLinkDoor(player, door, reopenActions);
                else if (response.ItemIndex == unlinkRow)
                    UnlinkDoor(door, reopenActions);
                else if (response.ItemIndex == deleteRow)
                    OpenDeleteDoor(player, door, onBack, reopenActions);
            }

            _dialogService.Show(player.Entity, actionsDialog, OnActionChosen);
        }

        private string GetLinkRowText(PropertyDoor door)
        {
            PropertyDoor? linkedDoor = GetLinkedDoor(door);
            return linkedDoor == null
                ? "Link to another door"
                : $"Link to another door {ChatColor.LightGray}(now leads to {GetDoorNameWithProperty(linkedDoor)}{ChatColor.LightGray})";
        }

        /// <summary>
        /// The door's name and where it leads, for the doors of one property.
        /// </summary>
        private string GetDoorLabel(PropertyDoor door)
        {
            PropertyDoor? linkedDoor = GetLinkedDoor(door);
            return linkedDoor == null
                ? $"{GetDoorName(door)} {ChatColor.LightGray}- not linked"
                : $"{GetDoorName(door)} -> {GetDoorNameWithProperty(linkedDoor)}";
        }

        private PropertyDoor? GetLinkedDoor(PropertyDoor door)
        {
            ulong? linkedDoorId = door.GetLinkedDoorId();
            return linkedDoorId.HasValue ? _propertyService.GetDoor(linkedDoorId.Value) : null;
        }

        private void MoveDoorToPlayer(Player player, PropertyDoor door, Action onMoved)
        {
            _propertyEditorService.MoveDoorToPlayer(door, player);
            // The admin may be in another virtual world than the door's, for example outside after /gotopos.
            _propertyService.MovePlayerToDoor(player, door);
            onMoved();
        }

        private void OpenRenameDoor(Player player, PropertyDoor door, Action onBack)
        {
            void Rename(string name)
            {
                string? error = _propertyEditorService.RenameDoor(door, name);
                ContinueAfter(player, error, onBack, () => OpenRenameDoor(player, door, onBack));
            }

            OpenNameInput(player, $"Rename {door.GetName()}", $"Enter the new name for {ChatColor.Highlight}{door.GetName()}{ChatColor.White}:", Rename, onBack);
        }

        /// <summary>
        /// Lets the admin pick the door this one leads to. The link goes both ways.
        /// </summary>
        private void OpenLinkDoor(Player player, PropertyDoor door, Action onBack)
        {
            List<PropertyDoor> otherDoors = _propertyEditorService.GetAllDoors().Where(otherDoor => otherDoor.GetId() != door.GetId()).ToList();
            if (otherDoors.Count == 0)
            {
                ShowMessage(player, "Link to another door", "There are no other doors yet. Add one first.", onBack);
                return;
            }

            BetterListDialog doorsDialog = new BetterListDialog("Link", "Back");
            doorsDialog.SetTitle(TitleType.Children, PropertiesTitle, door.GetName(), "Link to");
            Dictionary<int, PropertyDoor> doorsByRow = new Dictionary<int, PropertyDoor>();
            foreach (PropertyDoor otherDoor in otherDoors)
            {
                doorsByRow[doorsDialog.AddRow(GetDoorNameWithProperty(otherDoor))] = otherDoor;
            }

            _dialogService.Show(player.Entity, doorsDialog, response =>
            {
                if (response.Response != DialogResponse.LeftButton || !doorsByRow.TryGetValue(response.ItemIndex, out PropertyDoor? otherDoor))
                {
                    onBack();
                    return;
                }

                string? error = _propertyEditorService.LinkDoors(door, otherDoor);
                ContinueAfter(player, error, onBack, onBack);
            });
        }

        private void UnlinkDoor(PropertyDoor door, Action onUnlinked)
        {
            _propertyEditorService.UnlinkDoor(door);
            onUnlinked();
        }

        private void OpenDeleteDoor(Player player, PropertyDoor door, Action onDeleted, Action onBack)
        {
            void Delete()
            {
                _propertyEditorService.DeleteDoor(door);
                onDeleted();
            }

            ShowConfirmation(player, "Delete door", $"Are you sure you want to delete the door {ChatColor.Highlight}{door.GetLabelText()}{ChatColor.White}?", Delete, onBack);
        }
    }
}
