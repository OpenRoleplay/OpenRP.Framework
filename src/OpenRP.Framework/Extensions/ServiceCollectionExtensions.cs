using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.Accounts.Extensions;
using OpenRP.Framework.Features.AccountSettingsFeature.Extensions;
using OpenRP.Framework.Features.Animations.Extensions;
using OpenRP.Framework.Features.Characters.Extensions;
using OpenRP.Framework.Features.Inventories.Extensions;
using OpenRP.Framework.Features.VirtualWorlds.Extensions;
using OpenRP.Framework.Shared.Chat.Extensions;
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
                .AddAnimations()
                .AddVirtualWorldManager()
                .AddCharacters()
                .AddInventories()
                .AddAccounts()
                .AddAccountSettings()
                .AddChat()
                .AddServerSystemEvents()
                .AddSystemsInAssembly();
        }
    }
}
