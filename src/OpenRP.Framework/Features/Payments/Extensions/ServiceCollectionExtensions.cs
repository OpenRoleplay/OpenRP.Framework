using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.Payments.Services;
using OpenRP.Framework.Features.Payments.Services.Dialogs;

namespace OpenRP.Framework.Features.Payments.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPayments(this IServiceCollection self)
        {
            return self
                .AddSingleton<ICheckoutService, CheckoutService>()
                .AddSingleton<IPaymentDialogService, PaymentDialogService>();
        }
    }
}
