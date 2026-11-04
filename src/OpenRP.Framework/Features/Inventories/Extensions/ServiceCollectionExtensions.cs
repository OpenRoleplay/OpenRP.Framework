using Microsoft.Extensions.DependencyInjection;
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
                .AddTransient<IInventoryService, InventoryService>()
                .AddSingleton<IInventoryItemManager, InventoryItemManager>()
                .AddSingleton<IInventoryManager, InventoryManager>()
                .AddTransient<IInventoryDialogService, InventoryDialogService>(); // Transient because it uses IDroppedItemService, which uses a database context.
        }
    }
}
