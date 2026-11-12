# Bank accounts

An account belongs to one owner, holds money in its bank's local currency, and is reached by its OBAN.

## OBAN

`SA42 PALO 0000 1234 56`: the 2-letter tag of the bank's country (as on licence plates), 2 check digits, the bank's 4-letter code and 10 random digits.

- **The check digits** work like an IBAN's (mod 97), so a mistyped OBAN is caught before money moves.
- **Why random digits:** they don't give away how many accounts exist.
- **An OBAN is never reused,** also after its account is closed.
- **An OBAN never changes,** also when the bank's territory later belongs to another country. See [Banking](../banking.md#when-the-territory-changes-country).

## Who may do what

| Owner | View (balance, history) | Full use (also deposit, withdraw, transfer) | Close and manage access |
|---|---|---|---|
| Character | owner, access list | owner, full-access list | owner |
| Faction | the leader and members with `faction.bank`, access list | the same | the faction leader |
| Property | access list | full-access list | admins only |
| Bank (its own account) | the running faction's leader and members with `faction.bank` | the same | admins only |

- **Access** is given by the owner at a teller, by full character name, as view only or full use.
- **Opening:** for yourself, or for a faction you have `faction.bank` in. Admins open property accounts in `/editbanks`.

## Money rules

- **Currency:** an account holds its bank's local currency. Deposits and withdrawals only take or give cash in that currency, and transfers only go between accounts in the same currency.
- **Amounts** are above 0, with at most 2 decimals.
- **Withdrawals and transfers** need the money to be there. An account never goes below 0 by spending.
- **Monthly fee:** every 30 days after opening, the bank's fee (at most 100) goes from the account to the bank's own account in the same currency.
  - **If the balance can't cover it,** the account goes negative, but never below -1000. A fee that would go further is only charged down to -1000.
  - **While negative,** nothing can be withdrawn or transferred out. Deposits and incoming transfers pay it back.
  - **Why only the fee:** spending into the negative would create money from nothing, with no way to collect the debt. A negative balance from fees can't be spent.
- **Closing:** by the owner, only at a balance of exactly 0. A closed account keeps its history.

Want more? → [Banking](../banking.md) · [Tellers](tellers.md)
