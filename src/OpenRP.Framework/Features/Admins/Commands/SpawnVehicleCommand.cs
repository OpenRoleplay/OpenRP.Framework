using OpenRP.Framework.Features.Commands.Attributes;
using SampSharp.Entities.SAMP;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Admins.Commands
{
    public class SpawnVehicleCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Admin", "Tester" },
            Description = "Spawn a vehicle. Use /spawnveh followed by the vehicle name or ID to generate a new vehicle for your use.",
            CommandGroups = new string[] { "Tester" })]
        public void SpawnVeh(Player player, VehicleModelType model, IWorldService worldService, int first_color = 0, int second_color = -1)
        {
            if (second_color == -1)
            {
                second_color = first_color;
            }

            var vehicle = worldService.CreateVehicle(model, player.Position, player.Angle, first_color, second_color);
            vehicle.Engine = true;
            player.PutInVehicle(vehicle);
        }

        [ServerCommand(PermissionGroups = new string[] { "Admin", "Tester" },
            Description = "Spawn a vehicle. Use /sv followed by the vehicle name or ID to generate a new vehicle for your use.",
            CommandGroups = new string[] { "Tester" })]
        public void SV(Player player, VehicleModelType model, IWorldService worldService, int first_color = 0, int second_color = -1)
        {
            SpawnVeh(player, model, worldService, first_color, second_color);
        }

        [ServerCommand(PermissionGroups = new string[] { "Admin", "Tester" },
            Description = "Spawn a vehicle. Use /sv followed by the vehicle name or ID to generate a new vehicle for your use.",
            CommandGroups = new string[] { "Tester" })]
        public void SpawnVehicle(Player player, VehicleModelType model, IWorldService worldService, int first_color = 0, int second_color = -1)
        {
            SpawnVeh(player, model, worldService, first_color, second_color);
        }
    }
}
