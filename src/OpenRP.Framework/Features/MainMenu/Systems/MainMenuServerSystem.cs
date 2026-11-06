using OpenRP.Framework.Features.Characters.Services.Dialogs;
using OpenRP.Framework.Features.MainMenu.Entities;
using OpenRP.Framework.Features.MainMenu.Services.Dialogs;
using OpenRP.Framework.Shared.ServerEvents.Attributes;
using OpenRP.Framework.Shared.ServerEvents.Entities;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.MainMenu.Systems
{
    public class MainMenuServerSystem : IServerSystem
    {
        [ServerEvent]
        public void OnAccountLoggedIn(OnAccountLoggedInEventArgs args, ICharacterDialogService characterDialogService, IMainMenuDialogService mainMenuDialogService)
        {
            if (args.Player == null)
            {
                return;
            }

            OpenCharacterSelection(args.Player, characterDialogService, mainMenuDialogService);
        }

        [ServerEvent]
        public void OnCharacterSelected(OnCharacterSelectedEventArgs args, IEnumerable<IMainMenuScene> mainMenuScenes)
        {
            if (args.Player == null)
            {
                return;
            }

            LeaveMainMenuScenes(args.Player, mainMenuScenes);
        }

        /// <summary>
        /// Going back from the selection returns to the main menu's choices, so the player can log in differently.
        /// </summary>
        private static void OpenCharacterSelection(Player player, ICharacterDialogService characterDialogService, IMainMenuDialogService mainMenuDialogService)
        {
            characterDialogService.OpenCharacterSelection(player, () => mainMenuDialogService.OpenMainMenuChoiceMenu(player));
        }

        private static void LeaveMainMenuScenes(Player player, IEnumerable<IMainMenuScene> mainMenuScenes)
        {
            foreach (IMainMenuScene scene in mainMenuScenes)
            {
                scene.Leave(player);
            }
        }
    }
}
