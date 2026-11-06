using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Features.Factions.Services;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Factions.Commands
{
    public class SetFactionLeaderCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Make a player a leader of a faction, with its top rank.",
            CommandGroups = new string[] { "Admin", "Factions" })]
        public void SetFactionLeader(Player player, IFactionService factionService, IFactionMembershipService factionMembershipService, Player target, int faction_id)
        {
            Faction? faction = faction_id > 0 ? factionService.GetFaction((ulong)faction_id) : null;
            if (faction == null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, $"There is no faction with id {faction_id}.");
                return;
            }
            if (!target.IsPlayerPlayingAsCharacter())
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "That player is not playing as a character.");
                return;
            }

            string? error = factionMembershipService.SetLeader(target.GetPlayerCurrentlyPlayingAsCharacter(), faction);
            player.SendPlayerInfoMessage(error == null ? PlayerInfoMessageType.ADMIN : PlayerInfoMessageType.ERROR, error ?? $"{target.Name} is now a leader of {faction.GetName()}.");
        }

        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Make a player a leader of a faction, with its top rank.",
            CommandGroups = new string[] { "Admin", "Factions" })]
        public void SetFactionLeader(Player player)
        {
            player.SendPlayerInfoMessage(PlayerInfoMessageType.SYNTAX, "/setfactionleader [playerid] [faction id]");
        }
    }
}
