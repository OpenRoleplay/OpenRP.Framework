using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Shared.Batteries.Services;

namespace OpenRP.Framework.Shared.Batteries.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBatteries(this IServiceCollection self)
        {
            return self.AddSingleton<IBatteryPlaceService, BatteryPlaceService>();
        }
    }
}
