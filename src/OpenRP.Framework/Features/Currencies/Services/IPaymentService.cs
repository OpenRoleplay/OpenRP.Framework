using SampSharp.Entities.SAMP;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Currencies.Services
{
    public interface IPaymentService
    {
        /// <summary>
        /// Asks the receiver to accept a hand-to-hand payment, after checking that the sender has the exact bills.
        /// </summary>
        void RequestPayment(Player sender, Player receiver, string currencyCode, decimal amount);

        /// <summary>
        /// Accepts the payment offered to this player, which starts the hand-over.
        /// </summary>
        void AcceptPayment(Player receiver);

        /// <summary>
        /// Moves every accepted payment one step further: the next bill is handed over or taken.
        /// </summary>
        Task ContinueAcceptedPayments();

        /// <summary>
        /// Stops the payment this player sends or receives. Bills already handed over stay with the receiver.
        /// </summary>
        void CancelPayment(Player player);
    }
}
