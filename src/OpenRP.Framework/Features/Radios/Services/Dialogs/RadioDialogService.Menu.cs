using OpenRP.Framework.Features.Radios.Components;
using OpenRP.Framework.Features.Radios.Helpers;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Extensions;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Radios.Services.Dialogs
{
    public partial class RadioDialogService
    {
        public void OpenRadio(Player player)
        {
            List<Radio> radios = _radioService.GetUsableRadios(player);
            if (radios.Count == 0)
            {
                player.SendClientMessage($"{ChatColor.White}You don't have a radio, and the vehicle you're in has none.");
                return;
            }
            if (radios.Count == 1)
            {
                OpenRadioMenu(player, radios[0]);
                return;
            }
            OpenRadioPicker(player, radios);
        }

        public void OpenRadio(Player player, Radio radio)
        {
            WithUsableRadio(player, radio, () => OpenRadioMenu(player, radio));
        }

        private void OpenRadioPicker(Player player, List<Radio> radios)
        {
            BetterListDialog pickerDialog = new BetterListDialog("Choose", "Close");
            pickerDialog.SetTitle(TitleType.Children, RadioTitle, "Which radio?");
            Dictionary<int, Radio> radiosByRow = new Dictionary<int, Radio>();
            foreach (Radio radio in radios)
            {
                radiosByRow[pickerDialog.AddRow(GetRadioLabel(radio))] = radio;
            }

            _dialogService.ShowIgnoringDisconnect(player, pickerDialog, response =>
            {
                if (response.Response == DialogResponse.LeftButton && radiosByRow.TryGetValue(response.ItemIndex, out Radio? radio))
                {
                    WithUsableRadio(player, radio, () => OpenRadioMenu(player, radio));
                }
            });
        }

        private void OpenRadioMenu(Player player, Radio radio)
        {
            bool canChooseEarpiece = !radio.IsVehicleRadio() && _radioService.HasEarpiece(player);

            BetterListDialog menuDialog = new BetterListDialog("Choose", "Close");
            menuDialog.SetTitle(TitleType.Children, RadioTitle, radio.IsVehicleRadio() ? "Vehicle radio" : "Handheld radio");
            int powerRow = menuDialog.AddRow($"Power: {Highlight(radio.IsOn() ? "On" : "Off")}");
            int frequencyRow = menuDialog.AddRow($"Frequency: {Highlight(_radioFrequencyService.GetLabel(radio.GetFrequency()))}");
            int? earpieceRow = canChooseEarpiece ? menuDialog.AddRow($"Listen through: {Highlight(radio.UsesEarpiece() ? "Earpiece" : "Speaker")}") : null;

            void OnChosen(ListDialogResponse response)
            {
                Action reopenMenu = () => WithUsableRadio(player, radio, () => OpenRadioMenu(player, radio));
                if (response.Response != DialogResponse.LeftButton)
                    return;
                if (response.ItemIndex == powerRow)
                    WithUsableRadio(player, radio, () => TogglePower(player, radio, reopenMenu));
                else if (response.ItemIndex == frequencyRow)
                    OpenTune(player, radio, reopenMenu);
                else if (response.ItemIndex == earpieceRow)
                    WithUsableRadio(player, radio, () => ToggleEarpiece(player, radio, reopenMenu));
            }

            _dialogService.ShowIgnoringDisconnect(player, menuDialog, OnChosen);
        }

        private void TogglePower(Player player, Radio radio, Action onBack)
        {
            _radioEditorService.SetPower(radio, !radio.IsOn());
            player.SendClientMessage(radio.IsOn()
                ? $"{ChatColor.White}[Radio] Your radio is on, tuned to {_radioFrequencyService.GetLabel(radio.GetFrequency())}."
                : $"{ChatColor.White}[Radio] Your radio is off.");
            onBack();
        }

        private void ToggleEarpiece(Player player, Radio radio, Action onBack)
        {
            _radioEditorService.SetUsesEarpiece(radio, !radio.UsesEarpiece());
            player.SendClientMessage(radio.UsesEarpiece()
                ? $"{ChatColor.White}[Radio] You listen through your earpiece. People around you no longer hear your radio."
                : $"{ChatColor.White}[Radio] Your radio plays through its speaker. People around you can hear it.");
            onBack();
        }

        private void OpenTune(Player player, Radio radio, Action onBack)
        {
            void Tune(string frequencyText)
            {
                WithUsableRadio(player, radio, () =>
                {
                    string? error = _radioEditorService.SetFrequency(radio, frequencyText);
                    ContinueAfter(player, RadioTitle, error, () => SendTunedMessage(player, radio, onBack), () => OpenTune(player, radio, onBack));
                });
            }

            string prompt = $"Enter a frequency from {RadioFrequencyNumbers.Lowest} to {RadioFrequencyNumbers.Highest}. Anyone can use {RadioFrequencyNumbers.LowestOpen} to {RadioFrequencyNumbers.HighestOpen}.";
            OpenTextInput(player, RadioTitle, "Frequency", prompt, Tune, onBack);
        }

        private void SendTunedMessage(Player player, Radio radio, Action onBack)
        {
            player.SendClientMessage($"{ChatColor.White}[Radio] Tuned to {_radioFrequencyService.GetLabel(radio.GetFrequency())}.");
            onBack();
        }

        /// <summary>
        /// Runs the action when the player can still use the radio; they may have dropped it or left the vehicle while a
        /// menu was open.
        /// </summary>
        private void WithUsableRadio(Player player, Radio radio, Action action)
        {
            if (!_radioService.CanUse(player, radio))
            {
                ShowMessage(player, RadioTitle, "Radio", "You can't reach that radio anymore.", () => { });
                return;
            }
            action();
        }

        private string GetRadioLabel(Radio radio)
        {
            string kind = radio.IsVehicleRadio() ? "Vehicle radio" : "Handheld radio";
            string state = radio.IsOn() ? _radioFrequencyService.GetLabel(radio.GetFrequency()) : "off";
            return $"{kind} {ChatColor.LightGray}({state})";
        }

        private static string Highlight(string text)
        {
            return $"{ChatColor.Highlight}{text}{ChatColor.White}";
        }
    }
}
