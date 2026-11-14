using OpenRP.Framework.Features.FlyMode.Services;
using OpenRP.Framework.Features.Maps.Components;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Extensions;
using OpenRP.Framework.Shared.Dialogs.Helpers;
using OpenRP.OpenMp.Streamer;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Maps.Services.Dialogs
{
    /// <summary>
    /// The /editmaps menus. One file per flow: the maps, the editor, the objects, their materials and the removed buildings.
    /// </summary>
    public partial class MapDialogService : IMapDialogService
    {
        private const string MapsTitle = "Maps";
        private const string NotPossibleTitle = "Not possible";
        private const string MapGoneMessage = "That map no longer exists.";
        private const string ObjectGoneMessage = "That object no longer exists.";

        private readonly IDialogService _dialogService;
        private readonly IMapService _mapService;
        private readonly IMapEditorService _mapEditorService;
        private readonly IMapEditingService _mapEditingService;
        private readonly IFlyModeService _flyModeService;
        private readonly IStreamerService _streamerService;

        public MapDialogService(IDialogService dialogService, IMapService mapService, IMapEditorService mapEditorService,
            IMapEditingService mapEditingService, IFlyModeService flyModeService, IStreamerService streamerService)
        {
            _dialogService = dialogService;
            _mapService = mapService;
            _mapEditorService = mapEditorService;
            _mapEditingService = mapEditingService;
            _flyModeService = flyModeService;
            _streamerService = streamerService;
        }

        /// <summary>
        /// Shows that the map was deleted and goes back to the map list. Returns false when the map is gone, so the caller stops.
        /// </summary>
        private bool EnsureMapExists(Player player, Map map)
        {
            if (IsAlive(map))
            {
                return true;
            }
            ShowMessage(player, NotPossibleTitle, MapGoneMessage, () => OpenMapList(player));
            return false;
        }

        /// <summary>
        /// Shows that the object was deleted and goes back to the editor. Returns false when the object is gone, so the caller stops.
        /// </summary>
        private bool EnsureObjectExists(Player player, MapObject mapObject)
        {
            if (IsAlive(mapObject))
            {
                return true;
            }
            ShowMessage(player, NotPossibleTitle, ObjectGoneMessage, () => OpenMapEditor(player));
            return false;
        }

        private static bool IsAlive(Map map)
        {
            return map.IsComponentAlive && !map.IsDeleted();
        }

        private static bool IsAlive(MapObject mapObject)
        {
            return mapObject.IsComponentAlive && !mapObject.IsDeleted();
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

            ShowMessage(player, NotPossibleTitle, error, onAfterError);
        }

        private void ShowMessage(Player player, string title, string text, Action onClose)
        {
            BetterMessageDialog messageDialog = new BetterMessageDialog("Back");
            messageDialog.SetTitle(TitleType.Children, MapsTitle, title);
            messageDialog.SetContent(text);
            _dialogService.ShowIgnoringDisconnect(player, messageDialog, _ => onClose());
        }

        private void ShowConfirmation(Player player, string title, string question, Action onConfirmed, Action onCancelled)
        {
            BetterMessageDialog confirmationDialog = new BetterMessageDialog(DialogHelper.Yes, DialogHelper.No);
            confirmationDialog.SetTitle(TitleType.Children, MapsTitle, title);
            confirmationDialog.SetContent(question);
            _dialogService.ShowIgnoringDisconnect(player, confirmationDialog, response =>
            {
                if (response.Response == DialogResponse.LeftButton)
                {
                    onConfirmed();
                    return;
                }
                onCancelled();
            });
        }

        private void OpenTextInput(Player player, string title, string prompt, Action<string> onEntered, Action onBack)
        {
            BetterInputDialog inputDialog = new BetterInputDialog("Continue", "Back");
            inputDialog.SetTitle(TitleType.Children, MapsTitle, title);
            inputDialog.SetContent(prompt);
            _dialogService.ShowIgnoringDisconnect(player, inputDialog, response =>
            {
                if (response.Response != DialogResponse.LeftButton)
                {
                    onBack();
                    return;
                }
                onEntered(response.InputText ?? string.Empty);
            });
        }

        /// <summary>
        /// Shows a list whose rows each have an action. The button or row that has no action leads to onBack.
        /// </summary>
        private void ShowActionList(Player player, BetterListDialog dialog, IReadOnlyDictionary<int, Action> actions, Action onBack)
        {
            _dialogService.ShowIgnoringDisconnect(player, dialog, response =>
            {
                if (response.Response == DialogResponse.LeftButton && actions.TryGetValue(response.ItemIndex, out Action? action))
                {
                    action();
                    return;
                }
                onBack();
            });
        }

        /// <summary>
        /// Reopens the editor of the map the admin is editing, when they still are editing one.
        /// </summary>
        private void ReopenEditor(Player player)
        {
            Map? editedMap = _mapEditingService.GetEditedMap(player);
            if (editedMap != null)
            {
                OpenEditorMenu(player, editedMap);
            }
        }
    }
}
