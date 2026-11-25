# Inventories

Every character has an inventory with a weight limit. Items can hold their own inventory: a wallet holds money. What lies on the ground is in the world inventory.

**Why weight:** you can't carry everything, so what you take with you is a real choice. Clothing is an item too, so you own, carry, swap and give outfits instead of just picking a skin.

## How to use it

| Command | Groups | Does |
|---|---|---|
| `/inventory`, `/inv` | Default | Opens your inventory |

Picking an item offers, depending on the item:
- **Open:** for a wallet.
- **Set As Default:** for wallets.
- **Wear.**
- **Use:** for any other item. It only uses up an item with a use limit (such as the hotwire tools); other items stay. It publishes [`OnPlayerInventoryItemUsed`](../server-events.md), so a phone or radio opens its menu.
- **Edit.**
- **Description.**
- **Destroy.**
- **Drop.**

## How it works

1. **Weight.** An item's weight is its weight × amount, plus the contents if it holds an inventory. New characters get 10 kg. A stack can have its own weight, such as a caught fish. It keeps that weight and its remaining uses when it's moved or dropped.
2. **Stacking.** Stackable items join a matching stack when their extra data matches.
3. **Flags.** Each stack has extra data, such as `DEFAULT_WALLET` (where money goes), `WEARING` (the worn outfit), the vehicle id of a key or the card id of a debit card.

## Known limitations

- **Too much in one class.** `Inventory` mixes state with logic, and is being trimmed when touched.

## Deep dive

- **Code:** `Features/Inventories/`.
- **Components:**
  - `Inventory`: `AddItem` (also with a weight of its own), `TakeItem`, `UseItem`, `DoesItemFit`, `DoesWeightFit`, `GetDefaultWallet`, …;
  - `InventoryItem`: `Transfer`, `GetItemInventory`, `GetVehicleId`, `GetSkinModelId`, …;
  - `InventoryOpened`, while the dialog is open.
- **Extra data:** `Helpers/ItemAdditionalData` parses and compares the key/value text. "Personalised" keys don't stop stacking.
- **Names of their own:** an `IInventoryItemNameProvider`, registered with `AddInventoryItemNameProvider<T>()`, gives the items it handles their own name and Extra Information in the inventory, such as a debit card showing its account. Other items show their item type's name. **Why:** the name can come from data another feature owns, such as an account name, which can't be stored in the item's extra data.
- **Services:**
  - `IInventoryManager`: `CreateInventory` and `LoadInventory`;
  - `IInventoryItemManager`;
  - `IInventoryDialogService`.
- **Persistence:** [BaseManager](../persistence/base-manager.md), with a 60-second sync (`InventorySystem`).
- **Tables:** [`Inventories`, `InventoryItems`](../database.md#items-inventories-and-money).

Want more? → [Items](items.md) · [Currencies](currencies.md) · [DroppedItems](dropped-items.md)
