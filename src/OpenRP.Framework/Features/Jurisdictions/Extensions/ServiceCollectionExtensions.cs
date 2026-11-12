using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.Jurisdictions.Entities;
using OpenRP.Framework.Features.Jurisdictions.Services;
using OpenRP.Framework.Features.Jurisdictions.Services.Dialogs;

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
    }
}
