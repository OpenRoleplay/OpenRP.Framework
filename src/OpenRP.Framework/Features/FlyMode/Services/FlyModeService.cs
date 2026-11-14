using OpenRP.Framework.Features.FlyMode.Components;
using OpenRP.Framework.Features.FlyMode.Entities;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.FlyMode.Services
{
    public class FlyModeService : IFlyModeService
    {
        // An invisible model: the player only sees the camera moving, not an object.
        private const int FlyObjectModelId = 19300;
        private const float FlyObjectDrawDistance = 0f;
        private const int NoTeam = 0;
        private const int WeaponSlotCount = 13;
        private const string NotFlyingMessage = "You are not flying.";
        private const string FlyOnMessage = "Fly mode on. Use W/A/S/D and look where you want to go; hold Sprint to go faster, Walk to go slower.";
        private const string FlyOffMessage = "Fly mode off.";

        private readonly IWorldService _worldService;
        private readonly IEntityManager _entityManager;

        public FlyModeService(IWorldService worldService, IEntityManager entityManager)
        {
            _worldService = worldService;
            _entityManager = entityManager;
        }

        public bool IsFlying(Player player)
        {
            return player.GetComponent<Components.FlyMode>() != null;
        }

        public string? StartFlying(Player player)
        {
            string? error = GetStartFlyingError(player);
            if (error != null)
            {
                return error;
            }

            BeginFlying(player);
            return null;
        }

        public string? StopFlying(Player player)
        {
            Components.FlyMode? flyMode = player.GetComponent<Components.FlyMode>();
            if (flyMode == null)
            {
                return NotFlyingMessage;
            }

            EndFlying(player, flyMode, flyMode.GetSavedState());
            return null;
        }

        public string? StopFlyingAt(Player player, Vector3 position, int interior, int virtualWorld)
        {
            Components.FlyMode? flyMode = player.GetComponent<Components.FlyMode>();
            if (flyMode == null)
            {
                return NotFlyingMessage;
            }

            FlyModeSavedState landingState = flyMode.GetSavedState() with { Position = position, Interior = interior, VirtualWorld = virtualWorld };
            EndFlying(player, flyMode, landingState);
            return null;
        }

        public string? ToggleFlying(Player player)
        {
            return IsFlying(player) ? StopFlying(player) : StartFlying(player);
        }

        public Vector3? GetCameraPosition(Player player)
        {
            Components.FlyMode? flyMode = player.GetComponent<Components.FlyMode>();
            if (flyMode == null)
            {
                return null;
            }
            return flyMode.GetFlyObject()?.Position;
        }

        private static string? GetStartFlyingError(Player player)
        {
            if (player.GetComponent<Components.FlyMode>() != null)
            {
                return "You are already flying.";
            }
            if (!player.IsPlayerPlayingAsCharacter())
            {
                return "You need to be playing a character to fly.";
            }
            if (player.InAnyVehicle)
            {
                return "Get out of your vehicle before you fly.";
            }
            return null;
        }

        private void BeginFlying(Player player)
        {
            FlyModeSavedState savedState = CaptureSavedState(player);
            PlayerObject flyObject = CreateFlyObject(player);
            player.AddComponent<Components.FlyMode>(flyObject.Entity, savedState);
            player.ToggleSpectating(true);
            player.AttachCameraToObject(flyObject);
            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, FlyOnMessage);
        }

        /// <summary>
        /// Puts the player back into the game. The landing state is applied again after the spawn, which is what places them.
        /// </summary>
        private static void EndFlying(Player player, Components.FlyMode flyMode, FlyModeSavedState landingState)
        {
            player.CancelEdit();
            player.AddComponent<FlyModeRestore>(landingState);
            flyMode.Destroy();
            player.SetSpawnInfo(NoTeam, landingState.Skin, landingState.Position, landingState.Angle);
            player.ToggleSpectating(false);
            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, FlyOffMessage);
        }

        private PlayerObject CreateFlyObject(Player player)
        {
            return _worldService.CreatePlayerObject(player, FlyObjectModelId, player.Position, Vector3.Zero, FlyObjectDrawDistance, player.Entity);
        }

        private static FlyModeSavedState CaptureSavedState(Player player)
        {
            return new FlyModeSavedState(player.Position, player.Angle, player.Interior, player.VirtualWorld, player.Health, player.Armour, player.Skin, CaptureWeapons(player));
        }

        private static List<FlyModeWeapon> CaptureWeapons(Player player)
        {
            List<FlyModeWeapon> weapons = new List<FlyModeWeapon>();
            for (int slot = 0; slot < WeaponSlotCount; slot++)
            {
                player.GetWeaponData(slot, out Weapon weapon, out int ammo);
                if (weapon != Weapon.None)
                {
                    weapons.Add(new FlyModeWeapon(weapon, ammo));
                }
            }
            return weapons;
        }
    }
}
