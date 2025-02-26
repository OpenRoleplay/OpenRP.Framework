using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Database.Extensions;
using OpenRP.Framework.Features.Accounts.Extensions;
using OpenRP.Framework.Features.AccountSettingsFeature.Extensions;
using OpenRP.Framework.Features.ActorConversations.Extensions;
using OpenRP.Framework.Features.Actors.Extensions;
using OpenRP.Framework.Features.Animations.Extensions;
using OpenRP.Framework.Features.Characters.Extensions;
using OpenRP.Framework.Features.DebugSettingsFeature.Extensions;
using OpenRP.Framework.Features.Fishing.Extensions;
using OpenRP.Framework.Features.Inventories.Extensions;
using OpenRP.Framework.Features.Items.Extensions;
using OpenRP.Framework.Features.MainMenu.Extensions;
using OpenRP.Framework.Features.Permissions.Extensions;
using OpenRP.Framework.Features.Vehicles.Extensions;
using OpenRP.Framework.Features.VirtualWorlds.Extensions;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Framework.Shared.Commands.Extensions;
using OpenRP.Framework.Shared.Logging.Extensions;
using OpenRP.Framework.Shared.ServerEvents.Extensions;
using SampSharp.ColAndreas.Entities.Services;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOpenRoleplayFramework(this IServiceCollection self)
        {
            return self
                .AddServerLogging()
                .AddDatabase()
                .AddActors()
                .AddAnimations()
                .AddVirtualWorldManager()
                .AddCharacters()
                .AddInventories()
                .AddAccounts()
                .AddAccountSettings()
                .AddChat()
                .AddPermissions()
                .AddFishing()
                .AddVehicles()
                .AddMainMenu()
                .AddItems()
                .AddDebugSettings()
                .AddCommands()
                .AddServerSystemEvents()
                .AddSystemsInAssembly();
        }
    }
}
