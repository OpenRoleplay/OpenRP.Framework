using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Features.Radios.Components;
using OpenRP.Framework.Features.Radios.Enums;
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
        public void OpenFrequencyEditor(Player player)
        {
            BetterListDialog frequenciesDialog = new BetterListDialog("Choose", "Close");
            frequenciesDialog.SetTitle(TitleType.Children, FrequenciesTitle);
            int createRow = frequenciesDialog.AddRow($"{ChatColor.CornflowerBlue}Create a frequency");
            Dictionary<int, RadioFrequency> frequenciesByRow = new Dictionary<int, RadioFrequency>();
            foreach (RadioFrequency frequency in _radioFrequencyService.GetFrequencies())
            {
                frequenciesByRow[frequenciesDialog.AddRow(GetFrequencyLabel(frequency))] = frequency;
            }

            void OnFrequencyChosen(ListDialogResponse response)
            {
                Action reopenEditor = () => OpenFrequencyEditor(player);
                if (response.Response != DialogResponse.LeftButton)
                    return;
                if (response.ItemIndex == createRow)
                    OpenCreateFrequency(player, reopenEditor);
                else if (frequenciesByRow.TryGetValue(response.ItemIndex, out RadioFrequency? frequency))
                    OpenFrequencyActions(player, frequency, reopenEditor);
            }

            _dialogService.ShowIgnoringDisconnect(player, frequenciesDialog, OnFrequencyChosen);
        }

        private void OpenFrequencyActions(Player player, RadioFrequency frequency, Action onBack)
        {
            if (frequency.IsDeleted())
            {
                ShowMessage(player, FrequenciesTitle, frequency.GetName(), "This frequency has been deleted.", onBack);
                return;
            }

            BetterListDialog actionsDialog = new BetterListDialog("Choose", "Back");
            actionsDialog.SetTitle(TitleType.Children, FrequenciesTitle, $"{frequency.GetNumber()} {frequency.GetName()}");
            int renameRow = actionsDialog.AddRow("Rename");
            int modeRow = actionsDialog.AddRow($"Mode: {ChatColor.Highlight}{GetModeLabel(frequency.GetMode())}");
            int factionsRow = actionsDialog.AddRow($"Factions ({_radioFrequencyService.GetFrequencyFactions(frequency).Count})");
            int deleteRow = actionsDialog.AddRow("Delete");

            void OnActionChosen(ListDialogResponse response)
            {
                Action reopenActions = () => OpenFrequencyActions(player, frequency, onBack);
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (response.ItemIndex == renameRow)
                    OpenRenameFrequency(player, frequency, reopenActions);
                else if (response.ItemIndex == modeRow)
                    OpenModePicker(player, mode => SetMode(frequency, mode, reopenActions), reopenActions);
                else if (response.ItemIndex == factionsRow)
                    OpenFrequencyFactions(player, frequency, reopenActions);
                else if (response.ItemIndex == deleteRow)
                    OpenDeleteFrequency(player, frequency, onBack, reopenActions);
            }

            _dialogService.ShowIgnoringDisconnect(player, actionsDialog, OnActionChosen);
        }

        /// <summary>
        /// Asks for the number, then the name, then the mode.
        /// </summary>
        private void OpenCreateFrequency(Player player, Action onBack)
        {
            string prompt = $"Enter the frequency, from {RadioFrequencyNumbers.Lowest} to {RadioFrequencyNumbers.Highest}. Without rules, {RadioFrequencyNumbers.LowestOpen} to {RadioFrequencyNumbers.HighestOpen} are open to everyone and other numbers have no signal.";
            OpenTextInput(player, FrequenciesTitle, "Create", prompt, numberText => OpenCreateFrequencyName(player, numberText, onBack), onBack);
        }

        private void OpenCreateFrequencyName(Player player, string numberText, Action onBack)
        {
            void OnNamed(string name)
            {
                OpenModePicker(player, mode => CreateFrequency(player, numberText, name, mode, onBack), () => OpenCreateFrequencyName(player, numberText, onBack));
            }

            OpenTextInput(player, FrequenciesTitle, "Create", $"Enter a name for frequency {ChatColor.Highlight}{numberText.Trim()}{ChatColor.White}, for example 911 Dispatch:", OnNamed, onBack);
        }

        private void CreateFrequency(Player player, string numberText, string name, RadioFrequencyMode mode, Action onBack)
        {
            (RadioFrequency? frequency, string? error) = _radioEditorService.CreateFrequency(numberText, name, mode);
            ContinueAfter(player, FrequenciesTitle, error, () => OpenFrequencyActions(player, frequency!, onBack), () => OpenCreateFrequency(player, onBack));
        }

        private void OpenModePicker(Player player, Action<RadioFrequencyMode> onPicked, Action onBack)
        {
            BetterListDialog modeDialog = new BetterListDialog("Choose", "Back");
            modeDialog.SetTitle(TitleType.Children, FrequenciesTitle, "Mode");
            int protectedRow = modeDialog.AddRow($"Protected {ChatColor.LightGray}(everyone listens, its factions talk)");
            int encryptedRow = modeDialog.AddRow($"Encrypted {ChatColor.LightGray}(only its factions listen and talk)");

            _dialogService.ShowIgnoringDisconnect(player, modeDialog, response =>
            {
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (response.ItemIndex == protectedRow)
                    onPicked(RadioFrequencyMode.Protected);
                else if (response.ItemIndex == encryptedRow)
                    onPicked(RadioFrequencyMode.Encrypted);
            });
        }

        private void SetMode(RadioFrequency frequency, RadioFrequencyMode mode, Action onBack)
        {
            _radioEditorService.SetFrequencyMode(frequency, mode);
            onBack();
        }

        private void OpenRenameFrequency(Player player, RadioFrequency frequency, Action onBack)
        {
            void Rename(string name)
            {
                string? error = _radioEditorService.RenameFrequency(frequency, name);
                ContinueAfter(player, FrequenciesTitle, error, onBack, () => OpenRenameFrequency(player, frequency, onBack));
            }

            OpenTextInput(player, FrequenciesTitle, "Rename", $"Enter the new name for {ChatColor.Highlight}{frequency.GetName()}{ChatColor.White}:", Rename, onBack);
        }

        private void OpenFrequencyFactions(Player player, RadioFrequency frequency, Action onBack)
        {
            BetterListDialog factionsDialog = new BetterListDialog("Choose", "Back");
            factionsDialog.SetTitle(TitleType.Children, FrequenciesTitle, $"{frequency.GetNumber()} factions");
            int addRow = factionsDialog.AddRow($"{ChatColor.CornflowerBlue}Add a faction");
            Dictionary<int, RadioFrequencyFaction> frequencyFactionsByRow = new Dictionary<int, RadioFrequencyFaction>();
            foreach (RadioFrequencyFaction frequencyFaction in _radioFrequencyService.GetFrequencyFactions(frequency))
            {
                frequencyFactionsByRow[factionsDialog.AddRow(GetFactionName(frequencyFaction))] = frequencyFaction;
            }

            void OnFactionChosen(ListDialogResponse response)
            {
                Action reopenFactions = () => OpenFrequencyFactions(player, frequency, onBack);
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (response.ItemIndex == addRow)
                    _factionDialogService.OpenFactionPicker(player, $"Add to {frequency.GetNumber()}", null, faction => AddFaction(player, frequency, faction!, reopenFactions), reopenFactions);
                else if (frequencyFactionsByRow.TryGetValue(response.ItemIndex, out RadioFrequencyFaction? frequencyFaction))
                    OpenRemoveFaction(player, frequencyFaction, reopenFactions);
            }

            _dialogService.ShowIgnoringDisconnect(player, factionsDialog, OnFactionChosen);
        }

        private void AddFaction(Player player, RadioFrequency frequency, Faction faction, Action onBack)
        {
            string? error = _radioEditorService.AddFrequencyFaction(frequency, faction);
            ContinueAfter(player, FrequenciesTitle, error, onBack, onBack);
        }

        private void OpenRemoveFaction(Player player, RadioFrequencyFaction frequencyFaction, Action onBack)
        {
            void Remove()
            {
                _radioEditorService.RemoveFrequencyFaction(frequencyFaction);
                onBack();
            }

            ShowConfirmation(player, FrequenciesTitle, "Remove faction", $"Remove {ChatColor.Highlight}{GetFactionName(frequencyFaction)}{ChatColor.White} from this frequency?", Remove, onBack);
        }

        private void OpenDeleteFrequency(Player player, RadioFrequency frequency, Action onDeleted, Action onBack)
        {
            void Delete()
            {
                _radioEditorService.DeleteFrequency(frequency);
                onDeleted();
            }

            string question = $"Are you sure you want to delete {ChatColor.Highlight}{frequency.GetNumber()} {frequency.GetName()}{ChatColor.White}? Its number goes back to the default rules.";
            ShowConfirmation(player, FrequenciesTitle, "Delete frequency", question, Delete, onBack);
        }

        private string GetFrequencyLabel(RadioFrequency frequency)
        {
            int factionCount = _radioFrequencyService.GetFrequencyFactions(frequency).Count;
            return $"{frequency.GetNumber()} {frequency.GetName()} {ChatColor.LightGray}({GetModeLabel(frequency.GetMode())}, {factionCount} {(factionCount == 1 ? "faction" : "factions")}){ChatColor.White}";
        }

        private string GetFactionName(RadioFrequencyFaction frequencyFaction)
        {
            return _radioFrequencyService.GetFaction(frequencyFaction)?.GetName() ?? $"Faction #{frequencyFaction.GetFactionId()}";
        }

        private static string GetModeLabel(RadioFrequencyMode mode)
        {
            return mode == RadioFrequencyMode.Encrypted ? "Encrypted" : "Protected";
        }
    }
}
