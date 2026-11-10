using OpenRP.Framework.Features.Phones.Components;
using OpenRP.Framework.Features.Phones.Enums;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Extensions;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Phones.Services.Dialogs
{
    public partial class PhoneDialogService
    {
        /// <summary>
        /// Opening the call log marks the missed calls as seen; they're still marked new in this list.
        /// </summary>
        private void OpenCallLog(Player player, Phone phone, Action onBack)
        {
            BetterListDialog callLogDialog = new BetterListDialog("Choose", "Back");
            callLogDialog.SetTitle(TitleType.Children, GetTitle(phone), "Call log");
            Dictionary<int, PhoneCallLog> callLogsByRow = new Dictionary<int, PhoneCallLog>();
            foreach (PhoneCallLog callLog in _phoneService.GetCallLogs(phone))
            {
                callLogsByRow[callLogDialog.AddRow(GetCallLogLabel(phone, callLog))] = callLog;
            }
            if (callLogsByRow.Count == 0)
            {
                ShowMessage(player, "Call log", "There are no calls in the call log.", onBack);
                return;
            }
            _phoneEditorService.SetMissedCallsSeen(phone);

            void OnChosen(ListDialogResponse response)
            {
                Action reopenCallLog = () => WithCarriedPhone(player, phone, () => OpenCallLog(player, phone, onBack));
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (callLogsByRow.TryGetValue(response.ItemIndex, out PhoneCallLog? callLog))
                    OpenCallLogActions(player, phone, callLog.GetOtherNumber(), reopenCallLog);
            }

            _dialogService.ShowIgnoringDisconnect(player, callLogDialog, OnChosen);
        }

        private void OpenCallLogActions(Player player, Phone phone, uint otherNumber, Action onBack)
        {
            bool isContact = _phoneService.GetContacts(phone).Any(contact => contact.GetNumber() == otherNumber);

            BetterListDialog actionsDialog = new BetterListDialog("Choose", "Back");
            actionsDialog.SetTitle(TitleType.Children, GetTitle(phone), _phoneService.GetNumberLabel(phone, otherNumber));
            int callRow = actionsDialog.AddRow("Call back");
            int textRow = actionsDialog.AddRow("Send a message");
            int? saveRow = isContact ? null : actionsDialog.AddRow("Save the number");

            void OnChosen(ListDialogResponse response)
            {
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (response.ItemIndex == callRow)
                    Dial(player, phone, otherNumber, onBack);
                else if (response.ItemIndex == textRow)
                    OpenWriteMessage(player, phone, otherNumber, onBack);
                else if (response.ItemIndex == saveRow)
                    OpenAddContact(player, phone, otherNumber, onBack);
            }

            _dialogService.ShowIgnoringDisconnect(player, actionsDialog, OnChosen);
        }

        private string GetCallLogLabel(Phone phone, PhoneCallLog callLog)
        {
            string other = _phoneService.GetNumberLabel(phone, callLog.GetOtherNumber());
            string time = $"{ChatColor.LightGray}{GetTime(callLog.GetStartedAt())}";
            return callLog.GetCallType() switch
            {
                PhoneCallLogType.Missed => $"{(callLog.IsSeen() ? string.Empty : $"{ChatColor.Highlight}[New] {ChatColor.White}")}Missed: {other} {time}",
                PhoneCallLogType.Incoming => $"Received: {other} {time} ({GetDuration(callLog.GetDurationSeconds())})",
                _ => $"Dialed: {other} {time} ({GetDuration(callLog.GetDurationSeconds())})"
            };
        }

        private static string GetDuration(int seconds)
        {
            return $"{seconds / 60}:{seconds % 60:00}";
        }
    }
}
