using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Shared.BaseManager.Helpers;

namespace OpenRP.Framework.Features.Factions.Services
{
    public interface IFactionRankPermissionManager : IBaseManager
    {
        /// <summary>
        /// Inserts the rank permission right away and loads it, so toggling it twice before a sync can't add it twice.
        /// </summary>
        FactionRankPermission CreateAndLoad(FactionRankPermissionModel model);
    }
}
