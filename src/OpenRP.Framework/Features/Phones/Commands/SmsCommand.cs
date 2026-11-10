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
    public class SmsCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Send a text message, for example /sms 382-0142 On my way.",
            CommandGroups = new string[] { "Phone" })]
        public void Sms(Player player, IPhoneService phoneService, IPhoneEditorService phoneEditorService, IPhoneRoleplayService phoneRoleplayService, string number, string text)
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

            string? error = phoneEditorService.SendText(phone, parsedNumber.Value, text);
            if (error != null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, error);
                return;
            }
            phoneRoleplayService.SendTextAction(player);
            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"Message sent to {phoneService.GetNumberLabel(phone, parsedNumber.Value)}.");
        }

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Send a text message, for example /sms 382-0142 On my way.",
            CommandGroups = new string[] { "Phone" })]
        public void Sms(Player player)
        {
            player.SendPlayerInfoMessage(PlayerInfoMessageType.SYNTAX, "/sms [number] [message]");
        }
    }
}
