using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.WorldTime.Entities;
using OpenRP.Framework.Features.WorldTime.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.WorldTime.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWorldTime(
            this IServiceCollection services,
            Action<WorldTimeOptions> configureOptions)
        {
            services.Configure(configureOptions);

            services.AddSingleton<IWorldTimeService, WorldTimeService>();
            return services;
        }
    }
}
