using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.Fishing.Services;
using OpenRP.Framework.Features.Fishing.Services.Dialogs;

namespace OpenRP.Framework.Features.Fishing.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFishing(this IServiceCollection self)
        {
            return self
                .AddSingleton<IFishService, FishService>()
                .AddSingleton<IFishHotspotService, FishHotspotService>()
                .AddSingleton<IFishingService, FishingService>()
                .AddSingleton<IFishingNetManager, FishingNetManager>()
                .AddSingleton<IFishingNetService, FishingNetService>()
                .AddSingleton<ITrawlService, TrawlService>()
                .AddSingleton<IFishingNetDialogService, FishingNetDialogService>();
        }
    }
}
