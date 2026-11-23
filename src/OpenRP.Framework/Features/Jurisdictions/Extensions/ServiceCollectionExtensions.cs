using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.Jurisdictions.Entities;
using OpenRP.Framework.Features.Jurisdictions.Services;
using OpenRP.Framework.Features.Jurisdictions.Services.Dialogs;
using System.Linq;

namespace OpenRP.Framework.Features.Jurisdictions.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJurisdictions(this IServiceCollection self)
        {
            return self
                .AddSingleton<IJurisdictionService, JurisdictionService>()
                .AddSingleton<IJurisdictionManager, JurisdictionManager>()
                .AddSingleton<IFactionTaxRegistrationManager, FactionTaxRegistrationManager>()
                .AddSingleton<IJurisdictionSettingManager, JurisdictionSettingManager>()
                .AddSingleton<IJurisdictionSettingService, JurisdictionSettingService>()
                .AddSingleton<IJurisdictionEditorService, JurisdictionEditorService>()
                .AddSingleton<IJurisdictionDialogService, JurisdictionDialogService>();
        }

        /// <summary>
        /// Lets a feature keep jurisdictions it depends on from being deleted.
        /// </summary>
        public static IServiceCollection AddJurisdictionDeletionGuard<TGuard>(this IServiceCollection self)
            where TGuard : class, IJurisdictionDeletionGuard
        {
            return self.AddSingleton<IJurisdictionDeletionGuard, TGuard>();
        }

        /// <summary>
        /// Registers a jurisdiction setting, so the settings menus and the setting service can read it. Registering the
        /// same setting again does nothing.
        /// </summary>
        public static IServiceCollection AddJurisdictionSetting<TSetting>(this IServiceCollection self)
            where TSetting : class, IJurisdictionSetting
        {
            if (self.Any(descriptor => descriptor.ServiceType == typeof(TSetting)))
            {
                return self;
            }
            return self
                .AddSingleton<TSetting>()
                .AddSingleton<IJurisdictionSetting>(provider => provider.GetRequiredService<TSetting>());
        }
    }
}
