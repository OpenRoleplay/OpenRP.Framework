using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Maps.Components;
using OpenRP.Framework.Shared.BaseManager.Helpers;

namespace OpenRP.Framework.Features.Maps.Services
{
    public interface IMapObjectManager : IBaseManager
    {
        /// <summary>
        /// Inserts the object right away and loads it, so its materials can be added to it immediately.
        /// </summary>
        MapObject CreateAndLoad(MapObjectModel model);
    }
}
