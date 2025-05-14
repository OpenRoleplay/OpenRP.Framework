using OpenRP.Framework.Features.ActorConversations.Services;
using OpenRP.Framework.Features.CDN.Services;
using OpenRP.Framework.Features.VirtualWorlds.Services;
using SampSharp.Entities.SAMP;
using OpenRP.Streamer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.MainMenu.Entities.Scenes
{
    public class FamilyBarbecueScene : IMainMenuScene
    {
        private IStreamerService _streamerService;
        private IActorConversationWithPlayerManager _actorConversationWithPlayerManager;
        private IVirtualWorldManager _virtualWorldService;
        private IOpenCdnService _openCdnService;

        public FamilyBarbecueScene(
            IStreamerService streamerService, 
            IActorConversationWithPlayerManager actorConversationWithPlayerManager, 
            IVirtualWorldManager virtualWorldService, 
            IOpenCdnService openCdnService
        )
        {
            _streamerService = streamerService;
            _actorConversationWithPlayerManager = actorConversationWithPlayerManager;
            _virtualWorldService = virtualWorldService;
            _openCdnService = openCdnService;
        }

        public void Prepare()
        {
            _streamerService.CreateDynamicObject(1281, new Vector3(2109.1777f, 182.0393f, 1.1948f), new Vector3(0f, 0f, 58.6f)); // parktable1
            _streamerService.CreateDynamicObject(1598, new Vector3(2115.3115f, 183.1194f, 0.4019f), new Vector3(0f, 0f, 0f)); // beachball
            _streamerService.CreateDynamicObject(19831, new Vector3(2107.7038f, 186.1086f, 0.3894f), new Vector3(0f, 0f, 49.5999f)); // Barbeque1
            _streamerService.CreateDynamicObject(1642, new Vector3(2112.4953f, 185.0725f, 0.3243f), new Vector3(2.3f, 0.6999f, 57.1999f)); // beachtowel02
            _streamerService.CreateDynamicObject(1611, new Vector3(2118.3303f, 186.8210f, 0.0868f), new Vector3(0f, 0f, 0f)); // sandcastle2
            _streamerService.CreateDynamicObject(1642, new Vector3(2113.4916f, 186.6195f, 0.3019f), new Vector3(2.3f, 0.0999f, 57.2000f)); // beachtowel02
            _streamerService.CreateDynamicObject(866, new Vector3(2114.4440f, 181.5556f, -0.3420f), new Vector3(0f, 0f, 0f)); // sand_combush03
            _streamerService.CreateDynamicObject(1610, new Vector3(2118.3178f, 186.1943f, 0.0854f), new Vector3(0f, 0f, 0f)); // sandcastle1
            _streamerService.CreateDynamicObject(19582, new Vector3(2107.8654f, 186.2229f, 1.2227f), new Vector3(0f, 0f, 0f), virtualWorld: _virtualWorldService.GetMainMenuSceneVirtualWorld(1)); // MarcosSteak1
            _streamerService.CreateDynamicObject(866, new Vector3(2107.2495f, 188.1663f, -0.0520f), new Vector3(0f, 0f, 343.2000f)); // sand_combush03
            _streamerService.CreateDynamicObject(866, new Vector3(2107.8615f, 177.6082f, -0.1820f), new Vector3(0f, 0f, 343.2000f)); // sand_combush03
            _streamerService.CreateDynamicObject(866, new Vector3(2117.0537f, 190.1357f, -0.3120f), new Vector3(0f, 0f, 343.2000f)); // sand_combush03
            _streamerService.CreateDynamicObject(18736, new Vector3(2108.4479f, 187.1985f, 0.5195f), new Vector3(61.8999f, 0f, 328.6999f), virtualWorld: _virtualWorldService.GetMainMenuSceneVirtualWorld(1)); // vent
            _streamerService.CreateDynamicObject(19582, new Vector3(2107.6804f, 185.9903f, 1.2227f), new Vector3(0f, 0f, 232.5000f), virtualWorld: _virtualWorldService.GetMainMenuSceneVirtualWorld(1)); // MarcosSteak1
            _streamerService.CreateDynamicObject(2354, new Vector3(2108.9575f, 182.5948f, 1.2621f), new Vector3(334.4f, 22.8999f, 312.5000f), virtualWorld: _virtualWorldService.GetMainMenuSceneVirtualWorld(1)); // burger_healthy

            _actorConversationWithPlayerManager.CreateConversation("CONV_FAMILY", new List<ulong> { 2, 3, 4 });
        }

        public void Play(
            Player player
        )
        {
            player.ToggleSpectating(true);
            player.ToggleControllable(false);

            player.Position = new Vector3(2113.49, 186.657, -1);
            player.CameraPosition = new Vector3(2132.886962, 202.465911, 6.284189);
            player.SetCameraLookAt(new Vector3(2110.900634, 185.995895, 3.264186));
            player.InterpolateCameraPosition(new Vector3(2132.886962, 202.465911, 6.284189), new Vector3(2117.288330, 175.030685, 0.654188), 60000, CameraCut.Move);
            player.InterpolateCameraLookAt(new Vector3(2110.900634, 185.995895, 3.264186), new Vector3(2111.591064, 185.435897, 0.804188), 60000, CameraCut.Move); // Did +0,75 x
            player.VirtualWorld = _virtualWorldService.GetMainMenuSceneVirtualWorld(1);

            _openCdnService.PlayBase64(player, "music", "VGhlIEJlYWNoIEJveXMgLSBGdW4sIEZ1biwgRnVuICgxOTY0KS5tcDM=");

            _actorConversationWithPlayerManager.AttachPlayerToConversationAsync("CONV_FAMILY", player);
        }
    }
}
