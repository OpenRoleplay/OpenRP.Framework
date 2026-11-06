using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Features.Factions.Services;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Factions.Commands
{
    public class FactionPermissionCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Give a faction a permission, or take it away again. This is how top-level factions get their permissions.",
            CommandGroups = new string[] { "Admin", "Factions" })]
        public void FactionPermission(Player player, IFactionService factionService, IFactionPermissionService factionPermissionService, int faction_id, string permission)
        {
            Faction? faction = faction_id > 0 ? factionService.GetFaction((ulong)faction_id) : null;
            if (faction == null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, $"There is no faction with id {faction_id}.");
                return;
            }
            if (!factionPermissionService.IsKnownPermission(permission))
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, $"There is no permission '{permission}'. Command permissions look like cmd.<command>, for example cmd.fooc.");
                return;
            }
            if (factionPermissionService.IsDefaultPermission(permission))
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, $"Every faction already has {permission}.");
                return;
            }

            bool grant = !factionPermissionService.IsGrantedToFaction(faction, permission);
            factionPermissionService.SetFactionGrant(faction, permission, grant);
            SendResult(player, factionPermissionService, faction, permission, grant);
        }

        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Give a faction a permission, or take it away again. This is how top-level factions get their permissions.",
            CommandGroups = new string[] { "Admin", "Factions" })]
        public void FactionPermission(Player player)
        {
            player.SendPlayerInfoMessage(PlayerInfoMessageType.SYNTAX, "/factionpermission [faction id] [permission, for example cmd.fooc]");
        }

        private static void SendResult(Player player, IFactionPermissionService factionPermissionService, Faction faction, string permission, bool granted)
        {
            if (!granted)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ADMIN, $"{faction.GetName()} no longer has {permission}.");
                return;
            }

            string note = factionPermissionService.FactionHasPermission(faction, permission) ? string.Empty : " It's inactive until its parent faction has it too.";
            player.SendPlayerInfoMessage(PlayerInfoMessageType.ADMIN, $"{faction.GetName()} now has {permission}.{note}");
        }
    }
}
