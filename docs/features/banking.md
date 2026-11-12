# Banking

Banks hold accounts, and each account has its own OBAN (Open Bank Account Number), which others need to send it money. Players do their banking at a bank: they talk to an AI teller, who opens the right form for them, or use `/bank` at the counter.

## How to use it

| Command | Groups | Does |
|---|---|---|
| `/bank` | Default | Opens the bank menu at the teller you're standing at |
| `/editbanks` | Admin | Banks, their fee, branches, tellers and property accounts, and looking up an OBAN. See [Tellers](banking/tellers.md) |

**Talking to a teller** opens the matching form:

| You say, for example | The teller opens |
|---|---|
| "I'd like to open an account" | Open an account |
| "I want to deposit 500" | Deposit, with 500 filled in for you to confirm |
| "I'd like to withdraw some cash" | Withdraw |
| "I want to wire money to someone" | Transfer |
| "What's my balance?" | Your account, or the list when you have several |
| "My brother needs access" | Who has access |
| "I want to close my account" | Close the account |
| "Show me everything" | The full bank menu |

A teller only does what an admin allowed it to (see [Tellers](banking/tellers.md)). For anything else it says it can't help with that here.

## How it works

1. **Banks** have a name, a 4-letter code and a jurisdiction. The jurisdiction gives the accounts their currency, and its country's 2-letter tag starts their OBAN.
   - **Banks don't move.** The jurisdiction can't change after creating the bank, and a jurisdiction with a bank in it can't be deleted. **Why:** moving would change the meaning of every OBAN at the bank. To bank somewhere else, create another bank.
   - **When the territory changes country** (annexation, independence) or its currency changes, see [below](#when-the-territory-changes-country).
2. **Branches:** a bank's tellers only work inside its branches, which are properties set in `/editbanks`.
3. **Accounts** have one owner (a character, a faction or a property), and others can get view-only or full access. See [Accounts](banking/accounts.md).
4. **The form does the work.** The teller only opens it. You type and confirm amounts and OBANs yourself, and every check runs again on each step: the teller is still there, you still have access, the money is there.
   - **Why:** whatever a player says to the AI, it can't move money, choose an amount or act for someone else.
5. **Fees:** an account costs its bank's monthly fee (at most 100) every 30 days, paid into the bank's own account.

## When the territory changes country

A bank's city can be annexed or become independent ("Lies in" in `/editjurisdictions`). The bank keeps working: its branches, tellers and accounts stay.

| What changes | Existing accounts | New accounts |
|---|---|---|
| The country | keep their OBAN, which keeps working | start with the new country's tag |
| The currency (inherited from the new country, or set) | keep the old currency forever: deposits and withdrawals in old-currency cash, transfers only to old-currency accounts | the new currency |
| Fees | paid into the bank's house account in the same currency | the same |

- **Why no conversion:** players move their own money, as after the 1990s border changes, where old accounts stayed in the old money for a while.
- **House accounts:** a bank's house account for a second currency is opened when its first fee comes in.
- **For later:** currency exchange (rates set by governments) would let players change old money into new.

**Why balances live in the database, not in loaded components:** they must be exact with several servers on one database. Every change is one database update in a transaction, so money can't be lost or created by two servers saving at once.

## Known limitations

- **No ATMs, cheques, cards, interest or loans.**
- **Local currency only,** and no currency exchange.
- **The fee cap** is fixed at 100; governments can't set it yet.
- **Property accounts** are opened by admins, because properties have no owners yet.
- **No payroll** or automatic payments.

## Deep dive

- **Code:** `Features/Banking/`.
- **Services:**
  - `IBankService`: banks, branches and tellers;
  - `IBankAccountService`: access rules, opening and closing, house accounts per currency (`GetOrOpenHouseAccount`), access lists, history;
  - `BankJurisdictionGuard`: keeps a bank's jurisdiction from being deleted (an `IJurisdictionDeletionGuard`);
  - `IBankTransactionService`: deposit, withdraw, transfer and the monthly fees;
  - `ITellerService`: whether a teller can serve a customer right now;
  - `IBankEditorService`: `/editbanks`;
  - `IBankDialogService`: the menus.
- **Database:** `IBankingDatabase` gives each operation a fresh database context. Reads use `AsNoTracking`, and money moves with `ExecuteUpdate` inside a transaction (`UPDATE … SET Balance = Balance - x WHERE … AND Balance - x >= 0`).
  - **Transactions** go through `UseInTransaction`, which runs them in EF's execution strategy. **Why:** a host whose connection retries on errors (`EnableRetryOnFailure`) refuses transactions opened directly; this way a dropped connection retries the whole transaction.
  - **Deposits** take the cash first. If crediting fails for any reason, the cash is handed back.
- **The teller's actions:** registered with `AddActorAction` (see [ActorRoleplay](actor-roleplay.md#actions)), one per form, each needing its own actor permission.
- **Fees:** `BankingSystem` runs every hour. Each fee is claimed by moving the account's `NextFeeAt` in one statement, so two servers never charge it twice.
- **Tables:** [Banking](../database.md#banking).

Want more? → [Accounts](banking/accounts.md) · [Tellers](banking/tellers.md) · [Currencies](currencies.md)
