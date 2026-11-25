using OpenRP.Framework.Features.Payments.Enums;

namespace OpenRP.Framework.Features.Payments.Entities
{
    /// <summary>
    /// How a payment ended. The error says why it failed; the player has already been told.
    /// </summary>
    public record PaymentResult(PaymentOutcome Outcome, string? Error = null)
    {
        public static readonly PaymentResult Paid = new PaymentResult(PaymentOutcome.Paid);
        public static readonly PaymentResult Cancelled = new PaymentResult(PaymentOutcome.Cancelled);

        public static PaymentResult Failed(string error) => new PaymentResult(PaymentOutcome.Failed, error);

        public bool IsPaid => Outcome == PaymentOutcome.Paid;
    }
}
