using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.Animations.Services;
using OpenRP.Framework.Features.CDN.Entities;
using OpenRP.Framework.Features.CDN.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.CDN.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCDN(
            this IServiceCollection services,
            Action<OpenCdnOptions> configureOptions
        )
        {
            // Configure the options using the provided delegate
            services.Configure(configureOptions);

            // Register the CDN service as a singleton
            services.AddSingleton<IOpenCdnService, OpenCdnService>();

            return services;
        }
    }
}
