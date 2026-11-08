# DroppedItems

Items dropped on the ground. They lie there as objects with a label, are saved, and anyone can pick them up.

## How to use it

| Command | Groups | Does |
|---|---|---|
| `/pickup` | Default | Picks up the item you're standing next to, if it fits in your inventory |

Items are dropped from the [inventory](inventories.md) dialog.

## How it works

1. **Dropping** moves the items into the **world inventory** (inventory 1), and stores the spot on the ground below you. ColAndreas finds the ground.
2. **On the ground,** the item shows as an object (the item's drop model, or model 1575) with a small label.
3. **Picking up** first deletes the item's row, and only then puts the item in your inventory. If the row is already gone, someone on another server took it.
4. **At startup,** every dropped item is put back on the ground.

**Why delete first:** only one server can delete the row, so an item can't be picked up twice.

## Known limitations

- **No despawning.** Dropped items stay on the ground forever.
- **Other servers' items** only appear after a restart.

## Deep dive

- **Code:** `Features/DroppedItems/`.
- **Component:** `DroppedItem`, on the entity of its row (`DroppedItemEntities`). Its object and label are child entities of it.
- **Service:** `IDroppedItemService`: `LoadDroppedItems`, `DropItem` and `PickupNearbyItem`.
- **Money:** picking up money publishes [`OnPlayerCashUpdate`](../server-events.md).
- **Tables:** [`DroppedInventoryItems`](../database.md#droppedinventoryitems-droppedinventoryitemmodel), with the item itself kept in the world inventory.

Want more? → [Inventories](inventories.md)
