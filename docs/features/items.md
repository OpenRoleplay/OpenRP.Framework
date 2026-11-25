# Items

The item types that everything in an [inventory](inventories.md) is made of, such as "Wallet", "$20 bill" or "Rusty Paperclip". What players own are stacks of these, in inventories.

## How it works

1. **At startup,** every item type is loaded as an `Item` component.
2. **Every 60 seconds,** items added or changed in the database are picked up, so new items need no restart.
3. **The type decides the use:**

| Type | What it is |
|---|---|
| `Skin` | Clothing; wearing it changes your skin |
| `Wallet` | Holds money |
| `Currency` | A bill or coin |
| `Attachment` | Something worn as an object |
| `VehicleKey` | Opens one vehicle |
| `Misc` | Anything else |
| `Fish` | A caught fish |
| `Phone` | A [mobile phone](phones.md) |
| `Radio` | A [handheld radio](radios.md) |
| `RadioEarpiece` | Lets a radio's carrier listen without the people around them hearing it |
| `BankCard` | A [debit card](banking/cards-and-atms.md) for one bank account |

**Seeded items (68):**
- a uniform;
- a wallet;
- the SAD and CRU bills and coins;
- a vehicle key;
- the hotwire tools (33, 34, 35);
- a chicken egg (36);
- 12 fish;
- a mobile phone (49);
- a handheld radio (50) and a radio earpiece (51);
- [fishing](fishing.md) gear: a rod (52), bait (53) and a net (54);
- fishing junk (55 to 57) and treasures (58 to 61);
- four net-only fish (62 to 65);
- two small saltwater fish: a sardine (66) and a mackerel (67);
- a debit card (68).

Some code uses these ids, so don't renumber them.

## Known limitations

- Items can't be created or edited in-game.

## Deep dive

- **Code:** `Features/Items/`.
- **Component:** `Item`: name, weight, flags, and `IsItemWallet()`, `IsItemCurrency()`, `IsItemRadio()`, `GetCurrencyUnitId()`, ….
- **Services:** `IItemManager`: `LoadItems` and `ProcessChanges`. `IItemGiveService`: the [`/giveitem`](admins.md) command.
- **Logs:** written to `Logs/ItemManager/`.
- **Table:** [`Items`](../database.md#items-itemmodel).

Want more? → [Inventories](inventories.md)
