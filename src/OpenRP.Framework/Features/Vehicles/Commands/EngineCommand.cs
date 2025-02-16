using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Services;
using SampSharp.Entities.SAMP;
using SampSharp.Entities;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Database;
using OpenRP.Framework.Features.Vehicles.Components;
using System.Timers;
using Timer = System.Timers.Timer;
using OpenRP.Framework.Features.Vehicles.Services;

namespace OpenRP.Framework.Features.Vehicles.Commands
{
    public class EngineCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Toggle your vehicle's engine status. Use /engine to toggle the engine on or off.",
            CommandGroups = new string[] { "Vehicles", "General" })]
        public void Engine(Player player, IEntityManager entityManager, ICharacterVehicleManager characterVehicleManager, IChatService chatService, BaseDataContext baseDataContext)
        {
            HandleEngineCommand(player, entityManager, chatService, characterVehicleManager, baseDataContext, null);
        }

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Toggle your vehicle's engine status. Use /e to toggle the engine on or off.",
            CommandGroups = new string[] { "Vehicles", "General" })]
        public void E(Player player, IEntityManager entityManager, ICharacterVehicleManager characterVehicleManager, IChatService chatService, BaseDataContext baseDataContext)
        {
            Engine(player, entityManager, characterVehicleManager, chatService, baseDataContext);
        }

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Toggle your vehicle's engine status. Use /eon to turn the engine on.",
            CommandGroups = new string[] { "Vehicles", "General" })]
        public void EOn(Player player, IEntityManager entityManager, ICharacterVehicleManager characterVehicleManager, IChatService chatService, BaseDataContext baseDataContext)
        {
            HandleEngineCommand(player, entityManager, chatService, characterVehicleManager, baseDataContext, true);
        }

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Toggle your vehicle's engine status. Use /eoff to turn the engine off.",
            CommandGroups = new string[] { "Vehicles", "General" })]
        public void EOff(Player player, IEntityManager entityManager, ICharacterVehicleManager characterVehicleManager, IChatService chatService, BaseDataContext baseDataContext)
        {
            HandleEngineCommand(player, entityManager, chatService, characterVehicleManager, baseDataContext, false);
        }

        /// <summary>
        /// Handles engine-related commands.
        /// </summary>
        /// <param name="player">The player executing the command.</param>
        /// <param name="entityManager">The entity manager.</param>
        /// <param name="forceEngineState">Set to true to force engine on, false to force engine off, or null to toggle.</param>
        private void HandleEngineCommand(Player player, IEntityManager entityManager, IChatService chatService, ICharacterVehicleManager characterVehicleManager, BaseDataContext baseDataContext, bool? forceEngineState)
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
            bool currentEngineState = vehiclePlayerIsIn.Engine;

            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();
            var vehicleDataComponent = vehiclePlayerIsIn.GetComponent<VehicleDataComponent>();

            if (character == null)
            {
                return;
            }

            if (vehicleDataComponent == null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "This vehicle is a spawned-in vehicle and can not be parked!");
                return;
            }

            bool hasKey = characterVehicleManager.HasVehicleKey(character, vehicleDataComponent.VehicleData.Id);

            if (!hasKey)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You do not have a key for this vehicle!");
                return;
            }

            // Handle forced state commands
            if (forceEngineState.HasValue)
            {
                // Check if the vehicle is already in the requested state
                if (currentEngineState == forceEngineState.Value)
                {
                    player.SendPlayerInfoMessage(
                        PlayerInfoMessageType.ERROR,
                        $"The engine is already {(currentEngineState ? "on" : "off")}!"
                    );
                    return;
                }
            }

            bool targetEngineState = forceEngineState ?? !currentEngineState;

            string action = targetEngineState ? "start" : "stop";
            string vehicleModel = vehiclePlayerIsIn.Model.ToString();
            string message;

            // Chat message for the player
            message = $"twists the key of the {vehicleModel}...";
            chatService.SendPlayerChatMessage(player, PlayerChatMessageType.ME, message);

            if (targetEngineState)
            {
                // Starting the engine
                Timer engineStartTimer = new Timer(1000);
                engineStartTimer.Enabled = true;
                engineStartTimer.Elapsed += (entity, e) =>
                {
                    if (player.IsInVehicle(vehiclePlayerIsIn))
                    {
                        message = $"The engine of the {vehicleModel} comes to a start.";
                        chatService.SendPlayerChatMessage(player, PlayerChatMessageType.DO, message);

                        vehiclePlayerIsIn.Engine = true;
                        UpdateVehicleData(vehiclePlayerIsIn, baseDataContext, true);
                    }

                    engineStartTimer.Enabled = false;
                    engineStartTimer.Dispose();
                };
                engineStartTimer.Start();
            }
            else
            {
                // Stopping the engine
                message = $"The engine of the {vehicleModel} comes to a stop.";
                chatService.SendPlayerChatMessage(player, PlayerChatMessageType.DO, message);

                vehiclePlayerIsIn.Engine = false;
                UpdateVehicleData(vehiclePlayerIsIn, baseDataContext, false);
            }
        }


        /// <summary>
        /// Updates the database and component data for a vehicle's engine state.
        /// </summary>
        /// <param name="vehicle">The vehicle whose data to update.</param>
        /// <param name="isEngineOn">The new engine state.</param>
        private void UpdateVehicleData(Vehicle vehicle, BaseDataContext baseDataContext, bool isEngineOn)
        {
            VehicleDataComponent vehicleDataComponent = vehicle.GetComponent<VehicleDataComponent>();
            if (vehicleDataComponent == null) return;

            ulong id = vehicleDataComponent.VehicleData.Id;

            VehicleModel vehicleData = baseDataContext.Vehicles.Find(id);
            if (vehicleData == null) return;

            vehicleData.IsEngineOn = isEngineOn;
            vehicleDataComponent.VehicleData = vehicleData;
            baseDataContext.SaveChanges();
        }
    }
}
