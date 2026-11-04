using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Accounts.Components;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Characters.Services.Dialogs
{
    public partial class CharacterDialogService
    {
        public void OpenCharacterSelection(Player player, Action onGoBack)
        {
            Account? account = player.GetComponent<Account>();
            if (account == null)
            {
                return;
            }

            List<CharacterModel> characters = _characterService.GetCharactersByAccountId(account.GetAccountId());
            BetterListDialog selectionDialog = CreateCharacterSelectionDialog(characters, out int createCharacterRowIndex);

            void CharacterSelectionDialogHandler(ListDialogResponse response)
            {
                if (response.Response != DialogResponse.LeftButton)
                {
                    player.Kick();
                    return;
                }

                if (response.ItemIndex == createCharacterRowIndex)
                {
                    BeginCharacterCreation(player, onGoBack);
                    return;
                }

                _ = _characterService.SelectCharacterAsync(player, characters[response.ItemIndex].Id);
            }

            _dialogService.Show(player.Entity, selectionDialog, CharacterSelectionDialogHandler);
        }

        private static BetterListDialog CreateCharacterSelectionDialog(List<CharacterModel> characters, out int createCharacterRowIndex)
        {
            BetterListDialog selectionDialog = new BetterListDialog("Next", "Quit");
            selectionDialog.SetTitle(TitleType.Children, "Character Selection");

            foreach (CharacterModel character in characters)
            {
                selectionDialog.AddRow($"{ChatColor.CornflowerBlue}{character.FirstName} {character.LastName}");
            }

            createCharacterRowIndex = selectionDialog.AddRow("Create a new character");
            return selectionDialog;
        }
    }
}
