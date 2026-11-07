using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Shared.Chat.Enums;
using SampSharp.Entities.SAMP;
using SampSharp.Entities;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Features.Vehicles.Components;
using OpenRP.Framework.Features.Vehicles.Services;

namespace OpenRP.Framework.Features.Vehicles.Commands
{
    public class EngineCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Toggle your vehicle's engine status. Use /engine to toggle the engine on or off.",
            CommandGroups = new string[] { "Vehicles", "General" })]
        public void Engine(Player player, IEntityManager entityManager, IVehicleKeyService vehicleKeyService, IVehicleEngineService vehicleEngineService)
        {
            HandleEngineCommand(player, entityManager, vehicleKeyService, vehicleEngineService, null);
        }

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Toggle your vehicle's engine status. Use /e to toggle the engine on or off.",
            CommandGroups = new string[] { "Vehicles", "General" })]
        public void E(Player player, IEntityManager entityManager, IVehicleKeyService vehicleKeyService, IVehicleEngineService vehicleEngineService)
        {
            HandleEngineCommand(player, entityManager, vehicleKeyService, vehicleEngineService, null);
        }

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Toggle your vehicle's engine status. Use /eon to turn the engine on.",
            CommandGroups = new string[] { "Vehicles", "General" })]
        public void EOn(Player player, IEntityManager entityManager, IVehicleKeyService vehicleKeyService, IVehicleEngineService vehicleEngineService)
        {
            HandleEngineCommand(player, entityManager, vehicleKeyService, vehicleEngineService, true);
        }

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Toggle your vehicle's engine status. Use /eoff to turn the engine off.",
            CommandGroups = new string[] { "Vehicles", "General" })]
        public void EOff(Player player, IEntityManager entityManager, IVehicleKeyService vehicleKeyService, IVehicleEngineService vehicleEngineService)
        {
            HandleEngineCommand(player, entityManager, vehicleKeyService, vehicleEngineService, false);
        }

        /// <summary>
        /// Handles engine-related commands.
        /// </summary>
        /// <param name="forceEngineState">Set to true to force engine on, false to force engine off, or null to toggle.</param>
        private void HandleEngineCommand(Player player, IEntityManager entityManager, IVehicleKeyService vehicleKeyService, IVehicleEngineService vehicleEngineService, bool? forceEngineState)
        {
            if (!player.IsPlayerPlayingAsCharacter() || !player.InAnyVehicle)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You must be in a vehicle to do this command!");
                return;
            }

            if (player.VehicleSeat != 0)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You must be the driver to do this command!");
                return;
            }

            Vehicle vehiclePlayerIsIn = entityManager.GetComponent<Vehicle>(player.Vehicle);
            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();
            VehicleDataComponent vehicleDataComponent = vehiclePlayerIsIn.GetComponent<VehicleDataComponent>();

            if (character == null)
            {
                return;
            }

            if (vehicleDataComponent == null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "This vehicle is a spawned-in vehicle and can not be parked!");
                return;
            }

            if (!vehicleKeyService.HasVehicleKey(character, vehicleDataComponent.VehicleData.Id))
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You do not have a key for this vehicle!");
                return;
            }

            // The key is already turned; the engine is about to start.
            if (vehicleEngineService.IsEngineStarting(vehiclePlayerIsIn))
            {
                return;
            }

            bool currentEngineState = vehiclePlayerIsIn.Engine;
            if (forceEngineState.HasValue && currentEngineState == forceEngineState.Value)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, $"The engine is already {(currentEngineState ? "on" : "off")}!");
                return;
            }

            bool targetEngineState = forceEngineState ?? !currentEngineState;
            if (targetEngineState)
            {
                vehicleEngineService.BeginEngineStart(player, vehiclePlayerIsIn);
            }
            else
            {
                vehicleEngineService.StopEngine(player, vehiclePlayerIsIn);
            }
        }
    }
}
