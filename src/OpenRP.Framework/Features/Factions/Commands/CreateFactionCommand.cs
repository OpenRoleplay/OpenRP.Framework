using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Features.Factions.Services;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Factions.Commands
{
    public class CreateFactionCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Create a faction with a Leader rank. Make someone its leader with /setfactionleader.",
            CommandGroups = new string[] { "Admin", "Factions" })]
        public void CreateFaction(Player player, IFactionMembershipService factionMembershipService, string name)
        {
            Faction faction = factionMembershipService.CreateFaction(name);
            player.SendPlayerInfoMessage(PlayerInfoMessageType.ADMIN, $"Created faction {faction.GetName()} with id {faction.GetId()}.");
        }

        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Create a faction with a Leader rank. Make someone its leader with /setfactionleader.",
            CommandGroups = new string[] { "Admin", "Factions" })]
        public void CreateFaction(Player player)
        {
            player.SendPlayerInfoMessage(PlayerInfoMessageType.SYNTAX, "/createfaction [name]");
        }
    }
}
