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
- **Use.**
- **Edit.**
- **Description.**
- **Destroy.**
- **Drop.**

## How it works

1. **Weight.** An item's weight is its weight × amount, plus the contents if it holds an inventory. New characters get 10 kg.
2. **Stacking.** Stackable items join a matching stack when their extra data matches.
3. **Flags.** Each stack has extra data, such as `DEFAULT_WALLET` (where money goes), `WEARING` (the worn outfit) or the vehicle id of a key.

## Known limitations

- **Too much in one class.** `Inventory` mixes state with logic, and is being trimmed when touched.
- **Unused event.** `OnPlayerInventoryItemUsed` is never published.

## Deep dive

- **Code:** `Features/Inventories/`.
- **Components:**
  - `Inventory`: `AddItem`, `TakeItem`, `UseItem`, `DoesItemFit`, `GetDefaultWallet`, …;
  - `InventoryItem`: `Transfer`, `GetItemInventory`, `GetVehicleId`, `GetSkinModelId`, …;
  - `InventoryOpened`, while the dialog is open.
- **Extra data:** `Helpers/ItemAdditionalData` parses and compares the key/value text. "Personalised" keys don't stop stacking.
- **Services:**
  - `IInventoryManager`: `CreateInventory` and `LoadInventory`;
  - `IInventoryItemManager`;
  - `IInventoryDialogService`.
- **Persistence:** [BaseManager](../persistence/base-manager.md), with a 60-second sync (`InventorySystem`).
- **Tables:** [`Inventories`, `InventoryItems`](../database.md#items-inventories-and-money).

Want more? → [Items](items.md) · [Currencies](currencies.md) · [DroppedItems](dropped-items.md)
