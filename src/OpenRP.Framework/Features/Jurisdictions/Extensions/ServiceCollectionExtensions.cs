using Microsoft.Extensions.DependencyInjection;
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
    }
}
