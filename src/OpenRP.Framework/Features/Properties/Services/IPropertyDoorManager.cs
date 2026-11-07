using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Properties.Components;
using OpenRP.Framework.Shared.BaseManager.Helpers;

namespace OpenRP.Framework.Features.Properties.Services
{
    public interface IPropertyDoorManager : IBaseManager
    {
        /// <summary>
        /// Inserts the door right away and loads it, so it can be linked to other doors immediately.
        /// </summary>
        PropertyDoor CreateAndLoad(PropertyDoorModel model);
    }
}
