using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Properties.Components;
using OpenRP.Framework.Shared.BaseManager.Helpers;

namespace OpenRP.Framework.Features.Properties.Services
{
    public interface IPropertyManager : IBaseManager
    {
        /// <summary>
        /// Inserts the property right away and loads it, so doors can be added to it immediately.
        /// </summary>
        Property CreateAndLoad(PropertyModel model);
    }
}
