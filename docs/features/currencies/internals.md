# Currencies: internals

How money is built in code (`Features/Currencies/`). For what players see, see [Currencies](../currencies.md).

## Units

Each currency has **units**, the denominations (`CurrencyUnits`, for example $20.00). Each unit is an item of type `Currency`, whose use value holds its unit id.
- **Seeded units:** 29, for SAD (15) and CRU (14).
- **Seeded currencies:** SAD is id 1, CRU id 2 (shown with a `CRU ` prefix), USD id 3.

## Services

**`ICurrencyService`**

| Method | Does |
|---|---|
| `GetCurrencyByCode(code)` | The currency; the code is case-insensitive |
| `GetCurrencyFormat(id, amount)` | The amount as text, for example "$12.50" |
| `GetCharacterCurrencyAmount(character, id)` | How much is in the wallet |
| `GiveCharacterCurrency` / `TakeCharacterCurrency` | Return a `CurrencyChangeResult`: `Success`, `NoWallet`, `NotEnoughFunds`, `NotEnoughSpace` or `AmountNotPayableInBills` |
| `GetExactBillsFromWallet(wallet, id, amount)` | Bills that add up exactly, or null |
| `TransferBills(from, to, bills)` | False if the bills aren't there or don't fit |
| `SetPlayerMoneyToCharacterCurrency(player, character)` | Updates the GTA money counter |

**`IPaymentService`**: `RequestPayment`, `AcceptPayment`, `ContinueAcceptedPayments` (one step) and `CancelPayment`.

## State and timing

- **Payments:** `PendingPayment` is a component per payment. It holds the sender, receiver, amount, whose turn it is and the current bill.
- **Bill stacks:** `BillStack` is a number of bills of one denomination, for example 3 × $20.
- **`PaymentSystem`** advances payments every 2.5 s, and cancels them on disconnect.
- **`CurrencyServerSystem`** updates the GTA money counter on:
  - [`OnCharacterSelected`](../../server-events.md);
  - [`OnPlayerCashUpdate`](../../server-events.md), when it's the default currency.

## Events

`OnPlayerCashUpdate` is published by the currency and payment services, and by picking up dropped money.

## Tables

[`Currencies`, `CurrencyUnits`](../../database.md#currencies-currencymodel-and-currencyunits-currencyunitmodel), plus the currency items in `Items`.

Want more? → [Currencies](../currencies.md) · [Items](../items.md)
