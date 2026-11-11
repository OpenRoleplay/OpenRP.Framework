using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Radios.Services;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Radios.Commands
{
    public class RadioTalkCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Talk over your radio, for example /r Unit 12 en route.",
            CommandGroups = new string[] { "Radio" })]
        public void R(Player player, IRadioBroadcastService radioBroadcastService, string text)
        {
            string? error = radioBroadcastService.TransmitFromPlayer(player, text);
            if (error != null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, error);
            }
        }

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Talk over your radio, for example /r Unit 12 en route.",
            CommandGroups = new string[] { "Radio" })]
        public void R(Player player)
        {
            player.SendPlayerInfoMessage(PlayerInfoMessageType.SYNTAX, "/r [message]");
        }
    }
}
