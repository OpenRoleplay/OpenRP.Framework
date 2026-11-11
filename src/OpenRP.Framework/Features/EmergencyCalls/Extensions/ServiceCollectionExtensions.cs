using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.EmergencyCalls.Entities;
using OpenRP.Framework.Features.EmergencyCalls.Services;
using OpenRP.Framework.Features.EmergencyCalls.Services.Dialogs;
using OpenRP.Framework.Features.Phones.Extensions;
using System;

namespace OpenRP.Framework.Features.EmergencyCalls.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// 911 with an AI dispatcher that relays calls over the dispatch radio frequency (911 by default). Without an
        /// API key, callers fill in a short form instead.
        /// </summary>
        public static IServiceCollection AddEmergencyCalls(this IServiceCollection services, Action<EmergencyCallOptions> configureOptions)
        {
            services.Configure(configureOptions);

            return services
                .AddSingleton<IEmergencyDispatcherAi, EmergencyDispatcherAi>()
                .AddSingleton<IDispatchService, DispatchService>()
                .AddSingleton<IEmergencyCallDialogService, EmergencyCallDialogService>()
                // One instance: the phone network reaches it as a server number, the reply timer as itself.
                .AddSingleton<EmergencyNumber>()
                .AddServerPhoneNumber(provider => provider.GetRequiredService<EmergencyNumber>());
        }
    }
}
