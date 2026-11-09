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
            int insidePropertyRow = actionsDialog.AddRow(GetInsidePropertyRowText(door));
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
                else if (response.ItemIndex == insidePropertyRow)
                    OpenChooseInsideProperty(player, door, reopenActions);
                else if (response.ItemIndex == linkRow)
                    OpenLinkDoor(player, door, reopenActions);
                else if (response.ItemIndex == unlinkRow)
                    UnlinkDoor(door, reopenActions);
                else if (response.ItemIndex == deleteRow)
                    OpenDeleteDoor(player, door, onBack, reopenActions);
            }

            _dialogService.Show(player, actionsDialog, OnActionChosen);
        }

        private string GetInsidePropertyRowText(PropertyDoor door)
        {
            return $"Virtual world {ChatColor.LightGray}({GetInsidePropertyText(door)}{ChatColor.LightGray})";
        }

        private string GetInsidePropertyText(PropertyDoor door)
        {
            ulong? insidePropertyId = door.GetInsidePropertyId();
            Property? insideProperty = insidePropertyId.HasValue ? _propertyEditorService.GetProperty(insidePropertyId.Value) : null;
            return insideProperty == null ? "outside, shared world" : $"inside {insideProperty.GetName()}";
        }

        /// <summary>
        /// Lets the admin pick whose virtual world the door is in: outside, its own property's (usual for an
        /// interior), or another property's (a door inside another property, such as an apartment in a building).
        /// </summary>
        private void OpenChooseInsideProperty(Player player, PropertyDoor door, Action onBack)
        {
            BetterListDialog worldsDialog = new BetterListDialog("Choose", "Back");
            worldsDialog.SetTitle(TitleType.Children, PropertiesTitle, door.GetName(), "Virtual world");
            Dictionary<int, Property?> propertiesByRow = new Dictionary<int, Property?>
            {
                [worldsDialog.AddRow("Outside (shared world)")] = null
            };
            foreach (Property property in GetInsidePropertyChoices(door))
            {
                propertiesByRow[worldsDialog.AddRow($"Inside {property.GetName()}")] = property;
            }

            _dialogService.Show(player, worldsDialog, response =>
            {
                if (response.Response == DialogResponse.LeftButton && propertiesByRow.TryGetValue(response.ItemIndex, out Property? insideProperty))
                {
                    SetDoorInsideProperty(player, door, insideProperty);
                }
                onBack();
            });
        }

        /// <summary>
        /// The door's own property first, then every other property.
        /// </summary>
        private List<Property> GetInsidePropertyChoices(PropertyDoor door)
        {
            Property? ownProperty = door.GetProperty();
            List<Property> otherProperties = _propertyEditorService.GetProperties().Where(property => property.GetId() != ownProperty?.GetId()).ToList();
            return ownProperty == null ? otherProperties : otherProperties.Prepend(ownProperty).ToList();
        }

        private void SetDoorInsideProperty(Player player, PropertyDoor door, Property? insideProperty)
        {
            _propertyEditorService.SetDoorInsideProperty(door, insideProperty);
            // The door's label is now in another virtual world; put the admin there so they still see it.
            _propertyService.MovePlayerToDoor(player, door);
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

            _dialogService.Show(player, doorsDialog, response =>
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
