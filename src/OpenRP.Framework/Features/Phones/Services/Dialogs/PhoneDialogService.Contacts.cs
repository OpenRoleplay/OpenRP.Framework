using OpenRP.Framework.Features.Phones.Components;
using OpenRP.Framework.Features.Phones.Helpers;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Extensions;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Phones.Services.Dialogs
{
    public partial class PhoneDialogService
    {
        private void OpenContacts(Player player, Phone phone, Action onBack)
        {
            BetterListDialog contactsDialog = new BetterListDialog("Choose", "Back");
            contactsDialog.SetTitle(TitleType.Children, GetTitle(phone), "Phone book");
            int addRow = contactsDialog.AddRow($"{ChatColor.CornflowerBlue}Add a name");
            Dictionary<int, PhoneContact> contactsByRow = new Dictionary<int, PhoneContact>();
            foreach (PhoneContact contact in _phoneService.GetContacts(phone))
            {
                contactsByRow[contactsDialog.AddRow($"{contact.GetName()} {ChatColor.LightGray}({PhoneNumbers.Format(contact.GetNumber())})")] = contact;
            }

            void OnChosen(ListDialogResponse response)
            {
                Action reopenContacts = () => WithCarriedPhone(player, phone, () => OpenContacts(player, phone, onBack));
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (response.ItemIndex == addRow)
                    OpenAddContact(player, phone, null, reopenContacts);
                else if (contactsByRow.TryGetValue(response.ItemIndex, out PhoneContact? contact))
                    OpenContactActions(player, phone, contact, reopenContacts);
            }

            _dialogService.ShowIgnoringDisconnect(player, contactsDialog, OnChosen);
        }

        private void OpenContactActions(Player player, Phone phone, PhoneContact contact, Action onBack)
        {
            if (contact.IsDeleted())
            {
                onBack();
                return;
            }

            BetterListDialog actionsDialog = new BetterListDialog("Choose", "Back");
            actionsDialog.SetTitle(TitleType.Children, GetTitle(phone), contact.GetName());
            int callRow = actionsDialog.AddRow($"Call {Highlight(PhoneNumbers.Format(contact.GetNumber()))}");
            int textRow = actionsDialog.AddRow("Send a message");
            int renameRow = actionsDialog.AddRow("Rename");
            int deleteRow = actionsDialog.AddRow("Delete");

            void OnChosen(ListDialogResponse response)
            {
                Action reopenActions = () => WithCarriedPhone(player, phone, () => OpenContactActions(player, phone, contact, onBack));
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (response.ItemIndex == callRow)
                    Dial(player, phone, contact.GetNumber(), reopenActions);
                else if (response.ItemIndex == textRow)
                    OpenWriteMessage(player, phone, contact.GetNumber(), reopenActions);
                else if (response.ItemIndex == renameRow)
                    OpenRenameContact(player, phone, contact, reopenActions);
                else if (response.ItemIndex == deleteRow)
                    OpenDeleteContact(player, contact, onBack, reopenActions);
            }

            _dialogService.ShowIgnoringDisconnect(player, actionsDialog, OnChosen);
        }

        /// <summary>
        /// Asks for the name, then the number unless it's known already (saving a number from a text or the call log).
        /// </summary>
        private void OpenAddContact(Player player, Phone phone, uint? number, Action onBack)
        {
            void Add(string name, string numberText)
            {
                string? error = _phoneEditorService.AddContact(phone, name, numberText);
                ContinueAfter(player, error, onBack, () => OpenAddContact(player, phone, number, onBack));
            }

            void OnName(string name)
            {
                if (number.HasValue)
                {
                    Add(name, number.Value.ToString());
                    return;
                }
                OpenTextInput(player, "Add a name", $"Enter the number of {Highlight(name)}, for example 382-0142:", numberText => Add(name, numberText), onBack);
            }

            OpenTextInput(player, "Add a name", "Enter the name (at most 24 characters):", OnName, onBack);
        }

        private void OpenRenameContact(Player player, Phone phone, PhoneContact contact, Action onBack)
        {
            void Rename(string name)
            {
                string? error = _phoneEditorService.RenameContact(contact, name);
                ContinueAfter(player, error, onBack, () => OpenRenameContact(player, phone, contact, onBack));
            }

            OpenTextInput(player, "Rename", $"Enter the new name for {Highlight(contact.GetName())}:", Rename, onBack);
        }

        private void OpenDeleteContact(Player player, PhoneContact contact, Action onDeleted, Action onBack)
        {
            void Delete()
            {
                _phoneEditorService.DeleteContact(contact);
                onDeleted();
            }

            ShowConfirmation(player, "Delete", $"Delete {Highlight(contact.GetName())} from the phone book?", Delete, onBack);
        }
    }
}
