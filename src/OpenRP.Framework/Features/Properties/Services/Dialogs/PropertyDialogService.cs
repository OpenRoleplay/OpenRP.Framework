using OpenRP.Framework.Features.Properties.Components;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Helpers;
using SampSharp.Entities.SAMP;
using System;

namespace OpenRP.Framework.Features.Properties.Services.Dialogs
{
    /// <summary>
    /// The /editproperties menus. One file per flow: the properties and the doors.
    /// </summary>
    public partial class PropertyDialogService : IPropertyDialogService
    {
        private const string PropertiesTitle = "Properties";

        private readonly IDialogService _dialogService;
        private readonly IPropertyService _propertyService;
        private readonly IPropertyEditorService _propertyEditorService;

        public PropertyDialogService(IDialogService dialogService, IPropertyService propertyService, IPropertyEditorService propertyEditorService)
        {
            _dialogService = dialogService;
            _propertyService = propertyService;
            _propertyEditorService = propertyEditorService;
        }

        /// <summary>
        /// Continues with onSuccess when there's no error; otherwise shows the error and continues with onAfterError.
        /// </summary>
        private void ContinueAfter(Player player, string? error, Action onSuccess, Action onAfterError)
        {
            if (error == null)
            {
                onSuccess();
                return;
            }

            ShowMessage(player, "Not possible", error, onAfterError);
        }

        private void ShowMessage(Player player, string title, string text, Action onClose)
        {
            BetterMessageDialog messageDialog = new BetterMessageDialog("Back");
            messageDialog.SetTitle(TitleType.Children, PropertiesTitle, title);
            messageDialog.SetContent(text);
            _dialogService.Show(player.Entity, messageDialog, _ => onClose());
        }

        private void ShowConfirmation(Player player, string title, string question, Action onConfirmed, Action onCancelled)
        {
            BetterMessageDialog confirmationDialog = new BetterMessageDialog(DialogHelper.Yes, DialogHelper.No);
            confirmationDialog.SetTitle(TitleType.Children, PropertiesTitle, title);
            confirmationDialog.SetContent(question);
            _dialogService.Show(player.Entity, confirmationDialog, response =>
            {
                if (response.Response == DialogResponse.LeftButton)
                {
                    onConfirmed();
                    return;
                }
                onCancelled();
            });
        }

        private void OpenNameInput(Player player, string title, string prompt, Action<string> onEntered, Action onBack)
        {
            BetterInputDialog inputDialog = new BetterInputDialog("Continue", "Back");
            inputDialog.SetTitle(TitleType.Children, PropertiesTitle, title);
            inputDialog.SetContent(prompt);
            _dialogService.Show(player.Entity, inputDialog, response =>
            {
                if (response.Response != DialogResponse.LeftButton)
                {
                    onBack();
                    return;
                }
                onEntered(response.InputText);
            });
        }

        private static int? AddRowIf(BetterListDialog dialog, bool condition, string text)
        {
            return condition ? dialog.AddRow(text) : null;
        }

        private static string GetPropertyLabel(Property property)
        {
            int doorCount = property.GetDoors().Count;
            string doors = doorCount == 1 ? "1 door" : $"{doorCount} doors";
            return $"{property.GetName()} {ChatColor.LightGray}(#{property.GetId()}) - {doors}";
        }

        private static string GetDoorName(PropertyDoor door)
        {
            return $"{door.GetName()} {ChatColor.LightGray}(#{door.GetId()}){ChatColor.White}";
        }

        /// <summary>
        /// The door's name and property, for lists that mix the doors of all properties.
        /// </summary>
        private static string GetDoorNameWithProperty(PropertyDoor door)
        {
            string? propertyName = door.GetProperty()?.GetName();
            return propertyName == null ? GetDoorName(door) : $"{propertyName}: {GetDoorName(door)}";
        }
    }
}
