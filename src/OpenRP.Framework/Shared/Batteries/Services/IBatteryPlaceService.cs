using OpenRP.Framework.Database;
using OpenRP.Framework.Shared.Batteries.Entities;

namespace OpenRP.Framework.Shared.Batteries.Services
{
    public interface IBatteryPlaceService
    {
        /// <summary>
        /// Takes a snapshot of which inventories belong to which characters, online or not. Offline characters have
        /// no components, so their inventory ids come from the database.
        /// </summary>
        BatteryPlaces TakeSnapshot(BaseDataContext dataContext);
    }
}
