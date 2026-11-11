using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Database.Extensions;
using OpenRP.Framework.Features.Accounts.Extensions;
using OpenRP.Framework.Features.AccountSettingsFeature.Extensions;
using OpenRP.Framework.Features.ActorConversations.Extensions;
using OpenRP.Framework.Features.Actors.Extensions;
using OpenRP.Framework.Features.Animations.Extensions;
using OpenRP.Framework.Features.BiomeGenerator.Extensions;
using OpenRP.Framework.Features.Characters.Extensions;
using OpenRP.Framework.Features.ChickenCoops.Extensions;
using OpenRP.Framework.Features.Currencies.Extensions;
using OpenRP.Framework.Features.DroppedItems.Extensions;
using OpenRP.Framework.Features.Factions.Extensions;
using OpenRP.Framework.Features.Skills.Extensions;
using OpenRP.Framework.Features.Harvestables.Extensions;
using OpenRP.Framework.Features.DebugSettingsFeature.Extensions;
using OpenRP.Framework.Features.Fishing.Extensions;
using OpenRP.Framework.Features.Inventories.Extensions;
using OpenRP.Framework.Features.Items.Extensions;
using OpenRP.Framework.Features.Jurisdictions.Extensions;
using OpenRP.Framework.Features.MainMenu.Extensions;
using OpenRP.Framework.Features.Permissions.Extensions;
using OpenRP.Framework.Features.Phones.Extensions;
using OpenRP.Framework.Features.Radios.Extensions;
using OpenRP.Framework.Features.Properties.Extensions;
using OpenRP.Framework.Features.Vehicles.Extensions;
using OpenRP.Framework.Features.VirtualWorlds.Extensions;
using OpenRP.Framework.Shared.Batteries.Extensions;
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
                .AddAccounts()
                .AddAccountSettings()
                .AddChat()
                .AddPermissions()
                .AddFishing()
                .AddVehicles()
                .AddMainMenu()
                .AddItems()
                .AddInventories()
                .AddCurrencies()
                .AddDroppedItems()
                .AddSkills()
                .AddHarvestables()
                .AddProperties()
                .AddChickenCoops()
                .AddFactions()
                .AddJurisdictions()
                .AddBatteries()
                .AddPhones()
                .AddRadios()
                .AddDebugSettings()
                .AddBiomeGenerator()
                .AddCommands()
                .AddServerSystemEvents()
                // Named explicitly: the calling assembly isn't reliable once methods get inlined.
                .AddSystemsInAssembly(typeof(ServiceCollectionExtensions).Assembly);
        }
    }
}
