namespace OpenRP.Framework.Features.Banking.Enums
{
    public enum BankTransactionType
    {
        Deposit = 1,
        Withdrawal = 2,
        TransferIn = 3,
        TransferOut = 4,
        Fee = 5,
        FeeIncome = 6,
        // Money out of the payer's account for a card payment.
        CardPayment = 7,
        // A payment received, by card or as cash paid in for the receiver.
        PaymentReceived = 8
    }
}
