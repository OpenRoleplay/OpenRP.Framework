using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Fishing.Components;
using OpenRP.Framework.Features.Fishing.Entities;
using OpenRP.Framework.Shared.BaseManager.Helpers;
using SampSharp.Entities;
using System;
using System.Linq;

namespace OpenRP.Framework.Features.Fishing.Services
{
    public class FishingNetManager : BaseManager<VehicleFishingNetModel, VehicleFishingNet>, IFishingNetManager
    {
        public FishingNetManager(BaseDataContext dataContext, IEntityManager entityManager, ILogger<FishingNetManager> logger)
            : base(dataContext, entityManager, logger) { }

        public VehicleFishingNet? FindNetOfVehicle(ulong vehicleId)
        {
            return _entityManager.GetComponents<VehicleFishingNet>().FirstOrDefault(net => !net.IsDeleted() && net.GetVehicleId() == vehicleId);
        }

        public VehicleFishingNet CreateNet(ulong vehicleId, ulong storageInventoryId)
        {
            return CreateAndLoad(new VehicleFishingNetModel { VehicleId = vehicleId, StorageInventoryId = storageInventoryId });
        }

        public void DeleteNet(VehicleFishingNet net)
        {
            net.ProcessDeletion();
            SaveNow();
        }

        protected override DbSet<VehicleFishingNetModel> GetDbSet() => _dataContext.VehicleFishingNets;

        protected override Guid EntityTypeKey => FishingNetEntities.VehicleFishingNetType;

        protected override VehicleFishingNetModel GetModelFromComponent(VehicleFishingNet component)
        {
            return component.GetRawNetModel();
        }
    }
}
