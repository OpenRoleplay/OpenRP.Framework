using OpenRP.Framework.Features.Payments.Entities;
using SampSharp.Entities;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Payments.Components
{
    /// <summary>
    /// On a player who is being asked to pay. The feature that asked awaits its result. When the player leaves, the
    /// component goes with them and the payment ends as cancelled, so nobody waits forever.
    /// </summary>
    public class PendingCheckout : Component
    {
        private readonly PaymentRequest _request;
        private readonly TaskCompletionSource<PaymentResult> _result = new TaskCompletionSource<PaymentResult>(TaskCreationOptions.RunContinuationsAsynchronously);

        public PendingCheckout(PaymentRequest request)
        {
            _request = request;
        }

        public PaymentRequest GetRequest()
        {
            return _request;
        }

        public Task<PaymentResult> GetResult()
        {
            return _result.Task;
        }

        /// <summary>
        /// Ends the payment with this result and removes the component.
        /// </summary>
        public void End(PaymentResult result)
        {
            _result.TrySetResult(result);
            Destroy();
        }

        protected override void OnDestroyComponent()
        {
            _result.TrySetResult(PaymentResult.Cancelled);
        }
    }
}
