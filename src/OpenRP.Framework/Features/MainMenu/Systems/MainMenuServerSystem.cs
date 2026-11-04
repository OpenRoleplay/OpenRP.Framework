using OpenRP.Framework.Features.MainMenu.Entities;
using OpenRP.Framework.Shared.ServerEvents.Attributes;
using OpenRP.Framework.Shared.ServerEvents.Entities;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.MainMenu.Systems
{
    public class MainMenuServerSystem : IServerSystem
    {
        [ServerEvent]
        public void OnCharacterSelected(OnCharacterSelectedEventArgs args, IEnumerable<IMainMenuScene> mainMenuScenes)
        {
            if (args.Player == null)
            {
                return;
            }

            LeaveMainMenuScenes(args.Player, mainMenuScenes);
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
