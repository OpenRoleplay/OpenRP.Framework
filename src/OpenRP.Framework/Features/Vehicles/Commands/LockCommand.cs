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
using System.Linq;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Vehicles.Commands
{
    public class LockCommand : ISystem
    {
        // Seat 0 is the driver, seat 1 the front passenger. Only they reach the door lock button.
        private const int FrontPassengerSeat = 1;
        private const string NoKeyMessage = "You don't have the key for this vehicle.";

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Lock the doors of the vehicle you sit in, or the nearest one. Use /lock to lock them.",
            CommandGroups = new string[] { "Vehicles", "General" })]
        public async Task Lock(Player player, IEntityManager entityManager, IChatService chatService, IVehicleManager vehicleManager, IVehicleAccessService vehicleAccessService, IVehicleInfoService vehicleInfoService)
        {
            await HandleLockCommand(player, entityManager, chatService, vehicleManager, vehicleAccessService, vehicleInfoService, true);
        }

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Unlock the doors of the vehicle you sit in, or the nearest one. Use /unlock to unlock them.",
            CommandGroups = new string[] { "Vehicles", "General" })]
        public async Task Unlock(Player player, IEntityManager entityManager, IChatService chatService, IVehicleManager vehicleManager, IVehicleAccessService vehicleAccessService, IVehicleInfoService vehicleInfoService)
        {
            await HandleLockCommand(player, entityManager, chatService, vehicleManager, vehicleAccessService, vehicleInfoService, false);
        }

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Lock the doors of the vehicle, or unlock them when they're locked. Use /locks to toggle the doors.",
            CommandGroups = new string[] { "Vehicles", "General" })]
        public async Task Locks(Player player, IEntityManager entityManager, IChatService chatService, IVehicleManager vehicleManager, IVehicleAccessService vehicleAccessService, IVehicleInfoService vehicleInfoService)
        {
            await HandleLockCommand(player, entityManager, chatService, vehicleManager, vehicleAccessService, vehicleInfoService, null);
        }

        /// <summary>
        /// Handles the lock commands. Null for forceLocked toggles the doors.
        /// </summary>
        private async Task HandleLockCommand(Player player, IEntityManager entityManager, IChatService chatService, IVehicleManager vehicleManager,
            IVehicleAccessService vehicleAccessService, IVehicleInfoService vehicleInfoService, bool? forceLocked)
        {
            if (!player.IsPlayerPlayingAsCharacter())
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You must be playing a character to do this command!");
                return;
            }

            if (IsInRearSeat(player))
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You can't reach the lock button from here.");
                return;
            }

            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();
            bool isFromSeat = IsInFrontSeat(player);
            Vehicle? vehicle = FindVehicleToLock(player, character, entityManager, vehicleInfoService, vehicleAccessService);
            if (vehicle == null)
            {
                SendNoVehicleToLockMessage(player, entityManager, vehicleInfoService);
                return;
            }

            if (!CanPlayerControl(character, vehicle, isFromSeat, vehicleAccessService))
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, NoKeyMessage);
                return;
            }

            // Checked before the doors are compared: while a change is saved, the vehicle still shows the old state.
            if (vehicleManager.IsLockChangeInProgress(vehicle))
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "Wait a moment.");
                return;
            }

            bool isLocked = vehicle.Doors;
            if (forceLocked.HasValue && isLocked == forceLocked.Value)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, $"The doors are already {(isLocked ? "locked" : "unlocked")}.");
                return;
            }

            bool targetLocked = forceLocked ?? !isLocked;
            bool isChanged = await vehicleManager.SetVehicleLockAsync(vehicle, targetLocked);
            if (!player.IsComponentAlive || !vehicle.IsComponentAlive)
            {
                return;
            }
            if (!isChanged)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, $"The doors couldn't be {(targetLocked ? "locked" : "unlocked")}.");
                return;
            }

            SendLockMessages(player, chatService, vehicle, targetLocked, isFromSeat);
        }

        /// <summary>
        /// The vehicle the player sits in as driver or front passenger, otherwise the nearest one in reach that they have the key for.
        /// </summary>
        private static Vehicle? FindVehicleToLock(Player player, Character character, IEntityManager entityManager, IVehicleInfoService vehicleInfoService, IVehicleAccessService vehicleAccessService)
        {
            if (IsInFrontSeat(player))
            {
                return player.Vehicle;
            }
            return NearbyVehicleFinder.FindNearbyVehicles(entityManager, vehicleInfoService, player)
                .FirstOrDefault(vehicle => CanPlayerControl(character, vehicle, false, vehicleAccessService));
        }

        /// <summary>
        /// Says why no vehicle can be locked: none is in reach, or the ones in reach are not the player's.
        /// </summary>
        private static void SendNoVehicleToLockMessage(Player player, IEntityManager entityManager, IVehicleInfoService vehicleInfoService)
        {
            bool isAnyVehicleNear = NearbyVehicleFinder.FindNearbyVehicles(entityManager, vehicleInfoService, player).Count > 0;
            string message = isAnyVehicleNear
                ? NoKeyMessage
                : $"Sit in the vehicle, or stand within {NearbyVehicleFinder.NearbyDistance} metres of it.";
            player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, message);
        }

        private static bool IsInFrontSeat(Player player)
        {
            return player.InAnyVehicle && player.VehicleSeat <= FrontPassengerSeat;
        }

        private static bool IsInRearSeat(Player player)
        {
            return player.InAnyVehicle && player.VehicleSeat > FrontPassengerSeat;
        }

        /// <summary>
        /// A spawned vehicle has no data, so whoever sits in it may lock it. Otherwise the access rules decide, admins included.
        /// </summary>
        private static bool CanPlayerControl(Character character, Vehicle vehicle, bool isFromSeat, IVehicleAccessService vehicleAccessService)
        {
            VehicleDataComponent? vehicleDataComponent = vehicle.GetComponent<VehicleDataComponent>();
            if (vehicleDataComponent == null && isFromSeat)
            {
                return true;
            }
            return vehicleAccessService.CanControl(character, vehicleDataComponent, vehicleAccessService.IsAdmin(character));
        }

        /// <summary>
        /// The key is turned from outside, or the button is pressed from a front seat, then the result is announced.
        /// </summary>
        private static void SendLockMessages(Player player, IChatService chatService, Vehicle vehicle, bool isLocked, bool isFromSeat)
        {
            chatService.SendPlayerChatMessage(player, PlayerChatMessageType.ME, GetLockAction(vehicle, isLocked, isFromSeat));

            string state = isLocked ? "locked" : "unlocked";
            chatService.SendPlayerChatMessage(player, PlayerChatMessageType.DO, $"The doors of the {vehicle.Model} are now {state}.");
        }

        /// <summary>
        /// The /me line: a key turned in the door, or the lock button pushed down to lock and up to unlock.
        /// </summary>
        private static string GetLockAction(Vehicle vehicle, bool isLocked, bool isFromSeat)
        {
            if (!isFromSeat)
            {
                return $"turns the key in the door lock of the {vehicle.Model}.";
            }
            string buttonMovement = isLocked ? "pushes down" : "pushes up";
            return $"{buttonMovement} the door lock button of the {vehicle.Model}.";
        }
    }
}
