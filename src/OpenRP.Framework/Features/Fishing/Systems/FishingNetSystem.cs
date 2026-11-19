using OpenRP.Framework.Features.Fishing.Entities;
using OpenRP.Framework.Features.Fishing.Services;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.Fishing.Systems
{
    public class FishingNetSystem : ISystem
    {
        [Event]
        public void OnGameModeInit(IFishingNetManager fishingNetManager)
        {
            fishingNetManager.LoadAll();
        }

        [Timer(FishingSettings.NetSyncIntervalInMilliseconds)]
        public void ProcessChanges(IFishingNetManager fishingNetManager)
        {
            fishingNetManager.ProcessChanges();
        }

        [Event]
        public void OnGameModeExit(IFishingNetManager fishingNetManager)
        {
            fishingNetManager.SaveNow();
        }
    }
}
