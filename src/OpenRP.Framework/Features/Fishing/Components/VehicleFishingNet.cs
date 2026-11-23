using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Shared.BaseManager.Entities;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.Fishing.Components
{
    /// <summary>
    /// A fishing net fitted to a saved Reefer. It's its own entity, so it survives the vehicle being respawned.
    /// </summary>
    public class VehicleFishingNet : Component, IBaseDataComponent, IChangeable, IDeletable
    {
        private readonly VehicleFishingNetModel _netModel;
        private bool _hasChanges;
        private bool _isDeleted;

        public VehicleFishingNet(VehicleFishingNetModel netModel)
        {
            _netModel = netModel;
        }

        public VehicleFishingNetModel GetRawNetModel()
        {
            return _netModel;
        }

        public ulong GetId()
        {
            return _netModel.Id;
        }

        public ulong GetVehicleId()
        {
            return _netModel.VehicleId;
        }

        public ulong GetStorageInventoryId()
        {
            return _netModel.StorageInventoryId;
        }

        public bool HasChanges()
        {
            return _hasChanges;
        }

        public void ProcessChanges(bool processChanges = true)
        {
            _hasChanges = processChanges;
        }

        public bool IsDeleted()
        {
            return _isDeleted;
        }

        public void ProcessDeletion(bool processDeletion = true)
        {
            _isDeleted = processDeletion;
        }
    }
}
