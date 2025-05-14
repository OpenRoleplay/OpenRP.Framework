using OpenRP.Framework.Features.Fishing.Components;
using OpenRP.Framework.Features.Fishing.Services;
using SampSharp.Entities.SAMP;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRP.Streamer;

namespace OpenRP.Framework.Features.Fishing.Systems
{
    public class FishHotspotSystem : ISystem
    {
        [Event]
        public async void OnGameModeInit(IFishHotspotService fishHotspotService)
        {
            for (var i = 0; i < 30; i++)
            {
                await fishHotspotService.CreateRandomHotspot();
            }
        }

        [Event]
        public void OnPlayerConnect(Player player, IEntityManager entityManager)
        {
            // Temporarily for testing
            int iconId = 0;
            foreach (FishHotspot fishHotspot in entityManager.GetComponents<FishHotspot>())
            {
                DynamicObject fishHotspotObject = fishHotspot.GetDynamicObject();
                player.SetMapIcon(iconId, fishHotspotObject.Position, MapIcon.BoatYard, 0, MapIconType.Global);
                iconId++;
            }
        }
    }
}
