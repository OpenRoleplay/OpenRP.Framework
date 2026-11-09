using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Jurisdictions.Components;
using OpenRP.Framework.Shared.BaseManager.Helpers;

namespace OpenRP.Framework.Features.Jurisdictions.Services
{
    public interface IFactionTaxRegistrationManager : IBaseManager
    {
        /// <summary>
        /// Inserts the registration right away and loads it, so a duplicate is refused by the database at once.
        /// </summary>
        FactionTaxRegistration CreateAndLoad(FactionTaxRegistrationModel model);
    }
}
