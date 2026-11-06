using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Shared.BaseManager.Helpers;

namespace OpenRP.Framework.Features.Factions.Services
{
    public interface IFactionPermissionManager : IBaseManager
    {
        /// <summary>
        /// Inserts the faction permission right away and loads it, so granting it twice before a sync can't add it twice.
        /// </summary>
        FactionPermission CreateAndLoad(FactionPermissionModel model);
    }
}
