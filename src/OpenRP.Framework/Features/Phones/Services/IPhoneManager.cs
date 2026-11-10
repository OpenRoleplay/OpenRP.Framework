using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Phones.Components;
using OpenRP.Framework.Shared.BaseManager.Helpers;

namespace OpenRP.Framework.Features.Phones.Services
{
    public interface IPhoneManager : IBaseManager
    {
        /// <summary>
        /// Inserts the phone right away and loads it, so its item can point to its id.
        /// </summary>
        Phone CreateAndLoad(PhoneModel model);
    }
}
