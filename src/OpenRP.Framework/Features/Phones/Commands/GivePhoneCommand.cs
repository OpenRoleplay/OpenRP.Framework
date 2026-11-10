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
    public class GivePhoneCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Give a player a new mobile phone with its own number.",
            CommandGroups = new string[] { "Admin", "Phone" })]
        public void GivePhone(Player player, IPhoneEditorService phoneEditorService, Player playerid)
        {
            (Phone? phone, string? error) = phoneEditorService.GivePhone(playerid);
            if (error != null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, error);
                return;
            }

            string number = PhoneNumbers.Format(phone!.GetNumber());
            player.SendPlayerInfoMessage(PlayerInfoMessageType.ADMIN, $"You gave {playerid.Name} a phone with number {number}.");
            playerid.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"You received a mobile phone. Its number is {number}. Use /phone to use it.");
        }

        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Give a player a new mobile phone with its own number.",
            CommandGroups = new string[] { "Admin", "Phone" })]
        public void GivePhone(Player player)
        {
            player.SendPlayerInfoMessage(PlayerInfoMessageType.SYNTAX, "/givephone [player]");
        }
    }
}
