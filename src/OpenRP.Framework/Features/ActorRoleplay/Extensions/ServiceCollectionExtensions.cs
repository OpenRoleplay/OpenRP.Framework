using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.ActorRoleplay.Entities;
using OpenRP.Framework.Features.ActorRoleplay.Services;

namespace OpenRP.Framework.Features.ActorRoleplay.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// AI actors that reply to /me, /do and /say around them. Every host must call this, because the feature's
        /// systems are always registered; without an API key, actors simply don't reply.
        /// </summary>
        public static IServiceCollection AddActorRoleplay(this IServiceCollection services, Action<ActorRoleplayOptions> configureOptions)
        {
            services.Configure(configureOptions);

            return services
                .AddSingleton<IActorReplyGenerator, ActorReplyGenerator>()
                .AddSingleton<IActorRoleplayService, ActorRoleplayService>();
        }

        /// <summary>
        /// Lets AI actors with its permission perform this action for the players they talk with.
        /// </summary>
        public static IServiceCollection AddActorAction(this IServiceCollection services, Func<IServiceProvider, IActorAction> createAction)
        {
            return services.AddSingleton(createAction);
        }
    }
}
