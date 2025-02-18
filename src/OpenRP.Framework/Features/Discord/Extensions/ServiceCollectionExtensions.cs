using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.CDN.Entities;
using OpenRP.Framework.Features.CDN.Services;
using OpenRP.Framework.Features.Discord.Entities;
using OpenRP.Framework.Features.Discord.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Discord.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDiscord(
            this IServiceCollection services,
            Action<DiscordOptions> configureOptions
        )
        {
            // Configure the options using the provided delegate
            services.Configure(configureOptions);

            // Register the CDN service as a singleton
            services.AddSingleton<IDiscordService, DiscordService>();

            return services;
        }
    }
}
