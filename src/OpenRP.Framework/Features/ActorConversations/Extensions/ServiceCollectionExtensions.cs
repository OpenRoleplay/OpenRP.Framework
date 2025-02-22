using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.ActorConversations.Entities;
using OpenRP.Framework.Features.ActorConversations.Services;
using OpenRP.Framework.Features.Actors.Services;
using OpenRP.Framework.Features.Discord.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.ActorConversations.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddActorConversations(this IServiceCollection services, Action<ActorConversationOptions> configureOptions)
        {
            // Configure the options using the provided delegate
            services.Configure(configureOptions);

            return services
                .AddSingleton<IActorConversationWithPlayerManager, ActorConversationWithPlayerManager>();
        }
    }
}
