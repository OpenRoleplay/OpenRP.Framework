using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Maps.Components;
using OpenRP.Framework.Shared.BaseManager.Helpers;

namespace OpenRP.Framework.Features.Maps.Services
{
    public interface IMapObjectMaterialManager : IBaseManager
    {
        /// <summary>
        /// Inserts the material right away and loads it.
        /// </summary>
        MapObjectMaterial CreateAndLoad(MapObjectMaterialModel model);
    }
}
