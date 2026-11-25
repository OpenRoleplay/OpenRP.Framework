using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.Inventories.Entities;
using OpenRP.Framework.Features.Inventories.Services;
using OpenRP.Framework.Features.Inventories.Services.Dialogs;
using OpenRP.Framework.Shared.BaseManager.Helpers;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Inventories.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInventories(this IServiceCollection self)
        {
            return self
                .AddSingleton<IInventoryItemManager, InventoryItemManager>()
                .AddSingleton<IInventoryManager, InventoryManager>()
                .AddTransient<IInventoryDialogService, InventoryDialogService>(); // Transient because it uses IDroppedItemService, which uses a database context.
        }

        /// <summary>
        /// Registers a provider that gives the items it handles their own name in the inventory.
        /// </summary>
        public static IServiceCollection AddInventoryItemNameProvider<T>(this IServiceCollection self) where T : class, IInventoryItemNameProvider
        {
            return self.AddSingleton<IInventoryItemNameProvider, T>();
        }
    }
}
