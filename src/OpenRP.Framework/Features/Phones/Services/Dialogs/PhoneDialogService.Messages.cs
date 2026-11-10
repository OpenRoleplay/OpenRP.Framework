using OpenRP.Framework.Features.Phones.Components;
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
        private const int MessagePreviewLength = 40;

        private void OpenMessages(Player player, Phone phone, Action onBack)
        {
            BetterListDialog messagesDialog = new BetterListDialog("Read", "Back");
            messagesDialog.SetTitle(TitleType.Children, GetTitle(phone), "Messages");
            int writeRow = messagesDialog.AddRow($"{ChatColor.CornflowerBlue}Write a message");
            Dictionary<int, PhoneMessage> messagesByRow = new Dictionary<int, PhoneMessage>();
            // Unread first, then newest first, like the inbox of the time.
            foreach (PhoneMessage message in _phoneService.GetMessages(phone).OrderBy(message => message.IsRead()))
            {
                messagesByRow[messagesDialog.AddRow(GetMessageLabel(phone, message))] = message;
            }

            void OnChosen(ListDialogResponse response)
            {
                Action reopenMessages = () => WithCarriedPhone(player, phone, () => OpenMessages(player, phone, onBack));
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (response.ItemIndex == writeRow)
                    OpenNumberInput(player, "Write a message", number => OpenWriteMessage(player, phone, number, reopenMessages), reopenMessages);
                else if (messagesByRow.TryGetValue(response.ItemIndex, out PhoneMessage? message))
                    OpenMessage(player, phone, message, reopenMessages);
            }

            _dialogService.ShowIgnoringDisconnect(player, messagesDialog, OnChosen);
        }

        private void OpenMessage(Player player, Phone phone, PhoneMessage message, Action onBack)
        {
            if (message.IsDeleted())
            {
                onBack();
                return;
            }

            _phoneEditorService.SetMessageRead(message);
            string direction = message.IsIncoming() ? "From" : "To";
            BetterMessageDialog messageDialog = new BetterMessageDialog("Options", "Back");
            messageDialog.SetTitle(TitleType.Children, GetTitle(phone), "Message");
            messageDialog.SetContent($"{direction}: {Highlight(_phoneService.GetNumberLabel(phone, message.GetOtherNumber()))}\n{ChatColor.LightGray}{GetTime(message.GetSentAt())}{ChatColor.White}\n\n{message.GetText()}");

            _dialogService.ShowIgnoringDisconnect(player, messageDialog, response =>
            {
                if (response.Response == DialogResponse.LeftButton)
                {
                    WithCarriedPhone(player, phone, () => OpenMessageActions(player, phone, message, onBack));
                    return;
                }
                onBack();
            });
        }

        private void OpenMessageActions(Player player, Phone phone, PhoneMessage message, Action onBack)
        {
            uint otherNumber = message.GetOtherNumber();
            bool isContact = _phoneService.GetContacts(phone).Any(contact => contact.GetNumber() == otherNumber);

            BetterListDialog actionsDialog = new BetterListDialog("Choose", "Back");
            actionsDialog.SetTitle(TitleType.Children, GetTitle(phone), "Message");
            int replyRow = actionsDialog.AddRow("Reply");
            int callRow = actionsDialog.AddRow("Call");
            int? saveRow = isContact ? null : actionsDialog.AddRow("Save the number");
            int deleteRow = actionsDialog.AddRow("Delete");

            void OnChosen(ListDialogResponse response)
            {
                Action reopenActions = () => WithCarriedPhone(player, phone, () => OpenMessageActions(player, phone, message, onBack));
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (response.ItemIndex == replyRow)
                    OpenWriteMessage(player, phone, otherNumber, onBack);
                else if (response.ItemIndex == callRow)
                    Dial(player, phone, otherNumber, reopenActions);
                else if (response.ItemIndex == saveRow)
                    OpenAddContact(player, phone, otherNumber, reopenActions);
                else if (response.ItemIndex == deleteRow)
                {
                    _phoneEditorService.DeleteMessage(message);
                    onBack();
                }
            }

            _dialogService.ShowIgnoringDisconnect(player, actionsDialog, OnChosen);
        }

        private void OpenWriteMessage(Player player, Phone phone, uint number, Action onBack)
        {
            void Send(string text)
            {
                WithCarriedPhone(player, phone, () =>
                {
                    string? error = _phoneEditorService.SendText(phone, number, text);
                    if (error == null)
                    {
                        _phoneRoleplayService.SendTextAction(player);
                    }
                    ContinueAfter(player, error, () => ShowMessage(player, "Message", "Message sent.", onBack), () => OpenWriteMessage(player, phone, number, onBack));
                });
            }

            OpenTextInput(player, "Write a message", $"To: {Highlight(_phoneService.GetNumberLabel(phone, number))}\n\nType your message (at most 160 characters):", Send, onBack);
        }

        private string GetMessageLabel(Phone phone, PhoneMessage message)
        {
            string marker = message.IsIncoming() && !message.IsRead() ? $"{ChatColor.Highlight}[New] {ChatColor.White}" : string.Empty;
            string direction = message.IsIncoming() ? "From" : "To";
            string other = _phoneService.GetNumberLabel(phone, message.GetOtherNumber());
            return $"{marker}{direction} {other}: {ChatColor.LightGray}{Shorten(message.GetText(), MessagePreviewLength)}";
        }
    }
}
