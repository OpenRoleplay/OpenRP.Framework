using OpenRP.Framework.Features.Currencies.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Currencies.Systems
{
    public class PaymentSystem : ISystem
    {
        // The time between two steps of a hand-over, so the /me messages and animations can be followed.
        private const int PaymentStepIntervalInMilliseconds = 2500;

        [Timer(PaymentStepIntervalInMilliseconds)]
        public async void ContinueAcceptedPayments(IPaymentService paymentService)
        {
            // PaymentService catches and logs errors per payment, so nothing is lost in this async void.
            await paymentService.ContinueAcceptedPayments();
        }

        [Event]
        public void OnPlayerDisconnect(Player player, DisconnectReason disconnectReason, IPaymentService paymentService)
        {
            paymentService.CancelPayment(player);
        }
    }
}
