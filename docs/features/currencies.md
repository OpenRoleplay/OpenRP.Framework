# Currencies

Money is physical: bills and coins, carried as items in your wallet, or loose in your pockets. Paying someone means handing over actual bills.

**Why:** cash you can see, count, drop, lose or have stolen creates roleplay. Handing it over bill by bill makes a payment an RP moment rather than an instant transfer.

## How to use it

| Command | Groups | Does |
|---|---|---|
| `/pay [player] [currency code] [amount]` | Default | Offer someone money; they must accept |
| `/acceptpay` | Default | Accept the payment offered to you |

`/givemoney` (Admin) is in [Admins](admins.md).

**Currencies:**

| Code | Name | Shown as |
|---|---|---|
| SAD | San Andreas Dollar | `$12.50` |
| CRU | Caeroyna Ruble | `CRU 12.50` |
| USD | United States Dollar | `$12.50` |

## How it works

1. **Where your money is.** It's in your default wallet, otherwise your first wallet, otherwise loose in your inventory.
2. **Getting money.** It's paid out in as few bills as possible.
3. **Spending.** When you don't have the exact bills, a larger bill is broken and you get change.
4. **Paying someone:**
   - `/pay` checks that you have the **exact** bills, and that you're within 3 units;
   - `/acceptpay` checks the distance again;
   - then one bill changes hands every 2.5 seconds, with `/me` messages.
5. **The GTA money counter** shows your money in your default currency.

## Known limitations

- **Cash only:** no bank accounts or card payments yet.
- **USD has no bills seeded,** so it can't be held.
- **Cancelling.** A payment is only cancelled by a disconnect or an error. Bills already handed over stay with the receiver.

Want more? → [How it's built](currencies/internals.md) · [Inventories](inventories.md)
