using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.Properties.Services;
using OpenRP.Framework.Features.Properties.Services.Dialogs;

namespace OpenRP.Framework.Features.Properties.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProperties(this IServiceCollection self)
        {
            return self
                .AddSingleton<IPropertyService, PropertyService>()
                .AddSingleton<IPropertyManager, PropertyManager>()
                .AddSingleton<IPropertyDoorManager, PropertyDoorManager>()
                .AddSingleton<IPropertyEditorService, PropertyEditorService>()
                .AddSingleton<IPropertyDefaultDataExporter, PropertyDefaultDataExporter>()
                .AddSingleton<IPropertyDialogService, PropertyDialogService>();
        }
    }
}
