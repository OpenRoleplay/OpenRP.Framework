using OpenRP.Framework.Features.Phones.Components;
using OpenRP.Framework.Features.Phones.Helpers;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Batteries.Helpers;
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
        public void OpenPhone(Player player)
        {
            List<Phone> phones = _phoneService.GetCarriedPhones(player);
            if (phones.Count == 0)
            {
                player.SendClientMessage($"{ChatColor.White}You don't have a phone.");
                return;
            }
            if (phones.Count == 1)
            {
                OpenPhoneMenu(player, phones[0]);
                return;
            }
            OpenPhonePicker(player, phones);
        }

        private void OpenPhonePicker(Player player, List<Phone> phones)
        {
            BetterListDialog pickerDialog = new BetterListDialog("Choose", "Close");
            pickerDialog.SetTitle(TitleType.Children, PhoneTitle, "Which phone?");
            Dictionary<int, Phone> phonesByRow = new Dictionary<int, Phone>();
            foreach (Phone phone in phones)
            {
                phonesByRow[pickerDialog.AddRow(GetPhoneLabel(phone))] = phone;
            }

            _dialogService.ShowIgnoringDisconnect(player, pickerDialog, response =>
            {
                if (response.Response == DialogResponse.LeftButton && phonesByRow.TryGetValue(response.ItemIndex, out Phone? phone))
                {
                    WithCarriedPhone(player, phone, () => OpenPhoneMenu(player, phone));
                }
            });
        }

        private void OpenPhoneMenu(Player player, Phone phone)
        {
            if (!phone.IsOn())
            {
                OpenTurnedOffMenu(player, phone);
                return;
            }

            uint? lastDialedNumber = _phoneService.GetLastDialedNumber(phone);
            int unreadMessages = _phoneService.GetUnreadMessageCount(phone);
            int missedCalls = _phoneService.GetNewMissedCallCount(phone);

            BetterListDialog menuDialog = new BetterListDialog("Choose", "Close");
            menuDialog.SetTitle(TitleType.Children, GetTitle(phone));
            int dialRow = menuDialog.AddRow("Dial a number");
            int? redialRow = lastDialedNumber.HasValue ? menuDialog.AddRow($"Redial {Highlight(_phoneService.GetNumberLabel(phone, lastDialedNumber.Value))}") : null;
            int contactsRow = menuDialog.AddRow($"Phone book {ChatColor.LightGray}({_phoneService.GetContacts(phone).Count})");
            int messagesRow = menuDialog.AddRow(unreadMessages > 0 ? $"Messages {ChatColor.Highlight}({unreadMessages} new)" : "Messages");
            int callLogRow = menuDialog.AddRow(missedCalls > 0 ? $"Call log {ChatColor.Highlight}({missedCalls} missed)" : "Call log");
            int ringerRow = menuDialog.AddRow($"Ringer: {Highlight(phone.IsSilent() ? "Silent" : "On")}");
            int numberRow = menuDialog.AddRow($"My number: {Highlight(PhoneNumbers.Format(phone.GetNumber()))}");
            int batteryRow = menuDialog.AddRow(GetBatteryLabel(phone));
            int powerRow = menuDialog.AddRow("Turn off");

            void OnChosen(ListDialogResponse response)
            {
                Action reopenMenu = () => WithCarriedPhone(player, phone, () => OpenPhoneMenu(player, phone));
                if (response.Response != DialogResponse.LeftButton)
                    return;
                if (response.ItemIndex == dialRow)
                    OpenNumberInput(player, "Dial", number => Dial(player, phone, number, reopenMenu), reopenMenu);
                else if (response.ItemIndex == redialRow && lastDialedNumber.HasValue)
                    Dial(player, phone, lastDialedNumber.Value, reopenMenu);
                else if (response.ItemIndex == contactsRow)
                    WithCarriedPhone(player, phone, () => OpenContacts(player, phone, reopenMenu));
                else if (response.ItemIndex == messagesRow)
                    WithCarriedPhone(player, phone, () => OpenMessages(player, phone, reopenMenu));
                else if (response.ItemIndex == callLogRow)
                    WithCarriedPhone(player, phone, () => OpenCallLog(player, phone, reopenMenu));
                else if (response.ItemIndex == ringerRow)
                    WithCarriedPhone(player, phone, () => ToggleRinger(phone, reopenMenu));
                else if (response.ItemIndex == numberRow)
                    ShowMessage(player, "My number", $"This phone's number is {Highlight(PhoneNumbers.Format(phone.GetNumber()))}.", reopenMenu);
                else if (response.ItemIndex == batteryRow)
                    WithCarriedPhone(player, phone, () => Recharge(player, phone, reopenMenu));
                else if (response.ItemIndex == powerRow)
                    WithCarriedPhone(player, phone, () => TurnOff(player, phone));
            }

            _dialogService.ShowIgnoringDisconnect(player, menuDialog, OnChosen);
        }

        /// <summary>
        /// A phone that's off only turns on.
        /// </summary>
        private void OpenTurnedOffMenu(Player player, Phone phone)
        {
            BetterListDialog offDialog = new BetterListDialog("Choose", "Close");
            offDialog.SetTitle(TitleType.Children, GetTitle(phone), "Off");
            int powerRow = offDialog.AddRow("Turn on");
            int batteryRow = offDialog.AddRow(GetBatteryLabel(phone));

            _dialogService.ShowIgnoringDisconnect(player, offDialog, response =>
            {
                if (response.Response == DialogResponse.LeftButton && response.ItemIndex == powerRow)
                {
                    WithCarriedPhone(player, phone, () => TurnOn(player, phone));
                }
                else if (response.Response == DialogResponse.LeftButton && response.ItemIndex == batteryRow)
                {
                    WithCarriedPhone(player, phone, () => Recharge(player, phone, () => OpenPhoneMenu(player, phone)));
                }
            });
        }

        private void TurnOn(Player player, Phone phone)
        {
            string? error = _phoneEditorService.SetPower(phone, true);
            if (error != null)
            {
                ShowMessage(player, "Turn on", error, () => OpenPhoneMenu(player, phone));
                return;
            }
            string? news = _phoneService.GetNewsSummary(phone);
            player.SendClientMessage($"{ChatColor.White}[Phone] Your phone is on.{(news == null ? string.Empty : $" {news}.")}");
            OpenPhoneMenu(player, phone);
        }

        /// <summary>
        /// A call on the phone is cut off within a second by the call timer.
        /// </summary>
        private void TurnOff(Player player, Phone phone)
        {
            _phoneEditorService.SetPower(phone, false);
            player.SendClientMessage($"{ChatColor.White}[Phone] Your phone is off. Calls can't reach it, but texts wait until it's on.");
        }

        /// <summary>
        /// Puts the phone on charge. It stays usable, and is full after at most a minute.
        /// </summary>
        private void Recharge(Player player, Phone phone, Action onBack)
        {
            string? error = _phoneBatteryService.BeginRecharge(player, phone);
            if (error != null)
            {
                ShowMessage(player, "Battery", error, onBack);
                return;
            }

            int seconds = (int)Math.Ceiling(phone.GetCharge()!.Duration.TotalSeconds);
            player.SendClientMessage($"{ChatColor.White}[Phone] You put your phone on charge. It's full in {seconds} seconds.");
            onBack();
        }

        private string GetBatteryLabel(Phone phone)
        {
            string charging = phone.IsCharging() ? $" {ChatColor.LightGray}(charging)" : string.Empty;
            return $"Battery: {Highlight(BatteryLevels.Format(_phoneBatteryService.GetCurrentLevel(phone)))}{charging}";
        }

        private void ToggleRinger(Phone phone, Action onBack)
        {
            _phoneEditorService.SetSilent(phone, !phone.IsSilent());
            onBack();
        }

        private string GetPhoneLabel(Phone phone)
        {
            string status = phone.IsOn() ? _phoneService.GetNewsSummary(phone) ?? "on" : "off";
            return $"{PhoneNumbers.Format(phone.GetNumber())} {ChatColor.LightGray}({status})";
        }
    }
}
