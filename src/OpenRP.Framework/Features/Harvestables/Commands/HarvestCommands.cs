using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Harvestables.Services;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Harvestables.Commands
{
    public class HarvestCommands : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Harvest a resource.",
            CommandGroups = new string[] { "Gathering" })]
        public void Harvest(Player player, IHarvestableService harvestableService, string resource)
        {
            harvestableService.BeginHarvest(player, resource);
        }

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Harvest a resource.",
            CommandGroups = new string[] { "Gathering" })]
        public void Harvest(Player player, IHarvestableService harvestableService)
        {
            string options = string.Join($" {ChatColor.Yellow}OR{ChatColor.White} ", harvestableService.GetResourceNames());
            player.SendPlayerInfoMessage(PlayerInfoMessageType.SYNTAX, $"/harvest [{options}]");
        }
    }
}
