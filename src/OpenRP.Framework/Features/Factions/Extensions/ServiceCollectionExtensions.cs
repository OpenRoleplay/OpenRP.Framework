using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.Factions.Services;

namespace OpenRP.Framework.Features.Factions.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFactions(this IServiceCollection self)
        {
            return self
                .AddSingleton<IFactionManager, FactionManager>()
                .AddSingleton<IFactionService, FactionService>();
        }
    }
}
