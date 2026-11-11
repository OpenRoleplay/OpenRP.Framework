using OpenRP.Framework.Features.Factions.Services.Dialogs;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Extensions;
using OpenRP.Framework.Shared.Dialogs.Helpers;
using SampSharp.Entities.SAMP;
using System;

namespace OpenRP.Framework.Features.Radios.Services.Dialogs
{
    /// <summary>
    /// The /radio and /editfrequencies menus. One file per flow: the radio menu and the frequency editor.
    /// </summary>
    public partial class RadioDialogService : IRadioDialogService
    {
        private const string RadioTitle = "Radio";
        private const string FrequenciesTitle = "Frequencies";

        private readonly IDialogService _dialogService;
        private readonly IRadioService _radioService;
        private readonly IRadioFrequencyService _radioFrequencyService;
        private readonly IRadioEditorService _radioEditorService;
        private readonly IFactionDialogService _factionDialogService;
        private readonly IRadioBatteryService _radioBatteryService;

        public RadioDialogService(IDialogService dialogService, IRadioService radioService, IRadioFrequencyService radioFrequencyService, IRadioEditorService radioEditorService,
            IFactionDialogService factionDialogService, IRadioBatteryService radioBatteryService)
        {
            _radioBatteryService = radioBatteryService;
            _dialogService = dialogService;
            _radioService = radioService;
            _radioFrequencyService = radioFrequencyService;
            _radioEditorService = radioEditorService;
            _factionDialogService = factionDialogService;
        }

        /// <summary>
        /// Continues with onSuccess when there's no error; otherwise shows the error and continues with onAfterError.
        /// </summary>
        private void ContinueAfter(Player player, string title, string? error, Action onSuccess, Action onAfterError)
        {
            if (error == null)
            {
                onSuccess();
                return;
            }

            ShowMessage(player, title, "Not possible", error, onAfterError);
        }

        private void ShowMessage(Player player, string title, string subtitle, string text, Action onClose)
        {
            BetterMessageDialog messageDialog = new BetterMessageDialog("Back");
            messageDialog.SetTitle(TitleType.Children, title, subtitle);
            messageDialog.SetContent(text);
            _dialogService.ShowIgnoringDisconnect(player, messageDialog, _ => onClose());
        }

        private void ShowConfirmation(Player player, string title, string subtitle, string question, Action onConfirmed, Action onCancelled)
        {
            BetterMessageDialog confirmationDialog = new BetterMessageDialog(DialogHelper.Yes, DialogHelper.No);
            confirmationDialog.SetTitle(TitleType.Children, title, subtitle);
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

        private void OpenTextInput(Player player, string title, string subtitle, string prompt, Action<string> onEntered, Action onBack)
        {
            BetterInputDialog inputDialog = new BetterInputDialog("Continue", "Back");
            inputDialog.SetTitle(TitleType.Children, title, subtitle);
            inputDialog.SetContent(prompt);
            _dialogService.ShowIgnoringDisconnect(player, inputDialog, response =>
            {
                if (response.Response != DialogResponse.LeftButton)
                {
                    onBack();
                    return;
                }
                onEntered(response.InputText);
            });
        }
    }
}
