using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Phones.Components;
using OpenRP.Framework.Features.Phones.Helpers;
using OpenRP.Framework.Features.Phones.Services;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Phones.Commands
{
    public class CallCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Call a phone number, for example /call 382-0142 or /call 911.",
            CommandGroups = new string[] { "Phone" })]
        public void Call(Player player, IPhoneService phoneService, IPhoneCallService phoneCallService, string number)
        {
            Phone? phone = phoneService.GetPhoneToUse(player);
            uint? parsedNumber = PhoneNumbers.Parse(number);
            if (phone == null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You don't have a phone.");
                return;
            }
            if (parsedNumber == null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "That isn't a phone number. Enter up to 7 digits, for example 382-0142.");
                return;
            }

            string? error = phoneCallService.Dial(player, phone, parsedNumber.Value);
            if (error != null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, error);
            }
        }

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Call a phone number, for example /call 382-0142 or /call 911.",
            CommandGroups = new string[] { "Phone" })]
        public void Call(Player player)
        {
            player.SendPlayerInfoMessage(PlayerInfoMessageType.SYNTAX, "/call [number]");
        }
    }
}
