using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.DroppedItems.Services;

namespace OpenRP.Framework.Features.DroppedItems.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDroppedItems(this IServiceCollection self)
        {
            // Transient because DroppedItemService uses a database context.
            return self
                .AddTransient<IDroppedItemService, DroppedItemService>();
        }
    }
}
