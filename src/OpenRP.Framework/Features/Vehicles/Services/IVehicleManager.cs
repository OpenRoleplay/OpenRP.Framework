using OpenRP.Framework.Database;
using OpenRP.Framework.Database.Models;
using SampSharp.Entities.SAMP;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Vehicles.Services
{
    public interface IVehicleManager
    {
        /// <summary>
        /// Spawns every saved vehicle that isn't loaded yet. Called once at startup.
        /// </summary>
        void LoadAndUnloadVehicles();

        /// <summary>
        /// Spawns one saved vehicle, with its colours, plate, virtual world and interior, and returns it.
        /// </summary>
        Vehicle LoadVehicle(VehicleModel vehicle);

        /// <summary>
        /// Destroys a loaded vehicle and spawns the given data in its place. Used when the model changes.
        /// </summary>
        Vehicle ReplaceVehicle(Vehicle vehicle, VehicleModel vehicleData);

        /// <summary>
        /// Destroys a loaded vehicle, ejecting its occupants. Its database row is left alone.
        /// </summary>
        void DestroyVehicle(Vehicle vehicle);

        /// <summary>
        /// Inserts or updates the vehicle's row. Returns false, and logs the error, when the save fails. After a save, the
        /// shared context forgets its older copy of the row.
        /// </summary>
        bool SaveVehicle(VehicleModel vehicle, BaseDataContext context);

        /// <summary>
        /// Deletes a saved vehicle for good: its row, then its keys, its radio and its loaded entity. Returns an error message, or null when it worked.
        /// </summary>
        string? DeleteVehicle(ulong id);

        void UpdateVehicle(Vehicle vehicle);

        /// <summary>
        /// Applies every setting of the data to the vehicle, moving and repairing it. Only for spawning and reloading.
        /// </summary>
        void UpdateVehicle(Vehicle vehicle, VehicleModel vehicleData);

        /// <summary>
        /// Applies only the settings that differ between before and after to the live vehicle, and makes after its data.
        /// </summary>
        void UpdateChangedSettings(Vehicle vehicle, VehicleModel before, VehicleModel after);

        /// <summary>
        /// The model with this GTA model id (400 to 611), or null when there's no such row.
        /// </summary>
        VehicleModelModel? FindVehicleModelByGtaId(int modelId);

        /// <summary>
        /// The model row with this id, or null when there's no such row.
        /// </summary>
        VehicleModelModel? FindVehicleModelById(ulong id);

        /// <summary>
        /// The model as text, for example "Infernus (411)", or the spawn type of a random model.
        /// </summary>
        string DescribeModel(VehicleModel vehicleData);

        /// <summary>
        /// The plate the vehicle has, starting with the tag of the country it's registered in.
        /// </summary>
        string GetNumberPlate(VehicleModel vehicleData);

        /// <summary>
        /// Sets the plate of every loaded vehicle again, for when the tags of jurisdictions changed.
        /// </summary>
        void RefreshNumberPlates();

        /// <summary>
        /// Unregisters the loaded vehicles of a jurisdiction that was deleted, so they get an SA plate.
        /// </summary>
        void RemoveVehiclesFromJurisdiction(ulong jurisdictionId);

        /// <summary>
        /// Locks or unlocks the vehicle. A saved vehicle's column is written first, and the game only changes once that
        /// succeeds. A spawned vehicle, which has no row, is only changed in game. Returns false when the save fails.
        /// </summary>
        Task<bool> SetVehicleLockAsync(Vehicle vehicle, bool isLocked);

        /// <summary>
        /// True while a lock change of this saved vehicle is being saved. A second lock or unlock must wait for it.
        /// </summary>
        bool IsLockChangeInProgress(Vehicle vehicle);

        /// <summary>
        /// The loaded vehicle of this saved vehicle, or null when it isn't spawned.
        /// </summary>
        Vehicle? FindLoadedVehicle(ulong vehicleId);
    }
}
