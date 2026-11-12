namespace OpenRP.Framework.Features.Banking.Enums
{
    /// <summary>
    /// What a customer can do at a teller; each needs its own actor permission.
    /// </summary>
    public enum BankOperation
    {
        View = 1,
        OpenAccount = 2,
        CloseAccount = 3,
        Deposit = 4,
        Withdraw = 5,
        Transfer = 6,
        ManageAccess = 7
    }
}
