using OpenRP.Framework.Features.Payments.Entities;
using SampSharp.Entities.SAMP;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Payments.Services
{
    /// <summary>
    /// The one way features take money from a player: shops, rent, fines, fees. The player pays cash or with a debit
    /// card, and the money always lands in a bank account. (Handing cash to another player is /pay, in Currencies.)
    /// </summary>
    public interface ICheckoutService
    {
        /// <summary>
        /// Asks the player to pay, and completes when they paid, cancelled, failed, or left. The player is told why a
        /// payment failed; telling them what they paid for is up to the caller. After awaiting it, check that the
        /// player is still alive before using them.
        /// </summary>
        Task<PaymentResult> RequestPaymentAsync(Player player, PaymentRequest request);
    }
}
