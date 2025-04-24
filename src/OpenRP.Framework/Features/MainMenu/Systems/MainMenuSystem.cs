using OpenRP.Framework.Features.ActorConversations.Services;
using OpenRP.Framework.Features.CDN.Services;
using OpenRP.Framework.Features.MainMenu.Entities;
using OpenRP.Framework.Features.MainMenu.Services.Dialogs;
using OpenRP.Framework.Features.VirtualWorlds.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using OpenRP.Streamer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.MainMenu.Systems
{
    public class MainMenuSystem : ISystem
    {
        [Event]
        public void OnGameModeInit(
            IEnumerable<IMainMenuScene> mainMenuScenes
        )
        {
            try
            {
                foreach (var scene in mainMenuScenes)
                {
                    scene.Prepare();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        [Event]
        public void OnPlayerRequestClass(
            Player player,
            int classid,
            IMainMenuDialogService mainMenuService,
            IEnumerable<IMainMenuScene> mainMenuScenes
        ) 
        {
            try
            {
                // Toggle player state
                player.ToggleSpectating(true);
                player.ToggleControllable(false);

                // Get all scenes as a list
                var scenesList = mainMenuScenes.ToList();
                if (scenesList.Count > 0)
                {
                    var random = new Random();
                    int index = random.Next(scenesList.Count);
                    var selectedScene = scenesList[index];
                    selectedScene.Play(player);
                }
                else
                {
                    Console.WriteLine("No scenes available.");
                }

                // Open main menu after starting to play the scene
                mainMenuService.OpenMainMenu(player);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

    }
}
