using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Characters.Components;
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
using OpenRP.Framework.Features.Vehicles.Services;

namespace OpenRP.Framework.Features.Vehicles.Commands
{
    public class ParkCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Make your vehicle spawn here from now on; only the owner or a key holder can.",
            CommandGroups = new string[] { "Vehicles", "Ownership" })]
        public void Park(Player player, IEntityManager entityManager, BaseDataContext dataContext, IVehicleAccessService vehicleAccessService)
        {
            // Check if the player is in a vehicle and is the driver
            if (player.IsPlayerPlayingAsCharacter() && player.InAnyVehicle)
            {
                if (player.VehicleSeat == 0) // Player must be the driver
                {
                    Vehicle vehiclePlayerIsIn = entityManager.GetComponent<Vehicle>(player.Vehicle);

                    // Get the current position and rotation of the vehicle
                    Vector3 vehiclePosition = vehiclePlayerIsIn.Position;
                    float vehicleAngle = vehiclePlayerIsIn.Angle;

                    // Retrieve the VehicleDataComponent to get vehicle's ID
                    VehicleDataComponent vehicleDataComponent = vehiclePlayerIsIn.GetComponent<VehicleDataComponent>();
                    if (vehicleDataComponent == null)
                    {
                        player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "This vehicle is a temporary vehicle (it came from /spawnveh), so it can't be parked.");
                    }
                    else if (!CanPlayerPark(player, vehicleAccessService, vehicleDataComponent))
                    {
                        player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You're not allowed to park this vehicle.");
                    }
                    else
                    {
                        // The live data is the one source of truth; only the placement columns are written.
                        VehicleModel vehicleData = vehicleDataComponent.VehicleData;
                        vehicleData.X = vehiclePosition.X;
                        vehicleData.Y = vehiclePosition.Y;
                        vehicleData.Z = vehiclePosition.Z;
                        vehicleData.Rotation = vehicleAngle;

                        ulong id = vehicleData.Id;
                        dataContext.Vehicles
                            .Where(row => row.Id == id)
                            .ExecuteUpdate(setters => setters
                                .SetProperty(row => row.X, vehiclePosition.X)
                                .SetProperty(row => row.Y, vehiclePosition.Y)
                                .SetProperty(row => row.Z, vehiclePosition.Z)
                                .SetProperty(row => row.Rotation, vehicleAngle)
                                .SetProperty(row => row.UpdatedOn, DateTime.UtcNow));

                        // Notify the player that the vehicle has been parked and saved
                        player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, "This vehicle will now spawn here in the future.");
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
        public void Savecar(Player player, IEntityManager entityManager, BaseDataContext context, IVehicleAccessService vehicleAccessService)
        {
            Park(player, entityManager, context, vehicleAccessService);
        }

        /// <summary>
        /// The ownership rule for parking, for the character playing this player.
        /// </summary>
        private static bool CanPlayerPark(Player player, IVehicleAccessService vehicleAccessService, VehicleDataComponent vehicleDataComponent)
        {
            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();
            return vehicleAccessService.CanPark(character, vehicleDataComponent, vehicleAccessService.IsAdmin(character));
        }
    }
}
