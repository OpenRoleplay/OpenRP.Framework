using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Database.Services;
using OpenRP.Framework.Features.Vehicles.Components;
using OpenRP.Framework.Features.Vehicles.Helpers;
using SampSharp.Entities.SAMP;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRP.Framework.Database;

namespace OpenRP.Framework.Features.Vehicles.Services
{
    public class VehicleManager : IVehicleManager
    {
        private readonly IWorldService _worldService;
        private readonly IEntityManager _entityManager;
        private readonly VehicleHelper _vehicleHelper;
        private readonly BaseDataContext _DataContext;
        private readonly IDataMemoryService _dataMemoryService;

        public VehicleManager(IWorldService worldService, IEntityManager entityManager, BaseDataContext dataContext, IDataMemoryService dataMemoryService)
        {
            _worldService = worldService;
            _entityManager = entityManager;
            _vehicleHelper = new VehicleHelper();
            _DataContext = dataContext;
            _dataMemoryService = dataMemoryService;
        }

        public void LoadAndUnloadVehicles()
        {
            List<ulong> currentlyLoadedVehicleIds = GetCurrentlyLoadedVehicleDatabaseIds();

            // Fetch all vehicles except those with the specified IDs
            List<VehicleModel> vehicles = _DataContext.Vehicles
                .Where(vehicle => !currentlyLoadedVehicleIds.Contains(vehicle.Id))
                .ToList();

            foreach (VehicleModel vehicle in vehicles)
            {
                Vehicle loadedVehicle = CreateVehicle(vehicle);
            }
        }

        private List<ulong> GetCurrentlyLoadedVehicleDatabaseIds()
        {
            List<ulong> loadedVehicleDatabaseIds = new List<ulong>();
            foreach (VehicleDataComponent loadedVehicle in _entityManager.GetComponents<VehicleDataComponent>())
            {
                loadedVehicleDatabaseIds.Add(loadedVehicle.VehicleData.Id);
            }
            return loadedVehicleDatabaseIds;
        }

        public Vehicle CreateVehicle(VehicleModel vehicle)
        {
            // Store position in Vector3
            Vector3 position = new Vector3(vehicle.X, vehicle.Y, vehicle.Z);

            // Create Vehicle
            var vehicleModels = _dataMemoryService.GetVehicleModels();

            // Use a static or shared Random instance; here we create a local one if needed.
            Random rnd = new Random();

            int modelToSpawn = 400;

            // If the vehicle's Model navigation property is not null, use its ModelId.
            if (vehicle.ModelId != null)
            {
                var foundModel = vehicleModels.FirstOrDefault(i => i.Id == vehicle.ModelId);
                modelToSpawn = foundModel?.ModelId ?? 400;
            }
            else
            {
                // Otherwise, randomly select one spawn type model matching the vehicle's ModelSpawnTypeId.
                var spawnTypeModel = _dataMemoryService.GetSpawnTypeModels()
                                               .Where(i => i.VehicleSpawnTypeId == vehicle.ModelSpawnTypeId)
                                               .OrderBy(x => rnd.Next())
                                               .FirstOrDefault();

                if (spawnTypeModel != null)
                {
                    var foundModel = vehicleModels.FirstOrDefault(i => i.Id == spawnTypeModel.VehicleModelId);
                    modelToSpawn = foundModel?.ModelId ?? 400;
                }
            }

            if (vehicle.ServerVehicle)
            {
                VehicleModelDefaultColorModel vehicleModelDefaultColor = GetRandomVehicleColor((int)modelToSpawn);

                vehicle.Color1 = vehicleModelDefaultColor.Color1;
                vehicle.Color2 = vehicleModelDefaultColor.Color2;
            }

            Vehicle spawnedVehicle = _worldService.CreateVehicle((VehicleModelType)modelToSpawn, position, vehicle.Rotation, vehicle.Color1, vehicle.Color2);
            UpdateVehicle(spawnedVehicle, vehicle);

            // Create Vehicle Data Component
            VehicleDataComponent vehicleDataComponent = spawnedVehicle.AddComponent<VehicleDataComponent>();
            vehicleDataComponent.VehicleData = vehicle;

            return spawnedVehicle;
        }

        public VehicleModelDefaultColorModel GetRandomVehicleColor(int modelId)
        {
            Random rnd = new Random();

            VehicleModelDefaultColorModel vehicleModelDefaultColor = _dataMemoryService.GetVehicleModelDefaultColors()
                             .Where(i => i.VehicleModel.ModelId == modelId)
                             .OrderBy(x => rnd.Next())
                             .FirstOrDefault();

            if (vehicleModelDefaultColor == null)
            {
                vehicleModelDefaultColor = new VehicleModelDefaultColorModel()
                {
                    Color1 = 1,
                    Color2 = 1
                };
            }

            return vehicleModelDefaultColor;
        }

        public void UpdateVehicle(Vehicle vehicle)
        {
            var vehicleDataComponent = vehicle.GetComponent<VehicleDataComponent>();

            if (vehicleDataComponent == null)
                return;

            // Retrieve the vehicle data from the database
            var vehicleData = _DataContext.Vehicles.Find(vehicleDataComponent.VehicleData.Id);

            if (vehicleData == null)
                return;

            // Update the vehicle based on the retrieved data
            UpdateVehicle(vehicle, vehicleData);
        }

        public void UpdateVehicle(Vehicle vehicle, VehicleModel vehicleData)
        {
            if (vehicleData == null)
                return;

            // Update the vehicle based on the provided data
            vehicle.Position = new Vector3(vehicleData.X, vehicleData.Y, vehicleData.Z);
            vehicle.Rotation = new Vector3(0, 0, vehicleData.Rotation);
            vehicle.ChangeColor(vehicleData.Color1, vehicleData.Color2);
            vehicle.SetNumberPlate(_vehicleHelper.GenerateCarPlate(vehicleData.Id, "SA"));
            vehicle.Engine = vehicleData.IsEngineOn;
            vehicle.Lights = vehicleData.AreLightsOn;
            vehicle.Alarm = vehicleData.IsAlarmOn;
            vehicle.Doors = vehicleData.AreDoorsLocked;
            vehicle.Bonnet = vehicleData.IsBonnetOpen;
            vehicle.Boot = vehicleData.IsBootOpen;
            vehicle.IsDriverDoorOpen = vehicleData.IsDriverDoorOpen;
            vehicle.IsPassengerDoorOpen = vehicleData.IsPassengerDoorOpen;
            vehicle.IsBackLeftDoorOpen = vehicleData.IsBackLeftDoorOpen;
            vehicle.IsBackRightDoorOpen = vehicleData.IsBackRightDoorOpen;
            vehicle.Health = vehicleData.CurrentHealth;
        }

        public async Task<bool> SetVehicleLockAsync(Vehicle vehicle, bool isLocked)
        {
            try
            {
                if (vehicle == null)
                {
                    return false;
                }

                VehicleDataComponent vehicleDataComponent = vehicle.GetComponent<VehicleDataComponent>();

                if (vehicleDataComponent == null)
                {
                    return false;
                }

                ulong id = vehicleDataComponent.VehicleData.Id;

                VehicleModel vehicleData = _DataContext.Vehicles.Find(id);

                if (vehicleData == null)
                {
                    return false;
                }

                // Update the lock state
                vehicle.Doors = isLocked;
                vehicleData.AreDoorsLocked = isLocked;
                vehicleDataComponent.VehicleData = vehicleData;

                // Save changes to the database
                await _DataContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[VehicleManager] Error setting lock state for Vehicle {vehicle.Entity.ToString()}: {ex.Message}");
                return false;
            }
        }
    }
}
