using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.ActorConversations.Entities;
using OpenRP.Framework.Features.ActorConversations.Services;

namespace OpenRP.Framework.Features.ActorConversations.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// AI-written conversations between actors that players watch, such as the main menu scene. Each turn is
        /// written by ActorRoleplay's generator, so the host must call AddActorRoleplay too.
        /// </summary>
        public static IServiceCollection AddActorConversations(this IServiceCollection services, Action<ActorConversationOptions> configureOptions)
        {
            services.Configure(configureOptions);

            return services
                .AddSingleton<IActorConversationService, ActorConversationService>();
        }
    }
}
