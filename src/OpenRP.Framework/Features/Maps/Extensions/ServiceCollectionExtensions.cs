using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.Maps.Entities;
using OpenRP.Framework.Features.Maps.Services;
using OpenRP.Framework.Features.Maps.Services.Dialogs;

namespace OpenRP.Framework.Features.Maps.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMaps(this IServiceCollection self)
        {
            return self
                .AddSingleton<IMapDatabase, MapDatabase>()
                .AddSingleton<IMapStore, MapStore>()
                .AddSingleton<IMapService, MapService>()
                .AddSingleton<IMapEditorService, MapEditorService>()
                .AddSingleton<IMapEditingService, MapEditingService>()
                .AddSingleton<IMapBusyTracker, MapBusyTracker>()
                .AddSingleton<IMapBulkService, MapBulkService>()
                .AddSingleton<IMapDialogService, MapDialogService>();
        }

        /// <summary>
        /// Registers a handler that attaches behaviour to the map objects of the models it handles, when they spawn.
        /// </summary>
        public static IServiceCollection AddMapObjectSpawnHandler<T>(this IServiceCollection self) where T : class, IMapObjectSpawnHandler
        {
            return self.AddSingleton<IMapObjectSpawnHandler, T>();
        }
    }
}
