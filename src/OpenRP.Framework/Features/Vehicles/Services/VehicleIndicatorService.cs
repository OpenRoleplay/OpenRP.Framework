using OpenRP.Framework.Features.Vehicles.Components;
using OpenRP.Framework.Features.Vehicles.Enums;
using OpenRP.Framework.Features.Vehicles.Helpers;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Services;
using OpenRP.OpenMp.Streamer;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Linq;
using System.Numerics;

namespace OpenRP.Framework.Features.Vehicles.Services
{
    /// <summary>
    /// Puts the turn signals on vehicles: flashing light objects attached to the body, one front and one rear per lit side.
    /// </summary>
    public class VehicleIndicatorService : IVehicleIndicatorService
    {
        // The self-flashing orange light, the same model X-RP uses, so it needs no timer.
        private const int BlinkerModelId = 19294;
        // The lights sit at the size of the body divided by this, which puts them near the corners.
        private const float SizeDivisor = 2.23f;
        // How far the lights stand above the vehicle's origin.
        private const float BlinkerHeight = 0.1f;

        private readonly IEntityManager _entityManager;
        private readonly IStreamerService _streamerService;
        private readonly IVehicleInfoService _vehicleInfoService;
        private readonly IChatService _chatService;

        public VehicleIndicatorService(IEntityManager entityManager, IStreamerService streamerService, IVehicleInfoService vehicleInfoService, IChatService chatService)
        {
            _entityManager = entityManager;
            _streamerService = streamerService;
            _vehicleInfoService = vehicleInfoService;
            _chatService = chatService;
        }

        public bool HasTurnSignals(Vehicle vehicle)
        {
            return TurnSignalVehicles.CanHaveTurnSignals(vehicle.Model) && GetVehicleSize(vehicle) != Vector3.Zero;
        }

        public IndicatorState GetIndicatorState(Vehicle vehicle)
        {
            VehicleIndicators? indicators = vehicle.GetComponent<VehicleIndicators>();
            return indicators == null ? IndicatorState.Off : indicators.GetState();
        }

        public string? SetIndicator(Player driver, Vehicle vehicle, IndicatorState state)
        {
            if (!HasTurnSignals(vehicle))
            {
                return "This vehicle has no turn signals.";
            }

            if (GetIndicatorState(vehicle) == state)
            {
                return IndicatorRules.GetAlreadySetMessage(state);
            }

            ChangeIndicatorState(driver, vehicle, state);
            return null;
        }

        public void SetIndicatorFromKeyPress(Player driver, Vehicle vehicle, IndicatorState pressedState)
        {
            if (!HasTurnSignals(vehicle) || IsOnKeyCooldown(driver))
            {
                return;
            }

            IndicatorState currentState = GetIndicatorState(vehicle);
            IndicatorState targetState = IndicatorRules.GetStateAfterKeyPress(currentState, pressedState);
            if (targetState == currentState)
            {
                return;
            }

            StartKeyCooldown(driver);
            ChangeIndicatorState(driver, vehicle, targetState);
        }

        public void TurnOffTurnSignalsDrivenBy(EntityId driverEntityId)
        {
            // A copy, because turning off removes the components being listed.
            foreach (VehicleIndicators indicators in _entityManager.GetComponents<VehicleIndicators>()
                .Where(indicators => indicators.IsDrivenBy(driverEntityId) && indicators.GetState() != IndicatorState.Hazards)
                .ToList())
            {
                Vehicle? vehicle = indicators.GetComponent<Vehicle>();
                if (vehicle != null)
                {
                    RemoveIndicators(vehicle);
                }
            }
        }

        public void RemoveKeyCooldown(Player player)
        {
            player.DestroyComponents<IndicatorKeyCooldown>();
        }

        private void ChangeIndicatorState(Player driver, Vehicle vehicle, IndicatorState state)
        {
            ApplyIndicatorState(driver, vehicle, state);
            _chatService.SendPlayerChatMessage(driver, PlayerChatMessageType.ME, IndicatorRules.GetSwitchMessage(state));
        }

        private static bool IsOnKeyCooldown(Player player)
        {
            IndicatorKeyCooldown? cooldown = player.GetComponent<IndicatorKeyCooldown>();
            return cooldown != null && !IndicatorRules.IsCooldownOver(cooldown.GetStartedAtTick(), Environment.TickCount64);
        }

        private static void StartKeyCooldown(Player player)
        {
            player.DestroyComponents<IndicatorKeyCooldown>();
            player.AddComponent<IndicatorKeyCooldown>(Environment.TickCount64);
        }

        private void ApplyIndicatorState(Player driver, Vehicle vehicle, IndicatorState state)
        {
            RemoveIndicators(vehicle);
            if (state == IndicatorState.Off)
            {
                return;
            }

            // An array, so the driver's EntityId isn't taken as the parent entity.
            VehicleIndicators indicators = _entityManager.AddComponent<VehicleIndicators>(vehicle.Entity, new object[] { driver.Entity, state });
            CreateBlinkers(vehicle, indicators, state);
        }

        private static void RemoveIndicators(Vehicle vehicle)
        {
            VehicleIndicators? indicators = vehicle.GetComponent<VehicleIndicators>();
            if (indicators != null)
            {
                foreach (DynamicObject blinker in indicators.GetBlinkers())
                {
                    blinker.Destroy();
                }
            }
            vehicle.DestroyComponents<VehicleIndicators>();
        }

        private void CreateBlinkers(Vehicle vehicle, VehicleIndicators indicators, IndicatorState state)
        {
            Vector3 size = GetVehicleSize(vehicle);
            if (IndicatorRules.IsLeftSideLit(state))
            {
                CreateBlinkersOnSide(vehicle, indicators, size, -1f);
            }
            if (IndicatorRules.IsRightSideLit(state))
            {
                CreateBlinkersOnSide(vehicle, indicators, size, 1f);
            }
        }

        /// <summary>
        /// Puts a front and a rear light on one side. The sign is -1 for the left side and 1 for the right side.
        /// </summary>
        private void CreateBlinkersOnSide(Vehicle vehicle, VehicleIndicators indicators, Vector3 size, float sideSign)
        {
            float sideOffset = sideSign * size.X / SizeDivisor;
            float halfLength = size.Y / SizeDivisor;
            CreateBlinker(vehicle, indicators, new Vector3(sideOffset, halfLength, BlinkerHeight));
            CreateBlinker(vehicle, indicators, new Vector3(sideOffset, -halfLength, BlinkerHeight));
        }

        private void CreateBlinker(Vehicle vehicle, VehicleIndicators indicators, Vector3 offset)
        {
            DynamicObject blinker = _streamerService.CreateDynamicObject(BlinkerModelId, vehicle.Position, Vector3.Zero,
                virtualWorld: vehicle.VirtualWorld, interior: vehicle.Interior, parent: vehicle.Entity);
            blinker.AttachTo(vehicle, offset, Vector3.Zero);
            indicators.AddBlinker(blinker);
        }

        private Vector3 GetVehicleSize(Vehicle vehicle)
        {
            return _vehicleInfoService.GetModelInfo(vehicle.Model, VehicleModelInfoType.Size);
        }
    }
}
