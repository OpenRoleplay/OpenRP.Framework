using OpenRP.Framework.Features.Payments.Entities;
using SampSharp.Entities.SAMP;
using System;

namespace OpenRP.Framework.Features.Payments.Services.Dialogs
{
    public interface IPaymentDialogService
    {
        /// <summary>
        /// "Pay $X for ...": cash or card, as far as the request allows them. A disconnect calls nothing.
        /// </summary>
        void OpenPaymentMethodChoice(Player player, PaymentRequest request, Action onCash, Action onCard, Action onCancelled);
    }
}
