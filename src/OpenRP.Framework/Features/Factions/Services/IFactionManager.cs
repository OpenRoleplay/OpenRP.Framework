using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Shared.BaseManager.Helpers;

namespace OpenRP.Framework.Features.Factions.Services
{
    public interface IFactionManager : IBaseManager
    {
        /// <summary>
        /// Inserts the faction right away and loads it, so its top rank can be created immediately.
        /// </summary>
        Faction CreateAndLoad(FactionModel model);
    }
}
