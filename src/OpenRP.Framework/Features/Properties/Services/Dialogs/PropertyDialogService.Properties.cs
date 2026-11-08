using OpenRP.Framework.Features.Properties.Components;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Properties.Services.Dialogs
{
    public partial class PropertyDialogService
    {
        public void OpenPropertyEditor(Player player)
        {
            BetterListDialog propertiesDialog = new BetterListDialog("Choose", "Close");
            propertiesDialog.SetTitle(TitleType.Children, PropertiesTitle);
            int createRow = propertiesDialog.AddRow($"{ChatColor.CornflowerBlue}Create property");
            PropertyDoor? nearbyDoor = _propertyService.GetNearbyDoor(player);
            int? nearbyDoorRow = nearbyDoor == null ? null : propertiesDialog.AddRow($"{ChatColor.CornflowerBlue}Door I'm standing at: {ChatColor.White}{GetDoorNameWithProperty(nearbyDoor)}");
            Dictionary<int, Property> propertiesByRow = new Dictionary<int, Property>();
            foreach (Property property in _propertyEditorService.GetProperties())
            {
                propertiesByRow[propertiesDialog.AddRow(GetPropertyLabel(property))] = property;
            }

            void OnPropertyChosen(ListDialogResponse response)
            {
                Action reopenEditor = () => OpenPropertyEditor(player);
                if (response.Response != DialogResponse.LeftButton)
                    return;
                if (response.ItemIndex == createRow)
                    OpenCreateProperty(player, reopenEditor);
                else if (response.ItemIndex == nearbyDoorRow && nearbyDoor != null)
                    OpenDoorActions(player, nearbyDoor, reopenEditor);
                else if (propertiesByRow.TryGetValue(response.ItemIndex, out Property? property))
                    OpenPropertyActions(player, property, reopenEditor);
            }

            _dialogService.Show(player, propertiesDialog, OnPropertyChosen);
        }

        private void OpenPropertyActions(Player player, Property property, Action onBack)
        {
            if (property.IsDeleted())
            {
                ShowMessage(player, property.GetName(), "This property has been deleted.", onBack);
                return;
            }

            List<PropertyDoor> doors = property.GetDoors();
            BetterListDialog actionsDialog = new BetterListDialog("Choose", "Back");
            actionsDialog.SetTitle(TitleType.Children, PropertiesTitle, property.GetName());
            int renameRow = actionsDialog.AddRow("Rename");
            int addDoorRow = actionsDialog.AddRow("Add a door here");
            int? goToRow = AddRowIf(actionsDialog, doors.Count > 0, "Go to");
            int deleteRow = actionsDialog.AddRow("Delete");
            Dictionary<int, PropertyDoor> doorsByRow = new Dictionary<int, PropertyDoor>();
            foreach (PropertyDoor door in doors)
            {
                doorsByRow[actionsDialog.AddRow(GetDoorLabel(door))] = door;
            }

            void OnActionChosen(ListDialogResponse response)
            {
                Action reopenActions = () => OpenPropertyActions(player, property, onBack);
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (response.ItemIndex == renameRow)
                    OpenRenameProperty(player, property, reopenActions);
                else if (response.ItemIndex == addDoorRow)
                    OpenAddDoor(player, property, reopenActions);
                else if (response.ItemIndex == goToRow)
                    _propertyService.MovePlayerToDoor(player, doors[0]);
                else if (response.ItemIndex == deleteRow)
                    OpenDeleteProperty(player, property, onBack, reopenActions);
                else if (doorsByRow.TryGetValue(response.ItemIndex, out PropertyDoor? door))
                    OpenDoorActions(player, door, reopenActions);
            }

            _dialogService.Show(player, actionsDialog, OnActionChosen);
        }

        private void OpenCreateProperty(Player player, Action onBack)
        {
            void Create(string name)
            {
                (Property? property, string? error) = _propertyEditorService.CreateProperty(name);
                ContinueAfter(player, error, () => OpenPropertyActions(player, property!, onBack), () => OpenCreateProperty(player, onBack));
            }

            OpenNameInput(player, "Create property", "Enter the name of the new property:", Create, onBack);
        }

        private void OpenRenameProperty(Player player, Property property, Action onBack)
        {
            void Rename(string name)
            {
                string? error = _propertyEditorService.RenameProperty(property, name);
                ContinueAfter(player, error, onBack, () => OpenRenameProperty(player, property, onBack));
            }

            OpenNameInput(player, $"Rename {property.GetName()}", $"Enter the new name for {ChatColor.Highlight}{property.GetName()}{ChatColor.White}:", Rename, onBack);
        }

        /// <summary>
        /// Puts the admin on the new door afterwards: inside an interior, that's the property's virtual world, where its label is.
        /// </summary>
        private void OpenAddDoor(Player player, Property property, Action onBack)
        {
            void Create(string name)
            {
                (PropertyDoor? door, string? error) = _propertyEditorService.CreateDoor(property, name, player);
                ContinueAfter(player, error, () => ShowCreatedDoor(player, door!, onBack), () => OpenAddDoor(player, property, onBack));
            }

            OpenNameInput(player, "Add a door", $"The door is placed where you stand, facing the way you face. Enter the name of the new door of {ChatColor.Highlight}{property.GetName()}{ChatColor.White}, for example Entrance or Exit:", Create, onBack);
        }

        private void ShowCreatedDoor(Player player, PropertyDoor door, Action onBack)
        {
            _propertyService.MovePlayerToDoor(player, door);
            OpenDoorActions(player, door, onBack);
        }

        private void OpenDeleteProperty(Player player, Property property, Action onDeleted, Action onBack)
        {
            void Delete()
            {
                _propertyEditorService.DeleteProperty(property);
                onDeleted();
            }

            ShowConfirmation(player, "Delete property", $"Are you sure you want to delete {ChatColor.Highlight}{property.GetName()}{ChatColor.White} and all of its doors?", Delete, onBack);
        }
    }
}
