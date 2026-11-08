using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Helpers;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Characters.Services.Dialogs
{
    /// <summary>
    /// Character preferences: Hardcore Mode and the consent settings it locks.
    /// Changes are saved by the character persistence system.
    /// </summary>
    public partial class CharacterDialogService
    {
        private const string CharacterPreferencesTitle = "Character Preferences";

        private record PreferencesDialogRows(int HardcoreMode, int CharacterKill, int Rape, int Slavery, int WorldEvents);

        public void OpenCharacterPreferences(Player player)
        {
            CharacterPreferences? preferences = player.GetComponent<CharacterPreferences>();
            if (preferences == null)
            {
                return;
            }

            BetterTablistDialog preferencesDialog = CreateCharacterPreferencesDialog(preferences, out PreferencesDialogRows rows);

            void CharacterPreferencesDialogHandler(TablistDialogResponse response)
            {
                if (response.Response != DialogResponse.LeftButton)
                {
                    return;
                }

                OnCharacterPreferenceChosen(player, preferences, rows, response.ItemIndex);
            }

            _dialogService.Show(player, preferencesDialog, CharacterPreferencesDialogHandler);
        }

        private static BetterTablistDialog CreateCharacterPreferencesDialog(CharacterPreferences preferences, out PreferencesDialogRows rows)
        {
            BetterTablistDialog preferencesDialog = new BetterTablistDialog("Proceed", "Exit", 2);
            preferencesDialog.SetTitle(TitleType.Parents, CharacterPreferencesTitle);
            preferencesDialog.AddHeaders("Preference", "Value");

            preferencesDialog.AddHeaders("General");
            int hardcoreMode = preferencesDialog.AddRow("Hardcore Mode", DialogHelper.GetBooleanAsOnOrOff(preferences.IsHardcoreModeEnabled()));

            preferencesDialog.AddHeaders("Consent");
            int characterKill = preferencesDialog.AddRow("Allow Character Kill", DialogHelper.GetBooleanAsOnOrOff(preferences.IsCharacterKillAllowed()));
            int rape = preferencesDialog.AddRow("Allow Rape", DialogHelper.GetBooleanAsOnOrOff(preferences.IsRapeAllowed()));
            int slavery = preferencesDialog.AddRow("Allow Slavery", DialogHelper.GetBooleanAsOnOrOff(preferences.IsSlaveryAllowed()));
            int worldEvents = preferencesDialog.AddRow("Allow World Events", DialogHelper.GetBooleanAsOnOrOff(preferences.IsWorldEventsAllowed()));

            rows = new PreferencesDialogRows(hardcoreMode, characterKill, rape, slavery, worldEvents);
            return preferencesDialog;
        }

        private void OnCharacterPreferenceChosen(Player player, CharacterPreferences preferences, PreferencesDialogRows rows, int rowIndex)
        {
            if (rowIndex == rows.HardcoreMode)
            {
                OpenHardcoreModeInformation(player, preferences);
                return;
            }

            if (!IsConsentRow(rows, rowIndex))
            {
                // A header or spacer row was clicked.
                OpenCharacterPreferences(player);
                return;
            }

            if (!preferences.CanChangeConsent())
            {
                ShowConsentLockedMessage(player);
                return;
            }

            ToggleConsent(preferences, rows, rowIndex);
            OpenCharacterPreferences(player);
        }

        private static bool IsConsentRow(PreferencesDialogRows rows, int rowIndex)
        {
            return rowIndex == rows.CharacterKill
                || rowIndex == rows.Rape
                || rowIndex == rows.Slavery
                || rowIndex == rows.WorldEvents;
        }

        private static void ToggleConsent(CharacterPreferences preferences, PreferencesDialogRows rows, int rowIndex)
        {
            if (rowIndex == rows.CharacterKill)
            {
                preferences.SetCharacterKillAllowed(!preferences.IsCharacterKillAllowed());
            }
            else if (rowIndex == rows.Rape)
            {
                preferences.SetRapeAllowed(!preferences.IsRapeAllowed());
            }
            else if (rowIndex == rows.Slavery)
            {
                preferences.SetSlaveryAllowed(!preferences.IsSlaveryAllowed());
            }
            else if (rowIndex == rows.WorldEvents)
            {
                preferences.SetWorldEventsAllowed(!preferences.IsWorldEventsAllowed());
            }
        }

        private void ShowConsentLockedMessage(Player player)
        {
            string content = $"{ChatColor.CornflowerBlue}Hardcore Mode Active{ChatColor.White}\r\nHardcore Mode is currently enabled. As a result, you cannot change consent settings for Character Kills, Rape, Slavery, and World Events. Your preferences are locked to maintain a consistent and immersive roleplay experience.\r\n\r\n{ChatColor.Red}Warning: Changes to consent settings are disabled while Hardcore Mode is active. Hardcore Mode will remain active until your character is character killed. You cannot disable Hardcore Mode manually.{ChatColor.White}";

            ShowMessage(player, "Go Back", content, () => OpenCharacterPreferences(player), CharacterPreferencesTitle, "Consent");
        }

        private void OpenHardcoreModeInformation(Player player, CharacterPreferences preferences)
        {
            if (preferences.IsHardcoreModeEnabled())
            {
                ShowConsentLockedMessage(player);
                return;
            }

            string content = $"{ChatColor.CornflowerBlue}Hardcore Mode{ChatColor.White}\r\nHardcore Mode means you agree to all in-game interactions, such as character kills, slavery, and rape. By playing in Hardcore Mode, you help create a more dynamic roleplay environment and allow important story events that deepen your character's development.\r\n\r\nAs a reward, you gain the \"Blessed by Misfortune\" trait.\r\n\r\n{ChatColor.CornflowerBlue}Blessed by Misfortune{ChatColor.White}\r\nLuck shines on those who walk the path of danger. This trait gives you extra luck in many luck-based activities on Open Roleplay.\r\n\r\n{ChatColor.Red}Warning: Once you enable Hardcore Mode, it remains active until your character is character killed. It will automatically disable after that. So ask yourself this: are you fine with your character inevitably being killed in the future?{ChatColor.White}";

            ShowHardcoreModeQuestion(player, "Information", content, () => OpenHardcoreModeConfirmation(player, preferences));
        }

        private void OpenHardcoreModeConfirmation(Player player, CharacterPreferences preferences)
        {
            string content = $"{ChatColor.CornflowerBlue}Confirm Hardcore Mode Activation{ChatColor.White}\r\nAre you certain you want to enable Hardcore Mode? This means you consent to all in-game interactions, including character kills, slavery, and rape. By choosing Hardcore Mode, you contribute to a more dynamic roleplay environment and allow impactful story events that deeply affect your character's development.\r\n\r\n{ChatColor.CornflowerBlue}Important:{ChatColor.White} Once enabled, Hardcore Mode cannot be turned off manually. It will automatically disable only if your character is character killed.\r\n\r\n{ChatColor.Red}Warning: Activating Hardcore Mode commits your character to a path where their fate is sealed. Your character may face inevitable death in the future.{ChatColor.White}\r\n\r\nAre you sure that you wish to proceed?";

            ShowHardcoreModeQuestion(player, "Confirmation", content, () => ActivateHardcoreMode(player, preferences));
        }

        /// <summary>
        /// Shows a "Turn On" / "Go Back" question. Only "Turn On" continues; anything else returns to the preferences.
        /// </summary>
        private void ShowHardcoreModeQuestion(Player player, string step, string content, Action onTurnOn)
        {
            BetterMessageDialog questionDialog = new BetterMessageDialog("Turn On", "Go Back");
            questionDialog.SetTitle(TitleType.Children, CharacterPreferencesTitle, "Hardcore Mode", step);
            questionDialog.SetContent(content);

            void HardcoreModeQuestionDialogHandler(MessageDialogResponse response)
            {
                if (response.Response != DialogResponse.LeftButton)
                {
                    OpenCharacterPreferences(player);
                    return;
                }

                onTurnOn();
            }

            _dialogService.Show(player, questionDialog, HardcoreModeQuestionDialogHandler);
        }

        private void ActivateHardcoreMode(Player player, CharacterPreferences preferences)
        {
            preferences.EnableHardcoreMode();

            string content = $"{ChatColor.CornflowerBlue}Hardcore Mode Activated{ChatColor.White}\r\nYou have successfully activated Hardcore Mode. This means you consent to all in-game interactions, including character kills, slavery, and rape. By choosing Hardcore Mode, you contribute to a more dynamic roleplay environment and allow impactful story events that will deeply affect your character's development.\r\n\r\nAs a reward, you have gained the Blessed by Misfortune trait.\r\n\r\n{ChatColor.CornflowerBlue}Blessed by Misfortune{ChatColor.White}\r\nLuck shines on those who walk the path of danger. This trait gives you extra luck in many luck-based activities on Open Roleplay.\r\n\r\n{ChatColor.Red}Warning: Hardcore Mode cannot be turned off manually. It will automatically disable only if your character is character killed. Your character may face inevitable death in the future.{ChatColor.White}\r\n\r\nProceed with caution and embrace the challenges that lie ahead.";

            ShowMessage(player, "Go Back", content, () => OpenCharacterPreferences(player), CharacterPreferencesTitle, "Hardcore Mode", "Hardcore Mode Activated");
        }
    }
}
