using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.Actors.Entities;
using OpenRP.Framework.Features.Actors.Services;
using OpenRP.Framework.Features.Actors.Services.Dialogs;

namespace OpenRP.Framework.Features.Actors.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddActors(this IServiceCollection self)
        {
            return self
                .AddSingleton<IActorService, ActorService>()
                .AddSingleton<IActorManager, ActorManager>()
                .AddSingleton<IActorPermissionManager, ActorPermissionManager>()
                .AddSingleton<IActorPermissionService, ActorPermissionService>()
                .AddSingleton<IActorEditorService, ActorEditorService>()
                .AddSingleton<IActorDialogService, ActorDialogService>();
        }

        /// <summary>
        /// Offers a permission for actors, which admins switch on per actor in /editactors. Actors have none by default.
        /// </summary>
        public static IServiceCollection AddActorPermission(this IServiceCollection self, string name, string description)
        {
            return self.AddSingleton(new ActorPermissionDefinition(name, description));
        }
    }
}
