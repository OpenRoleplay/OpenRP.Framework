using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.AccountSettingsFeature.Models.Configuration;
using OpenRP.Framework.Features.Actors.Models.Configurations;
using OpenRP.Framework.Features.Characters.Models.Configuration;
using OpenRP.Framework.Features.Currencies.Models.Configuration;
using OpenRP.Framework.Features.DroppedItems.Models.Configurations;
using OpenRP.Framework.Features.Fishing.Models;
using OpenRP.Framework.Features.Inventories.Models.Configurations;
using OpenRP.Framework.Features.Items.Models.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Extensions
{
    public static partial class ModelBuilderExtensions
    {
        public static ModelBuilder ApplyOpenRoleplayFrameworkModelConfigurations(this ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AccountSettingsModelConfiguration());

            modelBuilder.ApplyConfiguration(new CharacterModelConfiguration());

            modelBuilder.ApplyConfiguration(new CurrencyModelConfiguration());
            modelBuilder.ApplyConfiguration(new CurrencyUnitModelConfiguration());

            modelBuilder.ApplyConfiguration(new DroppedInventoryItemModelConfiguration());

            modelBuilder.ApplyConfiguration(new ActorModelConfiguration());

            modelBuilder.ApplyConfiguration(new FishLootModelConfiguration());

            modelBuilder.ApplyConfiguration(new InventoryItemModelConfiguration());

            modelBuilder.ApplyConfiguration(new ItemModelConfiguration());
            return modelBuilder;
        }
    }

}
