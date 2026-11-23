using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Database;
using OpenRP.Framework.Features.Vehicles.Components;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;

namespace OpenRP.Framework.Features.Vehicles.Services
{
    /// <summary>
    /// Transient: it holds a BaseDataContext, so systems take it as a method parameter.
    /// </summary>
    public class VehicleEngineService : IVehicleEngineService
    {
        private static readonly TimeSpan EngineStartDelay = TimeSpan.FromSeconds(1);

        private readonly IEntityManager _entityManager;
        private readonly IChatService _chatService;
        private readonly BaseDataContext _dataContext;

        public VehicleEngineService(IEntityManager entityManager, IChatService chatService, BaseDataContext dataContext)
        {
            _entityManager = entityManager;
            _chatService = chatService;
            _dataContext = dataContext;
        }

        public bool IsEngineStarting(Vehicle vehicle)
        {
            return vehicle.GetComponent<EngineStarting>() != null;
        }

        public void BeginEngineStart(Player player, Vehicle vehicle)
        {
            _chatService.SendPlayerChatMessage(player, PlayerChatMessageType.ME, $"twists the key of the {vehicle.Model}...");
            // An array, so the driver's EntityId isn't taken as the parent entity.
            _entityManager.AddComponent<EngineStarting>(vehicle.Entity, new object[] { player.Entity, DateTime.UtcNow + EngineStartDelay });
        }

        public void FinishEngineStart(EngineStarting engineStarting)
        {
            Vehicle vehicle = engineStarting.GetComponent<Vehicle>();
            Player? driver = GetDriverStillInVehicle(engineStarting, vehicle);
            engineStarting.Destroy();

            if (driver == null)
            {
                return;
            }

            _chatService.SendPlayerChatMessage(driver, PlayerChatMessageType.DO, $"The engine of the {vehicle.Model} comes to a start.");
            vehicle.Engine = true;
            SaveEngineState(vehicle, true);
        }

        public void StopEngine(Player player, Vehicle vehicle)
        {
            _chatService.SendPlayerChatMessage(player, PlayerChatMessageType.ME, $"twists the key of the {vehicle.Model}...");
            _chatService.SendPlayerChatMessage(player, PlayerChatMessageType.DO, $"The engine of the {vehicle.Model} comes to a stop.");
            vehicle.Engine = false;
            SaveEngineState(vehicle, false);
        }

        private Player? GetDriverStillInVehicle(EngineStarting engineStarting, Vehicle vehicle)
        {
            EntityId driverEntityId = engineStarting.GetDriverEntityId();
            if (vehicle == null || !_entityManager.Exists(driverEntityId))
            {
                return null;
            }

            Player driver = _entityManager.GetComponent<Player>(driverEntityId);
            return driver != null && driver.IsInVehicle(vehicle) ? driver : null;
        }

        private void SaveEngineState(Vehicle vehicle, bool isEngineOn)
        {
            VehicleDataComponent vehicleDataComponent = vehicle.GetComponent<VehicleDataComponent>();
            if (vehicleDataComponent == null)
            {
                return;
            }

            // The live data is the one source of truth; only the engine column is written, so the rest of the row stays as it is.
            vehicleDataComponent.VehicleData.IsEngineOn = isEngineOn;
            _dataContext.Vehicles
                .Where(row => row.Id == vehicleDataComponent.VehicleData.Id)
                .ExecuteUpdate(setters => setters
                    .SetProperty(row => row.IsEngineOn, isEngineOn)
                    .SetProperty(row => row.UpdatedOn, DateTime.UtcNow));
        }
    }
}
