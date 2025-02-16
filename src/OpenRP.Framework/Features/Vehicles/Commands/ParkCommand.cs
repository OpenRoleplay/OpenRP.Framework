using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Shared.Chat.Enums;
using SampSharp.Entities.SAMP;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Database;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Framework.Features.Vehicles.Components;

namespace OpenRP.Framework.Features.Vehicles.Commands
{
    public class ParkCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Park your currently owned vehicle. Use /park to save your vehicle in a new parking spot.",
            CommandGroups = new string[] { "Vehicles", "Ownership" })]
        public void Park(Player player, IEntityManager entityManager, BaseDataContext dataContext)
        {
            // Check if the player is in a vehicle and is the driver
            if (player.IsPlayerPlayingAsCharacter() && player.InAnyVehicle)
            {
                if (player.VehicleSeat == 0) // Player must be the driver
                {
                    Vehicle vehiclePlayerIsIn = entityManager.GetComponent<Vehicle>(player.Vehicle);

                    // Get the current position and rotation of the vehicle
                    Vector3 vehiclePosition = vehiclePlayerIsIn.Position;
                    Vector3 vehicleRotation = vehiclePlayerIsIn.Rotation;

                    // Retrieve the VehicleDataComponent to get vehicle's ID
                    VehicleDataComponent vehicleDataComponent = vehiclePlayerIsIn.GetComponent<VehicleDataComponent>();
                    if (vehicleDataComponent != null)
                    {
                        ulong id = vehicleDataComponent.VehicleData.Id;

                        // Retrieve the VehicleModel from the database
                        VehicleModel vehicleData = dataContext.Vehicles.Find(id);
                        if (vehicleData != null)
                        {
                            // Update position and rotation values
                            vehicleData.X = vehiclePosition.X;
                            vehicleData.Y = vehiclePosition.Y;
                            vehicleData.Z = vehiclePosition.Z;

                            vehicleData.Rotation = vehicleRotation.Z;

                            vehicleDataComponent.VehicleData = vehicleData;

                            // Save the changes back to the database
                            dataContext.SaveChanges();
                        }

                        // Notify the player that the vehicle has been parked and saved
                        player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, "This vehicle will now spawn here in the future.");
                    }
                    else
                    {
                        player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "This vehicle is a spawned-in vehicle and can not be parked!");
                    }
                }
                else
                {
                    player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You must be the driver in order to park the vehicle!");
                }
            }
            else
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You must be in a vehicle in order to park it!");
            }
        }
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Park your currently owned vehicle. Use /savecar to save your vehicle in a new parking spot.",
            CommandGroups = new string[] { "Vehicles", "Ownership" })]
        public void Savecar(Player player, IEntityManager entityManager, BaseDataContext context)
        {
            Park(player, entityManager, context);
        }
    }
}
