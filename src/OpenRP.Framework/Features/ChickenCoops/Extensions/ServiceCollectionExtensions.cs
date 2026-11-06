using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.ChickenCoops.Services;

namespace OpenRP.Framework.Features.ChickenCoops.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddChickenCoops(this IServiceCollection self)
        {
            return self
                .AddSingleton<IChickenCoopService, ChickenCoopService>();
        }
    }
}
