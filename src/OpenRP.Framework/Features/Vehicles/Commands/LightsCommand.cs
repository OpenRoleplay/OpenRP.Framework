using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Services;
using SampSharp.Entities.SAMP;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Framework.Database;
using OpenRP.Framework.Features.Vehicles.Components;

namespace OpenRP.Framework.Features.Vehicles.Commands
{
    public class LightsCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Control your vehicle's lights. Use /lights to toggle the headlights.",
            CommandGroups = new string[] { "Vehicles", "General" })]
        public void Lights(Player player, IEntityManager entityManager, IChatService chatService, BaseDataContext baseDataContext)
        {
            HandleLightsCommand(player, entityManager, chatService, baseDataContext, null); // Toggles the lights
        }

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Control your vehicle's lights. Use /lon to turn them on.",
            CommandGroups = new string[] { "Vehicles", "General" })]
        public void Lon(Player player, IEntityManager entityManager, IChatService chatService, BaseDataContext baseDataContext)
        {
            HandleLightsCommand(player, entityManager, chatService, baseDataContext, true); // Forces lights to turn on
        }

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Control your vehicle's lights. Use /loff to turn them off.",
            CommandGroups = new string[] { "Vehicles", "General" })]
        public void Loff(Player player, IEntityManager entityManager, IChatService chatService, BaseDataContext baseDataContext)
        {
            HandleLightsCommand(player, entityManager, chatService, baseDataContext, false); // Forces lights to turn off
        }

        private void HandleLightsCommand(Player player, IEntityManager entityManager, IChatService chatService, BaseDataContext dataContext, bool? forceLightsState)
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
            bool currentLightsState = vehiclePlayerIsIn.Lights;

            // Handle forced state commands
            if (forceLightsState.HasValue)
            {
                // Check if the vehicle is already in the requested state
                if (currentLightsState == forceLightsState.Value)
                {
                    player.SendPlayerInfoMessage(
                        PlayerInfoMessageType.ERROR,
                        $"The lights are already {(currentLightsState ? "on" : "off")}!"
                    );
                    return;
                }
            }

            bool targetLightsState = forceLightsState ?? !currentLightsState;

            string action = targetLightsState ? "turns on" : "turns off";
            string vehicleModel = vehiclePlayerIsIn.Model.ToString();

            // Chat message for the player
            string message = $"reaches for the light switch of the {vehicleModel} and {action} the lights.";
            chatService.SendPlayerChatMessage(player, PlayerChatMessageType.ME, message);

            // Update lights state
            vehiclePlayerIsIn.Lights = targetLightsState;
            UpdateVehicleLightsData(player, vehiclePlayerIsIn, dataContext, targetLightsState);

            // Action message
            message = targetLightsState
                ? $"The lights of the {vehicleModel} are now shining brightly."
                : $"The lights of the {vehicleModel} fade into darkness.";
            chatService.SendPlayerChatMessage(player, PlayerChatMessageType.DO, message);
        }

        private void UpdateVehicleLightsData(Player player, Vehicle vehicle, BaseDataContext dataContext, bool lightsOn)
        {
            VehicleDataComponent vehicleDataComponent = vehicle.GetComponent<VehicleDataComponent>();
            if (vehicleDataComponent != null && !vehicleDataComponent.VehicleData.ServerVehicle)
            {
                ulong id = vehicleDataComponent.VehicleData.Id;

                VehicleModel vehicleData = dataContext.Vehicles.Find(id);

                vehicleData.AreLightsOn = lightsOn;
                vehicleDataComponent.VehicleData = vehicleData;
                dataContext.SaveChanges();
            }
        }
    }
}
