using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Radios.Components;
using OpenRP.Framework.Features.Radios.Services;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Radios.Commands
{
    public class GiveRadioCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Give a player a new handheld radio.",
            CommandGroups = new string[] { "Admin", "Radio" })]
        public void GiveRadio(Player player, IRadioEditorService radioEditorService, Player playerid)
        {
            (Radio? radio, string? error) = radioEditorService.GiveRadio(playerid);
            if (error != null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, error);
                return;
            }

            player.SendPlayerInfoMessage(PlayerInfoMessageType.ADMIN, $"You gave {playerid.Name} a handheld radio, tuned to {radio!.GetFrequency()}.");
            playerid.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, "You received a handheld radio. Use /radio to tune it and /r to talk.");
        }

        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Give a player a new handheld radio.",
            CommandGroups = new string[] { "Admin", "Radio" })]
        public void GiveRadio(Player player)
        {
            player.SendPlayerInfoMessage(PlayerInfoMessageType.SYNTAX, "/giveradio [player]");
        }
    }
}
