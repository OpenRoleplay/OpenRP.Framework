namespace OpenRP.Framework.Features.Payments.Enums
{
    public enum PaymentOutcome
    {
        Paid = 1,
        // The player chose not to pay, or left; nothing was paid.
        Cancelled = 2,
        // The payment was refused, for example a declined card; nothing was paid.
        Failed = 3
    }
}
