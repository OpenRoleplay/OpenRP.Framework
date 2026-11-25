using OpenRP.Framework.Features.Banking.Components;
using OpenRP.Framework.Features.Banking.Services;
using OpenRP.Framework.Features.Banking.Services.Dialogs;
using OpenRP.Framework.Shared.ServerEvents.Attributes;
using OpenRP.Framework.Shared.ServerEvents.Entities;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;

namespace OpenRP.Framework.Features.Banking.Systems
{
    public class BankCardServerSystem : IServerSystem
    {
        /// <summary>
        /// "Use" on a debit card in the inventory: at an ATM it opens the ATM with that card, elsewhere the card's menu.
        /// </summary>
        [ServerEvent]
        public void OnPlayerInventoryItemUsed(OnPlayerInventoryItemUsedEventArgs args, IAtmService atmService, IBankDialogService bankDialogService)
        {
            ulong? cardId = args.InventoryItem.GetBankCardId();
            if (cardId == null)
            {
                return;
            }

            Atm? atm = atmService.GetNearbyAtm(args.Player);
            if (atm != null)
            {
                bankDialogService.OpenAtm(args.Player, atm, cardId);
                return;
            }
            bankDialogService.OpenCard(args.Player, cardId.Value);
        }
    }
}
