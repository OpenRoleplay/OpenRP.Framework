using OpenRP.Framework.Features.FlyMode.Enums;
using OpenRP.Framework.Features.FlyMode.Helpers;
using SampSharp.Entities.SAMP;
using System;

namespace OpenRP.Framework.Features.FlyMode.Services
{
    public class FlyModeMovementService : IFlyModeMovementService
    {
        // Keys are read on every player update, but the object is only sent a new target this often while moving.
        private static readonly TimeSpan MoveInterval = TimeSpan.FromMilliseconds(100);

        public void UpdateFlight(Player player)
        {
            Components.FlyMode? flyMode = player.GetComponent<Components.FlyMode>();
            if (flyMode == null)
            {
                return;
            }

            player.GetKeys(out Keys keys, out int rawUpDown, out int rawLeftRight);
            int upDown = GetSignedAxis(rawUpDown);
            int leftRight = GetSignedAxis(rawLeftRight);
            if (IsDueForMove(flyMode))
            {
                MoveCamera(player, flyMode, keys);
            }
            if (upDown != flyMode.LastUpDown || leftRight != flyMode.LastLeftRight)
            {
                HandleKeysChanged(player, flyMode, keys, upDown, leftRight);
            }
            flyMode.LastUpDown = upDown;
            flyMode.LastLeftRight = leftRight;
        }

        // SampSharp reads open.mp's int16 axes as UInt16, so forward (-128) arrives as 65408.
        private static int GetSignedAxis(int rawAxis)
        {
            return unchecked((short)rawAxis);
        }

        private static bool IsDueForMove(Components.FlyMode flyMode)
        {
            return flyMode.Direction != FlyDirection.None && DateTime.UtcNow - flyMode.LastMove >= MoveInterval;
        }

        private static void HandleKeysChanged(Player player, Components.FlyMode flyMode, Keys keys, int upDown, int leftRight)
        {
            if (WasMoving(flyMode) && upDown == 0 && leftRight == 0)
            {
                StopFlight(flyMode);
                return;
            }

            flyMode.Direction = FlyDirections.GetDirection(upDown, leftRight);
            MoveCamera(player, flyMode, keys);
        }

        private static bool WasMoving(Components.FlyMode flyMode)
        {
            return flyMode.LastUpDown != 0 || flyMode.LastLeftRight != 0;
        }

        private static void StopFlight(Components.FlyMode flyMode)
        {
            flyMode.GetFlyObject()?.Stop();
            flyMode.Direction = FlyDirection.None;
            flyMode.Acceleration = 0f;
        }

        private static void MoveCamera(Player player, Components.FlyMode flyMode, Keys keys)
        {
            PlayerObject? flyObject = flyMode.GetFlyObject();
            if (flyObject == null)
            {
                return;
            }

            flyMode.Acceleration = FlySpeeds.GetNextAcceleration(flyMode.Acceleration);
            float speed = FlySpeeds.GetSpeed(flyMode.Acceleration, keys);
            Vector3 target = FlyDirections.GetTarget(player.CameraPosition, player.CameraFrontVector, flyMode.Direction);
            flyObject.Move(target, speed);
            flyMode.LastMove = DateTime.UtcNow;
        }
    }
}
