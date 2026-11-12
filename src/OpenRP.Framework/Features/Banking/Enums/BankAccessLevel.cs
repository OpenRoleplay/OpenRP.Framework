namespace OpenRP.Framework.Features.Banking.Enums
{
    /// <summary>
    /// What a character may do with an account, from least to most.
    /// </summary>
    public enum BankAccessLevel
    {
        None = 0,
        // Balance and history.
        View = 1,
        // Also deposit, withdraw and transfer.
        Full = 2,
        // Also close the account and change who has access: only the owner.
        Manage = 3
    }
}
