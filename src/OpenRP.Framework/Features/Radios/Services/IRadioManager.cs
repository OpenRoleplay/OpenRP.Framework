using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Radios.Components;
using OpenRP.Framework.Shared.BaseManager.Helpers;

namespace OpenRP.Framework.Features.Radios.Services
{
    public interface IRadioManager : IBaseManager
    {
        /// <summary>
        /// Inserts the radio right away and loads it, so its item can point to its id.
        /// </summary>
        Radio CreateAndLoad(RadioModel model);
    }
}
