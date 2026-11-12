using OpenRP.Framework.Features.Actors.Components;
using OpenRP.Framework.Features.Banking.Services;
using OpenRP.Framework.Features.Banking.Services.Dialogs;
using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Banking.Commands
{
    public class BankCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Open the bank menu at the teller you're standing at.",
            CommandGroups = new string[] { "Bank" })]
        public void Bank(Player player, ITellerService tellerService, IBankDialogService bankDialogService)
        {
            if (!player.IsPlayerPlayingAsCharacter())
            {
                return;
            }

            ServerActor? teller = tellerService.GetNearbyTeller(player);
            if (teller == null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You need to see a bank teller. Walk up to the counter at a bank.");
                return;
            }
            bankDialogService.OpenBankMenu(player, teller);
        }
    }
}
