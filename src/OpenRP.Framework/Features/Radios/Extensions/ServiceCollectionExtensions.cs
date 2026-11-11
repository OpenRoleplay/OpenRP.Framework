using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.Radios.Services;
using OpenRP.Framework.Features.Radios.Services.Dialogs;

namespace OpenRP.Framework.Features.Radios.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRadios(this IServiceCollection self)
        {
            return self
                .AddSingleton<IRadioManager, RadioManager>()
                .AddSingleton<IRadioFrequencyManager, RadioFrequencyManager>()
                .AddSingleton<IRadioFrequencyFactionManager, RadioFrequencyFactionManager>()
                .AddSingleton<IRadioService, RadioService>()
                .AddSingleton<IRadioFrequencyService, RadioFrequencyService>()
                .AddSingleton<IRadioBroadcastService, RadioBroadcastService>()
                .AddSingleton<IRadioEditorService, RadioEditorService>()
                .AddSingleton<IRadioDialogService, RadioDialogService>();
        }
    }
}
