using Microsoft.Extensions.DependencyInjection;
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
                .AddSingleton<IActorEditorService, ActorEditorService>()
                .AddSingleton<IActorDialogService, ActorDialogService>();
        }
    }
}
