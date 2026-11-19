using OpenRP.Framework.Features.Fishing.Entities;
using OpenRP.Framework.Features.Fishing.Services;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.Fishing.Systems
{
    public class FishHotspotSystem : ISystem
    {
        [Event]
        public void OnGameModeInit(IFishHotspotService fishHotspotService)
        {
            fishHotspotService.CreateHotspots();
        }

        [Timer(FishingSettings.HotspotMoveIntervalInMilliseconds)]
        public void MoveHotspots(IFishHotspotService fishHotspotService)
        {
            fishHotspotService.MoveHotspots();
        }
    }
}
