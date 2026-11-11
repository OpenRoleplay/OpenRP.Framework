using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Radios.Services;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Radios.Commands
{
    public class GiveEarpieceCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Give a player a radio earpiece, so they can keep their radio traffic to themselves.",
            CommandGroups = new string[] { "Admin", "Radio" })]
        public void GiveEarpiece(Player player, IRadioEditorService radioEditorService, Player playerid)
        {
            string? error = radioEditorService.GiveEarpiece(playerid);
            if (error != null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, error);
                return;
            }

            player.SendPlayerInfoMessage(PlayerInfoMessageType.ADMIN, $"You gave {playerid.Name} a radio earpiece.");
            playerid.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, "You received a radio earpiece. Choose Earpiece in /radio to keep radio traffic to yourself.");
        }

        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Give a player a radio earpiece, so they can keep their radio traffic to themselves.",
            CommandGroups = new string[] { "Admin", "Radio" })]
        public void GiveEarpiece(Player player)
        {
            player.SendPlayerInfoMessage(PlayerInfoMessageType.SYNTAX, "/giveearpiece [player]");
        }
    }
}
