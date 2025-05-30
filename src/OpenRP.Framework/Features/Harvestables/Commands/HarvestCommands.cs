using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared;
using SampSharp.Entities.SAMP;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRP.Framework.Features.Harvestables.Entities;
using OpenRP.Framework.Shared.Chat.Extensions;

namespace OpenRP.Framework.Features.Harvestables.Commands
{
    public class HarvestCommands : ISystem
    {
        private readonly IEnumerable<IHarvestableModel> _harvestableResources;

        public HarvestCommands(IEnumerable<IHarvestableModel> harvestableResources)
        {
            _harvestableResources = harvestableResources;
        }

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Harvest a resource.",
            CommandGroups = new string[] { "Gathering" })]
        public void Harvest(Player player, string resource)
        {
            // Look for a harvestable provider that matches the resource name.
            var harvestable = _harvestableResources.FirstOrDefault(r =>
                r.ResourceName.Equals(resource, StringComparison.OrdinalIgnoreCase));

            if (harvestable != null)
            {
                // Delegate to the resource's own harvest logic.
                harvestable.BeginHarvest(player);
            }
            else
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, $"No harvestable resource found for '{resource}'.");
            }
        }

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Harvest a resource.",
            CommandGroups = new string[] { "Gathering" })]
        public void Harvest(Player player)
        {
            IEnumerable<string> harvestable = _harvestableResources.Select(i => i.ResourceName);
            string options = string.Join($" {ChatColor.Yellow}OR{ChatColor.White} ", harvestable);
            player.SendPlayerInfoMessage(PlayerInfoMessageType.SYNTAX, $"/harvest [{options}]");
        }
    }
}
