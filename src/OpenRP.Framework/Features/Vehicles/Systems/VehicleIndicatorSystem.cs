using OpenRP.Framework.Features.Vehicles.Enums;
using OpenRP.Framework.Features.Vehicles.Helpers;
using OpenRP.Framework.Features.Vehicles.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Vehicles.Systems
{
    public class VehicleIndicatorSystem : ISystem
    {
        [Event]
        public void OnPlayerKeyStateChange(Player player, Keys newKeys, Keys oldKeys, IVehicleIndicatorService vehicleIndicatorService)
        {
            IndicatorState? pressedState = IndicatorRules.GetPressedState(newKeys, oldKeys);
            if (pressedState == null || !IsDrivingVehicle(player))
            {
                return;
            }

            Vehicle? vehicle = player.Vehicle;
            if (vehicle != null)
            {
                vehicleIndicatorService.SetIndicatorFromKeyPress(player, vehicle, pressedState.Value);
            }
        }

        [Event]
        public void OnPlayerStateChange(Player player, PlayerState newState, PlayerState oldState, IVehicleIndicatorService vehicleIndicatorService)
        {
            if (oldState == PlayerState.Driving && newState != PlayerState.Driving)
            {
                vehicleIndicatorService.TurnOffTurnSignalsDrivenBy(player.Entity);
            }
        }

        [Event]
        public void OnPlayerDisconnect(Player player, DisconnectReason disconnectReason, IVehicleIndicatorService vehicleIndicatorService)
        {
            vehicleIndicatorService.TurnOffTurnSignalsDrivenBy(player.Entity);
            vehicleIndicatorService.RemoveKeyCooldown(player);
        }

        private static bool IsDrivingVehicle(Player player)
        {
            return player.InAnyVehicle && player.VehicleSeat == 0;
        }
    }
}
