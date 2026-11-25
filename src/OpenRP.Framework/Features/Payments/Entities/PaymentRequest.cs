namespace OpenRP.Framework.Features.Payments.Entities
{
    /// <summary>
    /// What a feature asks a player to pay: how much, in which currency, into which bank account, and what for. The
    /// description shows in the dialog and in both bank histories, for example "Groceries at the 24/7".
    /// </summary>
    public record PaymentRequest(decimal Amount, ulong CurrencyId, ulong ReceiverAccountId, string Description)
    {
        public bool AllowsCash { get; init; } = true;
        public bool AllowsCard { get; init; } = true;
    }
}
