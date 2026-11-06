using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Shared.BaseManager.Helpers;

namespace OpenRP.Framework.Features.Factions.Services
{
    public interface IFactionRankManager : IBaseManager
    {
        /// <summary>
        /// Inserts the rank right away and loads it, so its id can be used immediately.
        /// </summary>
        FactionRank CreateAndLoad(FactionRankModel model);
    }
}
