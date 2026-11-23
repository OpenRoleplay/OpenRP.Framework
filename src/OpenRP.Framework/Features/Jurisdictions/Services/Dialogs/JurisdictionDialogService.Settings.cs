using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Jurisdictions.Components;
using OpenRP.Framework.Features.Jurisdictions.Entities;
using OpenRP.Framework.Features.Jurisdictions.Enums;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Extensions;
using OpenRP.Framework.Shared.Dialogs.Helpers;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Jurisdictions.Services.Dialogs
{
    public partial class JurisdictionDialogService
    {
        private const string GovernmentTitle = "Government";
        private const string SettingsTitle = "Settings";
        private const string NoLongerGovernsText = "You no longer lead the government of this jurisdiction, so you can't change its settings.";

        public void OpenGovernment(Player player)
        {
            if (!player.IsPlayerPlayingAsCharacter())
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You must be playing as a character in order to do this command!");
                return;
            }

            List<JurisdictionTreeRow> governedRows = GetGovernedRows(player.GetPlayerCurrentlyPlayingAsCharacter());
            if (governedRows.Count == 0)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You don't lead the government of any jurisdiction.");
                return;
            }

            if (governedRows.Count == 1)
            {
                OpenGovernedSettings(player, governedRows[0].Jurisdiction, () => { });
                return;
            }

            OpenGovernmentPicker(player, governedRows, () => { });
        }

        /// <summary>
        /// The settings for an admin, who may change any jurisdiction.
        /// </summary>
        public void OpenJurisdictionSettings(Player player, Jurisdiction jurisdiction, Action onBack)
        {
            OpenSettings(player, jurisdiction, onBack, () => true);
        }

        private List<JurisdictionTreeRow> GetGovernedRows(Character character)
        {
            return _jurisdictionService.GetJurisdictionTree()
                .Where(row => _jurisdictionSettingService.CanGovern(character, row.Jurisdiction))
                .ToList();
        }

        private void OpenGovernmentPicker(Player player, List<JurisdictionTreeRow> governedRows, Action onBack)
        {
            BetterListDialog pickerDialog = new BetterListDialog("Choose", "Back");
            pickerDialog.SetTitle(TitleType.Children, GovernmentTitle);
            Dictionary<int, Jurisdiction> jurisdictionsByRow = new Dictionary<int, Jurisdiction>();
            foreach (JurisdictionTreeRow governedRow in governedRows)
            {
                jurisdictionsByRow[pickerDialog.AddRow(GetTreeLabel(governedRow.Jurisdiction, governedRow.Depth))] = governedRow.Jurisdiction;
            }

            _dialogService.ShowIgnoringDisconnect(player, pickerDialog, response =>
            {
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (jurisdictionsByRow.TryGetValue(response.ItemIndex, out Jurisdiction? jurisdiction))
                    OpenGovernedSettings(player, jurisdiction, () => OpenGovernmentPicker(player, governedRows, onBack));
            });
        }

        private void OpenGovernedSettings(Player player, Jurisdiction jurisdiction, Action onBack)
        {
            OpenSettings(player, jurisdiction, onBack, () => CanPlayerGovern(player, jurisdiction));
        }

        private bool CanPlayerGovern(Player player, Jurisdiction jurisdiction)
        {
            return player.IsPlayerPlayingAsCharacter()
                && _jurisdictionSettingService.CanGovern(player.GetPlayerCurrentlyPlayingAsCharacter(), jurisdiction);
        }

        /// <summary>
        /// The settings of the jurisdiction as a table. canEdit is checked again before every change.
        /// </summary>
        private void OpenSettings(Player player, Jurisdiction jurisdiction, Action onBack, Func<bool> canEdit)
        {
            BetterTablistDialog settingsDialog = new BetterTablistDialog("Choose", "Back", 3);
            settingsDialog.SetTitle(TitleType.Children, JurisdictionsTitle, jurisdiction.GetName(), SettingsTitle);
            settingsDialog.AddHeaders("Setting", "Value", "Sub-jurisdictions");
            Dictionary<int, IJurisdictionSetting> settingsByRow = new Dictionary<int, IJurisdictionSetting>();
            foreach (IJurisdictionSetting setting in _jurisdictionSettingService.GetSettings())
            {
                settingsByRow[AddSettingRow(settingsDialog, jurisdiction, setting)] = setting;
            }

            _dialogService.ShowIgnoringDisconnect(player, settingsDialog, response =>
            {
                Action reopenSettings = () => OpenSettings(player, jurisdiction, onBack, canEdit);
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (settingsByRow.TryGetValue(response.ItemIndex, out IJurisdictionSetting? setting))
                    OpenSettingActions(player, jurisdiction, setting, reopenSettings, canEdit);
                else
                    reopenSettings();
            });
        }

        private int AddSettingRow(BetterTablistDialog settingsDialog, Jurisdiction jurisdiction, IJurisdictionSetting setting)
        {
            JurisdictionSettingState state = _jurisdictionSettingService.GetState(jurisdiction, setting);
            return settingsDialog.AddRow(setting.Name, GetValueLabel(state), GetSubJurisdictionsLabel(jurisdiction, setting, state));
        }

        private static string GetValueLabel(JurisdictionSettingState state)
        {
            string? sourceLabel = GetSourceLabel(state);
            return sourceLabel == null ? state.ValueText : $"{state.ValueText} {ChatColor.LightGray}{sourceLabel}";
        }

        private static string? GetSourceLabel(JurisdictionSettingState state)
        {
            return state.Source switch
            {
                JurisdictionSettingSource.Inherited => $"(from {state.SourceJurisdiction?.GetName()})",
                JurisdictionSettingSource.LimitedByParent => $"(limited by {state.SourceJurisdiction?.GetName()})",
                JurisdictionSettingSource.Default => "(default)",
                _ => null
            };
        }

        /// <summary>
        /// A country says whether it binds its sub-jurisdictions. Any other place says whether its country binds it.
        /// </summary>
        private string GetSubJurisdictionsLabel(Jurisdiction jurisdiction, IJurisdictionSetting setting, JurisdictionSettingState state)
        {
            if (jurisdiction.IsCountry())
            {
                return state.BindsSubJurisdictions ? "Bound" : "Free";
            }
            return _jurisdictionSettingService.IsBound(jurisdiction, setting) ? "Bound by country" : "-";
        }

        private void OpenSettingActions(Player player, Jurisdiction jurisdiction, IJurisdictionSetting setting, Action reopenSettings, Func<bool> canEdit)
        {
            JurisdictionSettingState state = _jurisdictionSettingService.GetState(jurisdiction, setting);
            string inheritLabel = jurisdiction.IsCountry() ? "Use default" : "Use parent's value";

            BetterListDialog actionsDialog = new BetterListDialog("Choose", "Back");
            actionsDialog.SetTitle(TitleType.Children, JurisdictionsTitle, jurisdiction.GetName(), setting.Name);
            int changeRow = actionsDialog.AddRow("Change value");
            int? clearRow = state.HasOwnValue ? actionsDialog.AddRow(inheritLabel) : null;
            int? bindRow = jurisdiction.IsCountry()
                ? actionsDialog.AddRow($"Bind sub-jurisdictions: {DialogHelper.GetBooleanAsOnOrOff(state.BindsSubJurisdictions)}")
                : null;

            _dialogService.ShowIgnoringDisconnect(player, actionsDialog, response =>
            {
                Action reopenActions = () => OpenSettingActions(player, jurisdiction, setting, reopenSettings, canEdit);
                if (response.Response != DialogResponse.LeftButton)
                    reopenSettings();
                else if (response.ItemIndex == changeRow)
                    OpenChangeValue(player, jurisdiction, setting, reopenSettings, reopenActions, canEdit);
                else if (response.ItemIndex == clearRow)
                    ClearOwnValue(player, jurisdiction, setting, reopenSettings, canEdit);
                else if (response.ItemIndex == bindRow)
                    // The row showed the binding as it was when the menu opened; the player chose its opposite.
                    SetBinding(player, jurisdiction, setting, !state.BindsSubJurisdictions, reopenActions, reopenSettings, canEdit);
                else
                    reopenSettings();
            });
        }

        private void OpenChangeValue(Player player, Jurisdiction jurisdiction, IJurisdictionSetting setting, Action reopenSettings, Action reopenActions, Func<bool> canEdit)
        {
            Action reopenInput = () => OpenChangeValue(player, jurisdiction, setting, reopenSettings, reopenActions, canEdit);
            OpenTextInput(player, setting.Name, setting.Prompt,
                input => SaveValue(player, jurisdiction, setting, input, reopenSettings, reopenInput, canEdit), reopenActions);
        }

        private void SaveValue(Player player, Jurisdiction jurisdiction, IJurisdictionSetting setting, string input, Action reopenSettings, Action reopenInput, Func<bool> canEdit)
        {
            WithEditPermission(player, canEdit, reopenSettings, () =>
                ContinueAfter(player, _jurisdictionSettingService.SetValue(jurisdiction, setting, input), reopenSettings, reopenInput));
        }

        private void ClearOwnValue(Player player, Jurisdiction jurisdiction, IJurisdictionSetting setting, Action reopenSettings, Func<bool> canEdit)
        {
            WithEditPermission(player, canEdit, reopenSettings, () =>
                ContinueAfter(player, _jurisdictionSettingService.ClearValue(jurisdiction, setting), reopenSettings, reopenSettings));
        }

        private void SetBinding(Player player, Jurisdiction country, IJurisdictionSetting setting, bool binds, Action reopenActions, Action reopenSettings, Func<bool> canEdit)
        {
            WithEditPermission(player, canEdit, reopenSettings, () =>
                ContinueAfter(player, _jurisdictionSettingService.SetBindsSubJurisdictions(country, setting, binds), reopenActions, reopenActions));
        }

        /// <summary>
        /// Runs the change while the player may still change the jurisdiction. Otherwise tells them and goes back to the settings.
        /// </summary>
        private void WithEditPermission(Player player, Func<bool> canEdit, Action reopenSettings, Action change)
        {
            if (!canEdit())
            {
                ShowMessage(player, "Not allowed", NoLongerGovernsText, reopenSettings);
                return;
            }
            change();
        }
    }
}
