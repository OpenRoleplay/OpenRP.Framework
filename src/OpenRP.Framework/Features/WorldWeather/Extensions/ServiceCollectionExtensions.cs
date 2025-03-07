using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.WorldWeather.Entities;
using OpenRP.Framework.Features.WorldWeather.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.WorldWeather.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWorldWeather(
            this IServiceCollection services,
            Action<WorldWeatherOptions> configureOptions)
        {
            return services
                .Configure(configureOptions)
                .AddSingleton<IWorldWeatherService, WorldWeatherService>()
                .AddSingleton<IWorldWindDirectionService, WorldWindDirectionService>();
        }
    }
}
