# Payments

The one way features take money from a player, such as shops, rent, fines or fees. The player pays cash or with a [debit card](banking/cards-and-atms.md), and the money always lands in a bank account.

Handing cash to another player is something else: that's `/pay`, in [Currencies](currencies.md).

## How to use it

```csharp
PaymentResult result = await checkoutService.RequestPaymentAsync(player,
    new PaymentRequest(25m, currencyId, shopAccountId, "groceries"));
if (!player.IsComponentAlive)
    return;
if (result.IsPaid)
    GiveGroceries(player);
```

| `PaymentRequest` | Meaning |
|---|---|
| `Amount`, `CurrencyId` | What to pay |
| `ReceiverAccountId` | The bank account that receives it |
| `Description` | Shown to the player ("Pay $25.00 for groceries") and in both bank histories |
| `AllowsCash`, `AllowsCard` | Both true by default |

| `PaymentResult.Outcome` | Meaning |
|---|---|
| `Paid` | The money is on the receiving account |
| `Cancelled` | The player chose not to pay, or left. Nothing was paid |
| `Failed` | Refused, for example a declined card. The player has been told why. Nothing was paid |

`/testpayment [amount] [OBAN]` (Tester, Admin) asks you to pay into an account, to try it out.

## How it works

1. The player chooses **Cash** or **Debit card**.
2. **Cash:** the bills come from their wallet, with change, and are paid into the receiving account. If the account can't take it, the cash is handed back.
3. **Card:** they choose one of their cards in the payment's currency and type the PIN. One database transaction takes the money from the card's account and puts it on the receiving account, or does nothing.
4. The task completes with the result. A `PendingCheckout` component on the player holds it; if the player leaves, the component goes and the payment ends as cancelled.

**Why one service:** every rule (cards, PINs, currencies, failures) is written once, and every feature behaves the same.

**Why the receiver is always a bank account:** cash a shop takes is paid into its account straight away, so money never piles up in places nobody can see.

## Known limitations

- **No feature uses it yet.** Shops, rent and fines come later.
- **One payment at a time** per player.

## Deep dive

- **Code:** `Features/Payments/` (`ICheckoutService`, `IPaymentDialogService`, `PendingCheckout`); the money moves in Banking's `IBankTransactionService.PayWithCard` and `PayInCashAsync`.
- **Bank history:** the payer's side is a `CardPayment`, the receiver's a `PaymentReceived`, both with the description and the card.

Want more? → [Debit cards and ATMs](banking/cards-and-atms.md) · [Banking](banking.md)
