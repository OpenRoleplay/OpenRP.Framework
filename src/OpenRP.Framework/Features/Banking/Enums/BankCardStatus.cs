namespace OpenRP.Framework.Features.Banking.Enums
{
    public enum BankCardStatus
    {
        Active = 1,
        // Replaced by a new card; never works again.
        Cancelled = 2,
        // By its owner or by too many wrong PINs; the bank can unblock it.
        Blocked = 3,
        AccountClosed = 4
    }
}
