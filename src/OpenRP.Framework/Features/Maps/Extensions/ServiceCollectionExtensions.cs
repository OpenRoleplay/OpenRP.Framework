using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.Maps.Services;
using OpenRP.Framework.Features.Maps.Services.Dialogs;

namespace OpenRP.Framework.Features.Maps.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMaps(this IServiceCollection self)
        {
            return self
                .AddSingleton<IMapService, MapService>()
                .AddSingleton<IMapManager, MapManager>()
                .AddSingleton<IMapObjectManager, MapObjectManager>()
                .AddSingleton<IMapObjectMaterialManager, MapObjectMaterialManager>()
                .AddSingleton<IMapRemovedBuildingManager, MapRemovedBuildingManager>()
                .AddSingleton<IMapEditorService, MapEditorService>()
                .AddSingleton<IMapEditingService, MapEditingService>()
                .AddSingleton<IMapDialogService, MapDialogService>();
        }
    }
}
