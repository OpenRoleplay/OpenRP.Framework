using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Features.Vehicles.Components;
using OpenRP.Framework.Features.Vehicles.Helpers;
using OpenRP.Framework.Features.Vehicles.Services;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Framework.Shared.Chat.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Linq;

namespace OpenRP.Framework.Features.Vehicles.Commands
{
    public class RepairVehicleCommand : ISystem
    {
        // The health of a spawned vehicle, which has no data of its own. Matches the VehicleModel default.
        private const float DefaultMaxHealth = 1000.0f;

        private readonly IEntityManager _entityManager;
        private readonly IChatService _chatService;
        private readonly IVehicleManager _vehicleManager;
        private readonly IVehicleInfoService _vehicleInfoService;
        private readonly ILogger<RepairVehicleCommand> _logger;

        public RepairVehicleCommand(IEntityManager entityManager, IChatService chatService, IVehicleManager vehicleManager, IVehicleInfoService vehicleInfoService, ILogger<RepairVehicleCommand> logger)
        {
            _entityManager = entityManager;
            _chatService = chatService;
            _vehicleManager = vehicleManager;
            _vehicleInfoService = vehicleInfoService;
            _logger = logger;
        }

        // The database context is a method parameter, not a constructor one: systems are singletons, and a context
        // kept for the whole server run grows and goes stale.
        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Repair the vehicle you sit in, or the nearest one, to full health.",
            CommandGroups = new string[] { "Admin" })]
        public void RepairVehicle(Player player, BaseDataContext dataContext)
        {
            Vehicle? vehicle = player.InAnyVehicle ? player.Vehicle : NearbyVehicleFinder.FindNearestVehicle(_entityManager, _vehicleInfoService, player);
            if (vehicle == null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, $"Sit in the vehicle, or stand within {NearbyVehicleFinder.NearbyDistance} metres of it, or use /repairvehicle [vehicle id].");
                return;
            }

            RepairLoadedVehicle(player, dataContext, vehicle);
            _chatService.SendPlayerChatMessage(player, PlayerChatMessageType.DO, GetLooksAsGoodAsNewText(vehicle));
        }

        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Repair a saved vehicle to full health, by its id.",
            CommandGroups = new string[] { "Admin" })]
        public void RepairVehicle(Player player, BaseDataContext dataContext, int vehicleId)
        {
            if (vehicleId <= 0)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "Enter a vehicle id from 1 up.");
                return;
            }

            RepairVehicleById(player, dataContext, (ulong)vehicleId);
        }

        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Repair the vehicle you sit in, or the nearest one, to full health. Same as /repairvehicle.",
            CommandGroups = new string[] { "Admin" })]
        public void FixVeh(Player player, BaseDataContext dataContext)
        {
            RepairVehicle(player, dataContext);
        }

        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Repair a saved vehicle to full health, by its id. Same as /repairvehicle [vehicle id].",
            CommandGroups = new string[] { "Admin" })]
        public void FixVeh(Player player, BaseDataContext dataContext, int vehicleId)
        {
            RepairVehicle(player, dataContext, vehicleId);
        }

        /// <summary>
        /// A loaded vehicle is repaired in game too, and a /do line is sent at it. An unloaded one only gets its row updated, and it spawns repaired.
        /// </summary>
        private void RepairVehicleById(Player player, BaseDataContext dataContext, ulong vehicleId)
        {
            Vehicle? loadedVehicle = _vehicleManager.FindLoadedVehicle(vehicleId);
            if (loadedVehicle != null)
            {
                RepairLoadedVehicle(player, dataContext, loadedVehicle);
                SendLooksAsGoodAsNewAtVehicle(player, loadedVehicle);
                return;
            }
            RepairUnloadedVehicle(player, dataContext, vehicleId);
        }

        /// <summary>
        /// The /do line is named after the admin who repaired the vehicle, and it's sent where the vehicle is, so people near it see it.
        /// </summary>
        private void SendLooksAsGoodAsNewAtVehicle(Player player, Vehicle vehicle)
        {
            Character? character = player.GetPlayerCurrentlyPlayingAsCharacter();
            if (character == null)
            {
                return;
            }
            _chatService.SendDoMessage(character.GetName(), vehicle.Position, vehicle.VirtualWorld, GetLooksAsGoodAsNewText(vehicle));
        }

        private static string GetLooksAsGoodAsNewText(Vehicle vehicle)
        {
            return $"The {vehicle.Model} looks as good as new.";
        }

        /// <summary>
        /// Repairs the vehicle in game, and for a saved one also its live data and its row. Spawned vehicles have no row.
        /// </summary>
        private void RepairLoadedVehicle(Player player, BaseDataContext dataContext, Vehicle vehicle)
        {
            VehicleDataComponent? vehicleDataComponent = vehicle.GetComponent<VehicleDataComponent>();
            float maxHealth = vehicleDataComponent?.VehicleData.MaxHealth ?? DefaultMaxHealth;
            ApplyFullRepair(vehicle, maxHealth);

            if (vehicleDataComponent == null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ADMIN, $"Repaired the spawned {vehicle.Model}.");
                return;
            }

            // The live data is the one source of truth, so it's updated as well as the row.
            VehicleModel vehicleData = vehicleDataComponent.VehicleData;
            vehicleData.CurrentHealth = maxHealth;
            SaveCurrentHealth(dataContext, vehicleData.Id, maxHealth);
            player.SendPlayerInfoMessage(PlayerInfoMessageType.ADMIN, $"Repaired vehicle #{vehicleData.Id}, {_vehicleManager.DescribeModel(vehicleData)}.");
        }

        private void RepairUnloadedVehicle(Player player, BaseDataContext dataContext, ulong vehicleId)
        {
            VehicleModel? vehicleData = dataContext.Vehicles.AsNoTracking().FirstOrDefault(row => row.Id == vehicleId);
            if (vehicleData == null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, $"There's no vehicle #{vehicleId}.");
                return;
            }

            SaveCurrentHealth(dataContext, vehicleId, vehicleData.MaxHealth);
            player.SendPlayerInfoMessage(PlayerInfoMessageType.ADMIN, $"Repaired vehicle #{vehicleId}, {_vehicleManager.DescribeModel(vehicleData)}.");
        }

        private static void ApplyFullRepair(Vehicle vehicle, float maxHealth)
        {
            vehicle.Repair();
            vehicle.Health = maxHealth;
        }

        /// <summary>
        /// Writes only the health and the update time of the row, so the rest of the vehicle stays as it is. Logs when no row matched.
        /// </summary>
        private void SaveCurrentHealth(BaseDataContext dataContext, ulong vehicleId, float health)
        {
            int updatedRows = dataContext.Vehicles
                .Where(row => row.Id == vehicleId)
                .ExecuteUpdate(setters => setters
                    .SetProperty(row => row.CurrentHealth, health)
                    .SetProperty(row => row.UpdatedOn, DateTime.UtcNow));

            if (updatedRows == 0)
            {
                _logger.LogWarning("Repairing vehicle {VehicleId} saved no health, because its row wasn't found.", vehicleId);
            }
        }
    }
}
