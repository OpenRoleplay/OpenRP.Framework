using OpenRP.Framework.Features.Fishing.Entities;
using OpenRP.Framework.Features.Fishing.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Fishing.Systems
{
    public class FishingSystem : ISystem
    {
        [Timer(FishingSettings.FisherUpdateIntervalInMilliseconds)]
        public void UpdateFishers(IFishingService fishingService)
        {
            fishingService.UpdateFishers();
        }

        [Event]
        public void OnPlayerKeyStateChange(Player player, Keys newKeys, Keys oldKeys, IFishingService fishingService)
        {
            if (newKeys.HasFlag(Keys.Yes) && !oldKeys.HasFlag(Keys.Yes))
            {
                fishingService.LandBite(player);
            }
        }

        // The issuer, weapon and body part are declared as the types SampSharp sends them in. They aren't used.
        [Event]
        public void OnPlayerTakeDamage(Player player, EntityId issuer, float amount, uint weapon, SampSharp.OpenMp.Core.Api.BodyPart bodyPart, IFishingService fishingService)
        {
            fishingService.EndFishingAfterDamage(player);
        }

        // The killer and reason are declared as the types SampSharp sends them in. They aren't used.
        [Event]
        public void OnPlayerDeath(Player player, EntityId killer, int reason, IFishingService fishingService)
        {
            fishingService.EndFishingAfterDeath(player);
        }

        [Event]
        public void OnPlayerDisconnect(Player player, DisconnectReason disconnectReason, IFishingService fishingService)
        {
            fishingService.EndFishing(player);
        }
    }
}
