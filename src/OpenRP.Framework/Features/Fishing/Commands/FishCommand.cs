using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Fishing.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Fishing.Commands
{
    public class FishCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Cast your line. Needs a Fishing Rod and Fishing Bait, while facing water or on a boat.",
            CommandGroups = new string[] { "Jobs", "Fishing" })]
        public void Fish(Player player, IFishingService fishingService)
        {
            fishingService.BeginFishing(player);
        }
    }
}
