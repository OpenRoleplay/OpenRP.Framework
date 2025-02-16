using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.Fishing.Services;
using OpenRP.Framework.Features.Inventories.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Fishing.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFishing(this IServiceCollection self)
        {
            return self
                .AddSingleton<IFishService, FishService>()
                .AddSingleton<IFishHotspotService, FishHotspotService>();
        }
    }
}
