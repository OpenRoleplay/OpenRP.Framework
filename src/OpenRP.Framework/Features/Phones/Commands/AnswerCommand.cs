using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Phones.Services;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Phones.Commands
{
    /// <summary>
    /// Not /pickup: that's picking an item up off the ground (DroppedItems).
    /// </summary>
    public class AnswerCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Answer the call ringing on your phone.",
            CommandGroups = new string[] { "Phone" })]
        public void Answer(Player player, IPhoneCallService phoneCallService)
        {
            AnswerCall(player, phoneCallService);
        }

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Answer the call ringing on your phone. Short for /answer.",
            CommandGroups = new string[] { "Phone" })]
        public void P(Player player, IPhoneCallService phoneCallService)
        {
            AnswerCall(player, phoneCallService);
        }

        private static void AnswerCall(Player player, IPhoneCallService phoneCallService)
        {
            string? error = phoneCallService.Answer(player);
            if (error != null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, error);
            }
        }
    }
}
