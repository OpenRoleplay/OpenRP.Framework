using OpenRP.Framework.Features.ChickenCoops.Services;
using OpenRP.Framework.Features.Commands.Attributes;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.ChickenCoops.Commands
{
    public class CollectEggCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Collect an egg from the chicken coop you're at. Crouch so you don't scare the chickens.",
            CommandGroups = new string[] { "Farming" })]
        public void CollectEgg(Player player, IChickenCoopService chickenCoopService)
        {
            chickenCoopService.BeginCollectEgg(player);
        }
    }
}
