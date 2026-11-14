using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Maps.Components;
using OpenRP.Framework.Shared.BaseManager.Helpers;

namespace OpenRP.Framework.Features.Maps.Services
{
    public interface IMapManager : IBaseManager
    {
        /// <summary>
        /// Inserts the map right away and loads it, so objects can be added to it immediately.
        /// </summary>
        Map CreateAndLoad(MapModel model);
    }
}
