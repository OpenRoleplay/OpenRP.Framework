using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Features.Vehicles.Enums;
using OpenRP.Framework.Features.Vehicles.Helpers;
using OpenRP.Framework.Features.Vehicles.Services;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Vehicles.Commands
{
    public class IndicatorCommand : ISystem
    {
        private const string Syntax = "/blinker [left | right | hazards | off]";

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Switch your vehicle's turn signals. Use /blinker left, right, hazards or off.",
            CommandGroups = new string[] { "Vehicles", "General" })]
        public void Blinker(Player player, string side, IVehicleIndicatorService vehicleIndicatorService)
        {
            HandleIndicatorCommand(player, side, vehicleIndicatorService);
        }

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Switch your vehicle's turn signals. Use /blinker left, right, hazards or off.",
            CommandGroups = new string[] { "Vehicles", "General" })]
        public void Blinker(Player player)
        {
            player.SendPlayerInfoMessage(PlayerInfoMessageType.SYNTAX, Syntax);
        }

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Switch your vehicle's turn signals. Use /indicator left, right, hazards or off.",
            CommandGroups = new string[] { "Vehicles", "General" })]
        public void Indicator(Player player, string side, IVehicleIndicatorService vehicleIndicatorService)
        {
            HandleIndicatorCommand(player, side, vehicleIndicatorService);
        }

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Switch your vehicle's turn signals. Use /indicator left, right, hazards or off.",
            CommandGroups = new string[] { "Vehicles", "General" })]
        public void Indicator(Player player)
        {
            player.SendPlayerInfoMessage(PlayerInfoMessageType.SYNTAX, Syntax);
        }

        private void HandleIndicatorCommand(Player player, string side, IVehicleIndicatorService vehicleIndicatorService)
        {
            IndicatorState? state = IndicatorRules.ParseState(side);
            if (state == null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.SYNTAX, Syntax);
                return;
            }

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

            Vehicle? vehicle = player.Vehicle;
            if (vehicle == null)
            {
                return;
            }

            string? error = vehicleIndicatorService.SetIndicator(player, vehicle, state.Value);
            if (error != null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, error);
            }
        }
    }
}
