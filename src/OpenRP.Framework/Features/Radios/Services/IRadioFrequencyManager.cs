using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Radios.Components;
using OpenRP.Framework.Shared.BaseManager.Helpers;

namespace OpenRP.Framework.Features.Radios.Services
{
    public interface IRadioFrequencyManager : IBaseManager
    {
        /// <summary>
        /// Inserts the frequency right away, so its unique number is claimed before another server can take it.
        /// </summary>
        RadioFrequency CreateAndLoad(RadioFrequencyModel model);
    }
}
