# Debit cards and ATMs

A debit card is an item that lets whoever holds it, and knows its 4-digit PIN, use one bank account: at ATMs and to [pay](../payments.md). The bank issues it at the teller.

## How to use it

| Where | What |
|---|---|
| Teller, account menu: **Debit card** | Order a card (an optional name, then the PIN twice), rename it, change the PIN, block it, unblock it, or order a new one |
| `/atm` within 2 units of an ATM | Choose a card if you carry several, type the PIN, then **Balance**, **Withdraw cash** or **Deposit cash** |
| Inventory, **Use** on a card | At an ATM: the ATM with that card. Elsewhere: the card's details and **Rename the card** |

**In the inventory** a card shows its bank, account and digits, so you can tell them apart: "SAB debit card: Savings ...4821". A named card puts its name first: "Groceries (SAB debit card: Savings ...4821)". "Blocked", "Cancelled" or "Account closed" shows under Extra Information.

## The rules

| Rule | Value |
|---|---|
| Cards per account per character | One working card; ordering a new one cancels the old one for good |
| Who can use a card | Whoever holds it and knows the PIN. The owner also needs full access to the account |
| Wrong PINs | 3 in a row block the card; the bank unblocks it |
| ATM withdrawals | At most 1,000 in the account's currency per card in any 24 hours |
| ATM deposits | The cash is taken at once and reaches the account **2 hours** later. At a teller it's there at once |
| Card names | Optional, at most 32 characters, no `{ }`, tabs or line breaks; anyone holding the card may rename it |

- **Why a card item with a PIN:** cards can be stolen and PINs overheard, which is roleplay. A stolen card still needs the PIN.
- **Why ATM deposits wait 2 hours:** like a 1990s envelope deposit, the money sits in the machine until the bank collects it. That gives cash in an ATM a place and a time, so ATMs can be robbed later.
- **Why a withdrawal limit:** a stolen card with an overheard PIN can't empty an account at once.
- **Taking someone's access away** blocks their cards for that account.
- **Closing an account** is refused while an ATM deposit is still on its way to it.

## ATMs

Every placed [map object](../maps.md) of model **2942** (`kmb_atm1`) is an ATM. Mappers place them with `/editmaps`; there's no ATM list to keep up.

**Why only placed objects:** GTA SA doesn't place `kmb_atm1` in the world itself, and its other ATM models (`kmb_atm2`, `kmb_atm3`) are broken variants. A separate list of ATMs would drift from the maps.

An ATM works in the object's interior and virtual world (or all of them, when the object is set to -1).

## Known limitations

- **ATMs belong to no bank,** so there's no fee for using another bank's ATM.
- **A stolen faction card:** when someone other than the owner uses a card, the owner's faction membership isn't checked, only that the card isn't blocked.
- **Cash and the bank aren't saved together:** the bank side is written at once, the wallet within 60 seconds, as at the teller.

## Deep dive

- **Services:**
  - `IBankCardService`: issuing (the card row, then the item with `[BANK_CARD_ID=n]`, saved at once), PINs, blocking, names, `GetUseError`;
  - `IAtmService`: finding ATMs, withdrawals within the limit, pending deposits, `CreditDueDeposits`;
  - `BankCardNameProvider`: the card's inventory name (an `IInventoryItemNameProvider`, see [Inventories](../inventories.md#deep-dive));
  - `AtmMapObjectSpawnHandler`: puts an `Atm` component on each spawned ATM object.
- **PINs** are hashed with BCrypt, like passwords. Hashing and checking run on a worker thread, so the server doesn't wait.
- **The limit** adds up the card's withdrawals of the last 24 hours (`BankTransactions.BankCardId`) in the same transaction as the debit. It locks the card row first, so two withdrawals with one card, even on two servers, can't both pass it.
- **Pending deposits:** `BankingSystem` runs every minute. Each due deposit is claimed by setting `CreditedAt` in the same transaction as the credit, so two servers never credit it twice. The row keeps the ATM's position, interior and world.
- **Tables:** [`BankCards`, `BankPendingDeposits`](../../database.md#banking).

Want more? → [Banking](../banking.md) · [Payments](../payments.md) · [Tellers](tellers.md)
