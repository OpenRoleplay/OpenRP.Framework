using OpenRP.Framework.Features.Banking.Components;
using OpenRP.Framework.Features.Banking.Services;
using OpenRP.Framework.Features.Banking.Services.Dialogs;
using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Banking.Commands
{
    public class AtmCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" }, Description = "Use the ATM you're standing at, with a debit card.", CommandGroups = new string[] { "Bank" })]
        public void Atm(Player player, IAtmService atmService, IBankDialogService bankDialogService)
        {
            Atm? atm = atmService.GetNearbyAtm(player);
            if (atm == null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You aren't standing at an ATM.");
                return;
            }
            bankDialogService.OpenAtm(player, atm, null);
        }
    }
}
