using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Database.Services;
using OpenRP.Framework.Features.Fishing.Services;
using OpenRP.Framework.Features.Radios.Services;
using OpenRP.Framework.Features.Vehicles.Components;
using OpenRP.Framework.Features.Vehicles.Helpers;
using SampSharp.Entities.SAMP;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using OpenRP.Framework.Database;
using OpenRP.Framework.Features.Jurisdictions.Services;

namespace OpenRP.Framework.Features.Vehicles.Services
{
    public class VehicleManager : IVehicleManager
    {
        // The GTA model a vehicle spawns as when its model can't be found.
        private const int FallbackModelId = 400;
        // The colour a server vehicle gets when its model has no default colours.
        private const int FallbackColor = 1;

        private readonly IWorldService _worldService;
        private readonly IEntityManager _entityManager;
        private readonly VehicleHelper _vehicleHelper;
        // Shared for the whole server run, so it's only used to delete rows and to forget stale copies. Everything
        // else goes through a short-lived context from _scopeFactory.
        private readonly BaseDataContext _DataContext;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IDataMemoryService _dataMemoryService;
        private readonly IJurisdictionService _jurisdictionService;
        private readonly IVehicleKeyService _vehicleKeyService;
        private readonly IRadioEditorService _radioEditorService;
        private readonly IFishingNetService _fishingNetService;
        private readonly ILogger<VehicleManager> _logger;
        // The saved vehicles whose lock is being saved right now, so a second lock or unlock waits for the first.
        private readonly HashSet<EntityId> _vehiclesWithLockChange = new HashSet<EntityId>();

        public VehicleManager(IWorldService worldService, IEntityManager entityManager, BaseDataContext dataContext, IServiceScopeFactory scopeFactory,
            IDataMemoryService dataMemoryService, IJurisdictionService jurisdictionService, IVehicleKeyService vehicleKeyService,
            IRadioEditorService radioEditorService, IFishingNetService fishingNetService, ILogger<VehicleManager> logger)
        {
            _fishingNetService = fishingNetService;
            _jurisdictionService = jurisdictionService;
            _vehicleKeyService = vehicleKeyService;
            _radioEditorService = radioEditorService;
            _worldService = worldService;
            _entityManager = entityManager;
            _vehicleHelper = new VehicleHelper();
            _DataContext = dataContext;
            _scopeFactory = scopeFactory;
            _dataMemoryService = dataMemoryService;
            _logger = logger;
        }

        public void LoadAndUnloadVehicles()
        {
            List<ulong> currentlyLoadedVehicleIds = GetCurrentlyLoadedVehicleDatabaseIds();

            // Fetch all vehicles except those with the specified IDs. Untracked: the loaded vehicles keep these
            // objects as their data, and the shared context must not track them.
            List<VehicleModel> vehicles = _DataContext.Vehicles
                .AsNoTracking()
                .Where(vehicle => !currentlyLoadedVehicleIds.Contains(vehicle.Id))
                .ToList();

            foreach (VehicleModel vehicle in vehicles)
            {
                LoadVehicle(vehicle);
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

        public Vehicle LoadVehicle(VehicleModel vehicle)
        {
            int modelToSpawn = GetModelToSpawn(vehicle);
            (int color1, int color2) = GetSpawnColors(vehicle, modelToSpawn);
            Vehicle spawnedVehicle = _worldService.CreateVehicle((VehicleModelType)modelToSpawn, GetPosition(vehicle), vehicle.Rotation, color1, color2);

            // The spawn colours only go to the vehicle. The data keeps the saved colours, so random ones are never saved.
            VehicleModel spawnedData = vehicle.Clone();
            spawnedData.Color1 = color1;
            spawnedData.Color2 = color2;
            UpdateVehicle(spawnedVehicle, spawnedData);

            VehicleDataComponent vehicleDataComponent = spawnedVehicle.AddComponent<VehicleDataComponent>();
            vehicleDataComponent.VehicleData = vehicle;
            return spawnedVehicle;
        }

        public Vehicle ReplaceVehicle(Vehicle vehicle, VehicleModel vehicleData)
        {
            DestroyVehicle(vehicle);
            return LoadVehicle(vehicleData);
        }

        public void DestroyVehicle(Vehicle vehicle)
        {
            foreach (Player occupant in GetOccupants(vehicle))
            {
                occupant.RemoveFromVehicle(true);
            }
            _entityManager.Destroy(vehicle.Entity);
        }

        private int GetModelToSpawn(VehicleModel vehicle)
        {
            if (vehicle.ModelId != null)
            {
                return GetGtaModelId(vehicle.ModelId.Value);
            }
            return GetRandomSpawnTypeGtaModelId(vehicle.ModelSpawnTypeId);
        }

        private int GetGtaModelId(ulong vehicleModelRowId)
        {
            return _dataMemoryService.GetVehicleModels()
                .FirstOrDefault(i => i.Id == vehicleModelRowId)?.ModelId ?? FallbackModelId;
        }

        private int GetRandomSpawnTypeGtaModelId(ulong? spawnTypeId)
        {
            Random rnd = new Random();
            var spawnTypeModel = _dataMemoryService.GetSpawnTypeModels()
                .Where(i => i.VehicleSpawnTypeId == spawnTypeId)
                .OrderBy(x => rnd.Next())
                .FirstOrDefault();

            return spawnTypeModel == null ? FallbackModelId : GetGtaModelId(spawnTypeModel.VehicleModelId);
        }

        private (int Color1, int Color2) GetSpawnColors(VehicleModel vehicle, int modelToSpawn)
        {
            if (!vehicle.ServerVehicle)
            {
                return (vehicle.Color1, vehicle.Color2);
            }

            VehicleModelDefaultColorModel defaultColors = GetRandomVehicleColor(modelToSpawn);
            return (defaultColors.Color1, defaultColors.Color2);
        }

        public VehicleModelDefaultColorModel GetRandomVehicleColor(int modelId)
        {
            Random rnd = new Random();

            VehicleModelDefaultColorModel? vehicleModelDefaultColor = _dataMemoryService.GetVehicleModelDefaultColors()
                             .Where(i => i.VehicleModel.ModelId == modelId)
                             .OrderBy(x => rnd.Next())
                             .FirstOrDefault();

            if (vehicleModelDefaultColor == null)
            {
                _logger.LogWarning("GTA model {ModelId} has no default colours, so a server vehicle of it gets colours {Color1} and {Color2}.", modelId, FallbackColor, FallbackColor);
                return new VehicleModelDefaultColorModel
                {
                    Color1 = FallbackColor,
                    Color2 = FallbackColor
                };
            }

            return vehicleModelDefaultColor;
        }

        public void UpdateVehicle(Vehicle vehicle)
        {
            VehicleDataComponent? vehicleDataComponent = vehicle.GetComponent<VehicleDataComponent>();

            if (vehicleDataComponent == null)
                return;

            // The in-memory data is the live state, so it's applied as it is rather than re-read from the database.
            UpdateVehicle(vehicle, vehicleDataComponent.VehicleData);
        }

        public void UpdateVehicle(Vehicle vehicle, VehicleModel vehicleData)
        {
            if (vehicleData == null)
                return;

            SetPlacement(vehicle, vehicleData);
            SetColours(vehicle, vehicleData);
            SetNumberPlate(vehicle, vehicleData);
            SetEngineAndLights(vehicle, vehicleData);
            SetDoors(vehicle, vehicleData);
            SetWindows(vehicle, vehicleData);
            SetHealth(vehicle, vehicleData.CurrentHealth);
            SetWorld(vehicle, vehicleData);
        }

        public void UpdateChangedSettings(Vehicle vehicle, VehicleModel before, VehicleModel after)
        {
            if (HasPlacementChanged(before, after))
                SetPlacement(vehicle, after);
            if (before.Color1 != after.Color1 || before.Color2 != after.Color2)
                SetColours(vehicle, after);
            if (before.JurisdictionId != after.JurisdictionId)
                SetNumberPlate(vehicle, after);

            UpdateChangedEngineAndLights(vehicle, before, after);
            UpdateChangedDoors(vehicle, before, after);
            UpdateChangedWindows(vehicle, before, after);

            if (before.CurrentHealth != after.CurrentHealth)
                SetHealth(vehicle, after.CurrentHealth);
            if (before.VirtualWorld != after.VirtualWorld || before.Interior != after.Interior)
                SetWorld(vehicle, after);

            VehicleDataComponent? vehicleDataComponent = vehicle.GetComponent<VehicleDataComponent>();
            if (vehicleDataComponent != null)
                vehicleDataComponent.VehicleData = after;
        }

        private static Vector3 GetPosition(VehicleModel vehicleData)
        {
            return new Vector3(vehicleData.X, vehicleData.Y, vehicleData.Z);
        }

        private static void SetPlacement(Vehicle vehicle, VehicleModel vehicleData)
        {
            vehicle.Position = GetPosition(vehicleData);
            vehicle.Angle = vehicleData.Rotation;
        }

        private static bool HasPlacementChanged(VehicleModel before, VehicleModel after)
        {
            return before.X != after.X || before.Y != after.Y || before.Z != after.Z || before.Rotation != after.Rotation;
        }

        private static void SetColours(Vehicle vehicle, VehicleModel vehicleData)
        {
            vehicle.ChangeColor(vehicleData.Color1, vehicleData.Color2);
        }

        private static void SetEngineAndLights(Vehicle vehicle, VehicleModel vehicleData)
        {
            vehicle.Engine = vehicleData.IsEngineOn;
            vehicle.Lights = vehicleData.AreLightsOn;
            vehicle.Alarm = vehicleData.IsAlarmOn;
        }

        private static void UpdateChangedEngineAndLights(Vehicle vehicle, VehicleModel before, VehicleModel after)
        {
            if (before.IsEngineOn != after.IsEngineOn)
                vehicle.Engine = after.IsEngineOn;
            if (before.AreLightsOn != after.AreLightsOn)
                vehicle.Lights = after.AreLightsOn;
            if (before.IsAlarmOn != after.IsAlarmOn)
                vehicle.Alarm = after.IsAlarmOn;
        }

        private static void SetDoors(Vehicle vehicle, VehicleModel vehicleData)
        {
            vehicle.Doors = vehicleData.AreDoorsLocked;
            vehicle.Bonnet = vehicleData.IsBonnetOpen;
            vehicle.Boot = vehicleData.IsBootOpen;
            vehicle.IsDriverDoorOpen = vehicleData.IsDriverDoorOpen;
            vehicle.IsPassengerDoorOpen = vehicleData.IsPassengerDoorOpen;
            vehicle.IsBackLeftDoorOpen = vehicleData.IsBackLeftDoorOpen;
            vehicle.IsBackRightDoorOpen = vehicleData.IsBackRightDoorOpen;
        }

        private static void UpdateChangedDoors(Vehicle vehicle, VehicleModel before, VehicleModel after)
        {
            if (before.AreDoorsLocked != after.AreDoorsLocked)
                vehicle.Doors = after.AreDoorsLocked;
            if (before.IsBonnetOpen != after.IsBonnetOpen)
                vehicle.Bonnet = after.IsBonnetOpen;
            if (before.IsBootOpen != after.IsBootOpen)
                vehicle.Boot = after.IsBootOpen;
            if (before.IsDriverDoorOpen != after.IsDriverDoorOpen)
                vehicle.IsDriverDoorOpen = after.IsDriverDoorOpen;
            if (before.IsPassengerDoorOpen != after.IsPassengerDoorOpen)
                vehicle.IsPassengerDoorOpen = after.IsPassengerDoorOpen;
            if (before.IsBackLeftDoorOpen != after.IsBackLeftDoorOpen)
                vehicle.IsBackLeftDoorOpen = after.IsBackLeftDoorOpen;
            if (before.IsBackRightDoorOpen != after.IsBackRightDoorOpen)
                vehicle.IsBackRightDoorOpen = after.IsBackRightDoorOpen;
        }

        private static void SetWindows(Vehicle vehicle, VehicleModel vehicleData)
        {
            vehicle.IsDriverWindowClosed = vehicleData.IsDriverWindowClosed;
            vehicle.IsPassengerWindowClosed = vehicleData.IsPassengerWindowClosed;
            vehicle.IsBackLeftWindowClosed = vehicleData.IsBackLeftWindowClosed;
            vehicle.IsBackRightWindowClosed = vehicleData.IsBackRightWindowClosed;
        }

        private static void UpdateChangedWindows(Vehicle vehicle, VehicleModel before, VehicleModel after)
        {
            if (before.IsDriverWindowClosed != after.IsDriverWindowClosed)
                vehicle.IsDriverWindowClosed = after.IsDriverWindowClosed;
            if (before.IsPassengerWindowClosed != after.IsPassengerWindowClosed)
                vehicle.IsPassengerWindowClosed = after.IsPassengerWindowClosed;
            if (before.IsBackLeftWindowClosed != after.IsBackLeftWindowClosed)
                vehicle.IsBackLeftWindowClosed = after.IsBackLeftWindowClosed;
            if (before.IsBackRightWindowClosed != after.IsBackRightWindowClosed)
                vehicle.IsBackRightWindowClosed = after.IsBackRightWindowClosed;
        }

        private static void SetHealth(Vehicle vehicle, float health)
        {
            vehicle.Health = health;
        }

        private static void SetWorld(Vehicle vehicle, VehicleModel vehicleData)
        {
            vehicle.VirtualWorld = vehicleData.VirtualWorld;
            vehicle.Interior = vehicleData.Interior;
        }

        public VehicleModelModel? FindVehicleModelByGtaId(int modelId)
        {
            return _dataMemoryService.GetVehicleModels().FirstOrDefault(i => i.ModelId == modelId);
        }

        public VehicleModelModel? FindVehicleModelById(ulong id)
        {
            return _dataMemoryService.GetVehicleModels().FirstOrDefault(i => i.Id == id);
        }

        public string DescribeModel(VehicleModel vehicleData)
        {
            if (vehicleData.ModelId != null)
            {
                VehicleModelModel? model = FindVehicleModelById(vehicleData.ModelId.Value);
                return model == null ? $"model row #{vehicleData.ModelId.Value}" : $"{model.Name} ({model.ModelId})";
            }
            if (vehicleData.ModelSpawnTypeId != null)
            {
                return $"random, spawn type #{vehicleData.ModelSpawnTypeId.Value}";
            }
            return "None";
        }

        public string GetNumberPlate(VehicleModel vehicleData)
        {
            return _vehicleHelper.GenerateCarPlate(vehicleData.Id, _jurisdictionService.GetPlateTag(vehicleData.JurisdictionId));
        }

        public bool SaveVehicle(VehicleModel vehicle, BaseDataContext context)
        {
            try
            {
                if (vehicle.Id == 0)
                {
                    context.Vehicles.Add(vehicle);
                }
                else
                {
                    context.Vehicles.Update(vehicle);
                }
                context.SaveChanges();
                DetachSharedVehicleRow(vehicle.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Saving vehicle {VehicleId} failed.", vehicle.Id);
                return false;
            }
        }

        /// <summary>
        /// The shared context may still track an older copy of this row. Detaching it means no lookup can return those
        /// old values again.
        /// </summary>
        private void DetachSharedVehicleRow(ulong vehicleId)
        {
            List<EntityEntry<VehicleModel>> trackedEntries = _DataContext.ChangeTracker.Entries<VehicleModel>()
                .Where(entry => entry.Entity.Id == vehicleId)
                .ToList();
            foreach (EntityEntry<VehicleModel> trackedEntry in trackedEntries)
            {
                trackedEntry.State = EntityState.Detached;
            }
        }

        public string? DeleteVehicle(ulong id)
        {
            VehicleModel? vehicle = _DataContext.Vehicles.Find(id);
            if (vehicle == null)
            {
                return $"There's no vehicle #{id}.";
            }

            // The row goes first: if that fails, nothing in game or in the inventories has been touched yet.
            if (!TryRemoveVehicleRow(vehicle))
            {
                return "The vehicle couldn't be deleted.";
            }

            RemoveVehicleLeftovers(id);
            return null;
        }

        /// <summary>
        /// Runs once the row is gone. A failure in the keys or the radio is logged, and the loaded vehicle is still destroyed.
        /// </summary>
        private void RemoveVehicleLeftovers(ulong vehicleId)
        {
            try
            {
                _vehicleKeyService.RemoveVehicleKeys(vehicleId);
                // The radio's row went with the vehicle (cascade). Marking its component deleted keeps the next save from writing to that row.
                _radioEditorService.RemoveVehicleRadio(vehicleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Removing the keys and radio of deleted vehicle {VehicleId} failed.", vehicleId);
            }
            RemoveVehicleFishingNet(vehicleId);
            DestroyLoadedVehicle(vehicleId);
        }

        /// <summary>
        /// The net's row went with the vehicle (cascade), but its storage and the fish in it did not.
        /// </summary>
        private void RemoveVehicleFishingNet(ulong vehicleId)
        {
            try
            {
                _fishingNetService.RemoveNetOfDeletedVehicle(vehicleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Removing the fishing net of deleted vehicle {VehicleId} failed.", vehicleId);
            }
        }

        private bool TryRemoveVehicleRow(VehicleModel vehicle)
        {
            try
            {
                _DataContext.Vehicles.Remove(vehicle);
                _DataContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Deleting vehicle {VehicleId} failed.", vehicle.Id);
                // Forget the failed delete, so later saves on this context don't repeat it.
                _DataContext.ChangeTracker.Clear();
                return false;
            }
        }

        private void DestroyLoadedVehicle(ulong vehicleId)
        {
            Vehicle? vehicle = FindLoadedVehicle(vehicleId);
            if (vehicle != null)
            {
                DestroyVehicle(vehicle);
            }
        }

        public Vehicle? FindLoadedVehicle(ulong vehicleId)
        {
            VehicleDataComponent? vehicleDataComponent = _entityManager.GetComponents<VehicleDataComponent>()
                .FirstOrDefault(component => component.VehicleData.Id == vehicleId);
            return vehicleDataComponent?.GetComponent<Vehicle>();
        }

        private List<Player> GetOccupants(Vehicle vehicle)
        {
            return _entityManager.GetComponents<Player>()
                .Where(player => player.InAnyVehicle && player.Vehicle?.Entity == vehicle.Entity)
                .ToList();
        }

        public void RefreshNumberPlates()
        {
            foreach (VehicleDataComponent vehicleDataComponent in _entityManager.GetComponents<VehicleDataComponent>())
            {
                SetNumberPlate(vehicleDataComponent.GetComponent<Vehicle>(), vehicleDataComponent.VehicleData);
            }
        }

        /// <summary>
        /// The database clears the link too, but loaded vehicles would save the old jurisdiction back when edited.
        /// </summary>
        public void RemoveVehiclesFromJurisdiction(ulong jurisdictionId)
        {
            foreach (VehicleDataComponent vehicleDataComponent in _entityManager.GetComponents<VehicleDataComponent>())
            {
                if (vehicleDataComponent.VehicleData.JurisdictionId != jurisdictionId)
                {
                    continue;
                }
                vehicleDataComponent.VehicleData.JurisdictionId = null;
                SetNumberPlate(vehicleDataComponent.GetComponent<Vehicle>(), vehicleDataComponent.VehicleData);
            }
        }

        /// <summary>
        /// The plate starts with the tag of the country the vehicle is registered in, or SA when it isn't registered.
        /// </summary>
        private void SetNumberPlate(Vehicle? vehicle, VehicleModel vehicleData)
        {
            vehicle?.SetNumberPlate(GetNumberPlate(vehicleData));
        }

        public async Task<bool> SetVehicleLockAsync(Vehicle vehicle, bool isLocked)
        {
            VehicleDataComponent? vehicleDataComponent = vehicle.GetComponent<VehicleDataComponent>();
            if (vehicleDataComponent == null)
            {
                // A spawned vehicle has no row, so its lock is only set in game.
                vehicle.Doors = isLocked;
                return true;
            }

            if (IsLockChangeInProgress(vehicle))
            {
                return false;
            }
            return await SaveVehicleLockWithMarkAsync(vehicle, vehicleDataComponent, isLocked);
        }

        public bool IsLockChangeInProgress(Vehicle vehicle)
        {
            return _vehiclesWithLockChange.Contains(vehicle.Entity);
        }

        /// <summary>
        /// Marks the vehicle while its lock is saved. The entity id is taken before the await, because the vehicle may be destroyed by then.
        /// </summary>
        private async Task<bool> SaveVehicleLockWithMarkAsync(Vehicle vehicle, VehicleDataComponent vehicleDataComponent, bool isLocked)
        {
            EntityId vehicleEntityId = vehicle.Entity;
            _vehiclesWithLockChange.Add(vehicleEntityId);
            try
            {
                return await SaveVehicleLockAsync(vehicle, vehicleDataComponent, isLocked);
            }
            finally
            {
                _vehiclesWithLockChange.Remove(vehicleEntityId);
            }
        }

        /// <summary>
        /// The game only changes once the column is saved, so the game and the database never disagree about the doors.
        /// </summary>
        private async Task<bool> SaveVehicleLockAsync(Vehicle vehicle, VehicleDataComponent vehicleDataComponent, bool isLocked)
        {
            ulong id = vehicleDataComponent.VehicleData.Id;
            try
            {
                bool isSaved = await UpdateVehicleColumnsAsync(id, setters => setters
                    .SetProperty(row => row.AreDoorsLocked, isLocked)
                    .SetProperty(row => row.UpdatedOn, DateTime.UtcNow));

                // The vehicle may have been destroyed while the save was running.
                if (!isSaved || !vehicle.IsComponentAlive || !vehicleDataComponent.IsComponentAlive)
                {
                    return false;
                }

                vehicle.Doors = isLocked;
                vehicleDataComponent.VehicleData.AreDoorsLocked = isLocked;
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Setting the lock of vehicle {VehicleId} to {IsLocked} failed.", id, isLocked);
                return false;
            }
        }

        /// <summary>
        /// Writes only the given columns of a vehicle's row, through a short-lived context. Returns false when no row matched.
        /// </summary>
        private async Task<bool> UpdateVehicleColumnsAsync(ulong vehicleId, Expression<Func<SetPropertyCalls<VehicleModel>, SetPropertyCalls<VehicleModel>>> setColumns)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            BaseDataContext dataContext = scope.ServiceProvider.GetRequiredService<BaseDataContext>();
            int updatedRows = await dataContext.Vehicles
                .Where(vehicle => vehicle.Id == vehicleId)
                .ExecuteUpdateAsync(setColumns);
            return updatedRows == 1;
        }
    }
}
