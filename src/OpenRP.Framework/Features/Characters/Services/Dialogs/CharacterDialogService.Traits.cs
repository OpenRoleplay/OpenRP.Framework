using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using SampSharp.Entities.SAMP;
using OpenRP.Framework.Shared.Dialogs.Extensions;

namespace OpenRP.Framework.Features.Characters.Services.Dialogs
{
    /// <summary>
    /// Character traits. The only trait so far is "Blessed by Misfortune", the reward for Hardcore Mode.
    /// </summary>
    public partial class CharacterDialogService
    {
        private const string CharacterTraitsTitle = "Character Traits";
        private const string BlessedByMisfortuneName = "Blessed by Misfortune";
        private const string BlessedByMisfortuneDescription = "Luck shines on those who walk the path of danger.";
        private const string BlessedByMisfortuneEffects = "This trait gives you extra luck in many luck-based activities on Open Roleplay.";

        public void OpenCharacterTraits(Player player)
        {
            CharacterPreferences? preferences = player.GetComponent<CharacterPreferences>();
            if (preferences == null)
            {
                return;
            }

            if (!preferences.HasBlessedByMisfortuneTrait())
            {
                ShowNoTraitsMessage(player);
                return;
            }

            OpenCharacterTraitList(player);
        }

        private void OpenCharacterTraitList(Player player)
        {
            BetterTablistDialog traitsDialog = new BetterTablistDialog("Details", "Exit", 2);
            traitsDialog.SetTitle(TitleType.Parents, CharacterTraitsTitle);
            traitsDialog.AddHeaders("Trait", "Description");
            traitsDialog.AddRow(BlessedByMisfortuneName, BlessedByMisfortuneDescription);

            void CharacterTraitsDialogHandler(TablistDialogResponse response)
            {
                if (response.Response != DialogResponse.LeftButton)
                {
                    return;
                }

                ShowBlessedByMisfortuneDetails(player);
            }

            _dialogService.ShowIgnoringDisconnect(player, traitsDialog, CharacterTraitsDialogHandler);
        }

        private void ShowBlessedByMisfortuneDetails(Player player)
        {
            string content = $"{ChatColor.CornflowerBlue}Name:\n{ChatColor.White}{BlessedByMisfortuneName}\n\n{ChatColor.CornflowerBlue}Description:\n{ChatColor.White}{BlessedByMisfortuneDescription}\n\n{ChatColor.CornflowerBlue}Effects:\n{ChatColor.White}{BlessedByMisfortuneEffects}";

            ShowMessage(player, "Go Back", content, () => OpenCharacterTraitList(player), CharacterTraitsTitle, BlessedByMisfortuneName);
        }

        private void ShowNoTraitsMessage(Player player)
        {
            BetterMessageDialog noTraitsDialog = new BetterMessageDialog("Go Back");
            noTraitsDialog.SetTitle(TitleType.Parents, CharacterTraitsTitle);
            noTraitsDialog.SetContent("Your character has no traits at this moment!");

            _dialogService.ShowIgnoringDisconnect(player, noTraitsDialog, (MessageDialogResponse response) => { });
        }
    }
}
