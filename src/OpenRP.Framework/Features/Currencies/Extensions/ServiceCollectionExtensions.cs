using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.Currencies.Services;

namespace OpenRP.Framework.Features.Currencies.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCurrencies(this IServiceCollection self)
        {
            return self
                .AddSingleton<ICurrencyService, CurrencyService>()
                .AddSingleton<IPaymentService, PaymentService>();
        }
    }
}
