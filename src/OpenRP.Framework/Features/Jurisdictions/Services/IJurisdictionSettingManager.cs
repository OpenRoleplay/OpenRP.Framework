using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Jurisdictions.Components;
using OpenRP.Framework.Shared.BaseManager.Helpers;

namespace OpenRP.Framework.Features.Jurisdictions.Services
{
    public interface IJurisdictionSettingManager : IBaseManager
    {
        /// <summary>
        /// Inserts the setting right away and loads it, so a duplicate is refused by the database at once.
        /// </summary>
        JurisdictionSetting CreateAndLoad(JurisdictionSettingModel model);
    }
}
