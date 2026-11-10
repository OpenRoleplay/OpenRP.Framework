using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Phones.Services;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Phones.Commands
{
    public class HangupCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "End your call, or decline the call ringing on your phone.",
            CommandGroups = new string[] { "Phone" })]
        public void Hangup(Player player, IPhoneCallService phoneCallService)
        {
            EndCall(player, phoneCallService);
        }

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "End your call, or decline the call ringing on your phone. Short for /hangup.",
            CommandGroups = new string[] { "Phone" })]
        public void H(Player player, IPhoneCallService phoneCallService)
        {
            EndCall(player, phoneCallService);
        }

        private static void EndCall(Player player, IPhoneCallService phoneCallService)
        {
            string? error = phoneCallService.HangUp(player);
            if (error != null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, error);
            }
        }
    }
}
