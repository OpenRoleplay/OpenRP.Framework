using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.Factions.Services;
using OpenRP.Framework.Features.Factions.Services.Dialogs;
using OpenRP.Framework.Features.Permissions.Entities;

namespace OpenRP.Framework.Features.Factions.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFactions(this IServiceCollection self)
        {
            return self
                .AddSingleton<IFactionManager, FactionManager>()
                .AddSingleton<IFactionPermissionManager, FactionPermissionManager>()
                .AddSingleton<IFactionRankManager, FactionRankManager>()
                .AddSingleton<IFactionRankPermissionManager, FactionRankPermissionManager>()
                .AddSingleton<IFactionMemberManager, FactionMemberManager>()
                .AddSingleton<IFactionService, FactionService>()
                .AddSingleton<IFactionRankService, FactionRankService>()
                .AddSingleton<FactionPermissionService>()
                .AddSingleton<IFactionPermissionService>(provider => provider.GetRequiredService<FactionPermissionService>())
                .AddSingleton<ICharacterPermissionSource>(provider => provider.GetRequiredService<FactionPermissionService>())
                .AddSingleton<IFactionMembershipService, FactionMembershipService>()
                .AddSingleton<IFactionChatService, FactionChatService>()
                .AddTransient<IFactionDialogService, FactionDialogService>(); // Transient because it uses ICharacterService, which uses a database context.
        }
    }
}
