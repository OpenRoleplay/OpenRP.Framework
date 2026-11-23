using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.FlyMode.Services;

namespace OpenRP.Framework.Features.FlyMode.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFlyMode(this IServiceCollection self)
        {
            return self
                .AddSingleton<IFlyModeService, FlyModeService>()
                .AddSingleton<IFlyModeMovementService, FlyModeMovementService>()
                .AddSingleton<IFlyModeRestoreService, FlyModeRestoreService>();
        }
    }
}
