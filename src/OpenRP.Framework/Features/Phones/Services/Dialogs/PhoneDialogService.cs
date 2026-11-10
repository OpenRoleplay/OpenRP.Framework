using OpenRP.Framework.Features.Phones.Components;
using OpenRP.Framework.Features.Phones.Helpers;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Extensions;
using OpenRP.Framework.Shared.Dialogs.Helpers;
using SampSharp.Entities.SAMP;
using System;

namespace OpenRP.Framework.Features.Phones.Services.Dialogs
{
    /// <summary>
    /// The /phone menus. One file per flow: the menu, the phone book, messages, the call log and incoming calls.
    /// </summary>
    public partial class PhoneDialogService : IPhoneDialogService
    {
        private const string PhoneTitle = "Phone";

        private readonly IDialogService _dialogService;
        private readonly IPhoneService _phoneService;
        private readonly IPhoneEditorService _phoneEditorService;
        private readonly IPhoneCallService _phoneCallService;
        private readonly IPhoneRoleplayService _phoneRoleplayService;

        public PhoneDialogService(IDialogService dialogService, IPhoneService phoneService, IPhoneEditorService phoneEditorService, IPhoneCallService phoneCallService,
            IPhoneRoleplayService phoneRoleplayService)
        {
            _phoneRoleplayService = phoneRoleplayService;
            _dialogService = dialogService;
            _phoneService = phoneService;
            _phoneEditorService = phoneEditorService;
            _phoneCallService = phoneCallService;
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
            messageDialog.SetTitle(TitleType.Children, PhoneTitle, title);
            messageDialog.SetContent(text);
            _dialogService.ShowIgnoringDisconnect(player, messageDialog, _ => onClose());
        }

        private void ShowConfirmation(Player player, string title, string question, Action onConfirmed, Action onCancelled)
        {
            BetterMessageDialog confirmationDialog = new BetterMessageDialog(DialogHelper.Yes, DialogHelper.No);
            confirmationDialog.SetTitle(TitleType.Children, PhoneTitle, title);
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
            inputDialog.SetTitle(TitleType.Children, PhoneTitle, title);
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

        /// <summary>
        /// Asks for a phone number and continues with it, asking again when it isn't one.
        /// </summary>
        private void OpenNumberInput(Player player, string title, Action<uint> onEntered, Action onBack)
        {
            void OnEntered(string text)
            {
                uint? number = PhoneNumbers.Parse(text);
                if (number == null)
                {
                    ShowMessage(player, title, "That isn't a phone number. Enter up to 7 digits, for example 382-0142.", () => OpenNumberInput(player, title, onEntered, onBack));
                    return;
                }
                onEntered(number.Value);
            }

            OpenTextInput(player, title, "Enter the number, for example 382-0142:", OnEntered, onBack);
        }

        /// <summary>
        /// Runs the action when the player still carries the phone; the phone may have been dropped or handed over
        /// while a menu was open.
        /// </summary>
        private void WithCarriedPhone(Player player, Phone phone, Action action)
        {
            if (!_phoneService.IsCarrying(player, phone))
            {
                ShowMessage(player, "Phone", "You don't have that phone anymore.", () => { });
                return;
            }
            action();
        }

        private void Dial(Player player, Phone phone, uint number, Action onBack)
        {
            WithCarriedPhone(player, phone, () =>
            {
                string? error = _phoneCallService.Dial(player, phone, number);
                if (error != null)
                {
                    ShowMessage(player, "Call", error, onBack);
                }
            });
        }

        private string GetTitle(Phone phone)
        {
            return $"{PhoneTitle} {PhoneNumbers.Format(phone.GetNumber())}";
        }

        private static string GetTime(DateTime utcTime)
        {
            return utcTime.ToLocalTime().ToString("dd MMM HH:mm");
        }

        private static string Shorten(string text, int length)
        {
            return text.Length <= length ? text : $"{text[..(length - 3)]}...";
        }

        private static string Highlight(string text)
        {
            return $"{ChatColor.Highlight}{text}{ChatColor.White}";
        }
    }
}
