using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Maps.Components;
using OpenRP.Framework.Shared.BaseManager.Helpers;

namespace OpenRP.Framework.Features.Maps.Services
{
    public interface IMapRemovedBuildingManager : IBaseManager
    {
        /// <summary>
        /// Inserts the removed building right away and loads it.
        /// </summary>
        MapRemovedBuilding CreateAndLoad(MapRemovedBuildingModel model);
    }
}
