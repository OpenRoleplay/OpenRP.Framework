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
        /// 911 with an AI dispatcher that relays calls to everyone with dispatch.receive. Without an API key, callers
        /// fill in a short form instead.
        /// </summary>
        public static IServiceCollection AddEmergencyCalls(this IServiceCollection services, Action<EmergencyCallOptions> configureOptions)
        {
            services.Configure(configureOptions);

            return services
                .AddSingleton<IEmergencyDispatcherAi, EmergencyDispatcherAi>()
                .AddSingleton<IDispatchService, DispatchService>()
                .AddSingleton<IEmergencyCallDialogService, EmergencyCallDialogService>()
                .AddServerPhoneNumber<EmergencyNumber>();
        }
    }
}
