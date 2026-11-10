using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Phones.Services.Dialogs;
using OpenRP.Framework.Features.Players.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Phones.Commands
{
    public class PhoneCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Use your phone: dial, the phone book, messages, the call log, the ringer and power.",
            CommandGroups = new string[] { "Phone" })]
        public void Phone(Player player, IPhoneDialogService phoneDialogService)
        {
            if (player.IsPlayerPlayingAsCharacter())
            {
                phoneDialogService.OpenPhone(player);
            }
        }
    }
}
