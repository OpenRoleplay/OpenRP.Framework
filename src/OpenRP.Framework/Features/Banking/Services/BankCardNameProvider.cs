using OpenRP.Framework.Features.Banking.Entities;
using OpenRP.Framework.Features.Banking.Enums;
using OpenRP.Framework.Features.Inventories.Components;
using OpenRP.Framework.Features.Inventories.Entities;

namespace OpenRP.Framework.Features.Banking.Services
{
    /// <summary>
    /// Shows a card in the inventory with its account and name, read fresh, so renaming either shows at once.
    /// </summary>
    public class BankCardNameProvider : IInventoryItemNameProvider
    {
        private readonly IBankCardService _bankCardService;

        public BankCardNameProvider(IBankCardService bankCardService)
        {
            _bankCardService = bankCardService;
        }

        public bool Handles(InventoryItem inventoryItem)
        {
            return inventoryItem.GetBankCardId() != null;
        }

        public string GetName(InventoryItem inventoryItem)
        {
            return GetCard(inventoryItem)?.GetLabel() ?? inventoryItem.GetName();
        }

        public string? GetExtraInformation(InventoryItem inventoryItem)
        {
            BankCardDetails? card = GetCard(inventoryItem);
            if (card == null)
                return "Unknown card";
            return card.GetStatus() == BankCardStatus.Active ? null : card.GetStatusText();
        }

        private BankCardDetails? GetCard(InventoryItem inventoryItem)
        {
            ulong? cardId = inventoryItem.GetBankCardId();
            return cardId == null ? null : _bankCardService.GetCardDetails(cardId.Value);
        }
    }
}
