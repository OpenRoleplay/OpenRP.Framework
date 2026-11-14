using OpenRP.Framework.Features.FlyMode.Enums;

namespace OpenRP.Framework.Features.FlyMode.Helpers
{
    /// <summary>
    /// Works out which way a flying player is heading, and the point their camera object flies towards.
    /// </summary>
    public static class FlyDirections
    {
        // The target is far enough away that the object keeps moving until it's stopped by a key release.
        private const float TargetDistance = 6000f;

        /// <summary>
        /// Negative up/down is forward and negative left/right is left, as the player keys report them.
        /// </summary>
        public static FlyDirection GetDirection(int upDown, int leftRight)
        {
            FlyDirection direction = FlyDirection.None;
            if (upDown < 0)
            {
                direction |= FlyDirection.Forward;
            }
            if (upDown > 0)
            {
                direction |= FlyDirection.Back;
            }
            if (leftRight < 0)
            {
                direction |= FlyDirection.Left;
            }
            if (leftRight > 0)
            {
                direction |= FlyDirection.Right;
            }
            return direction;
        }

        /// <summary>
        /// The point to fly towards: forward or back along the camera, plus sideways for left or right.
        /// </summary>
        public static Vector3 GetTarget(Vector3 cameraPosition, Vector3 cameraFront, FlyDirection direction)
        {
            Vector3 forward = cameraFront * GetForwardDistance(direction);
            Vector3 sideways = GetLeftVector(cameraFront) * GetSidewaysDistance(direction);
            return cameraPosition + forward + sideways;
        }

        // The camera's left, on the ground plane. Right is the same vector negated.
        private static Vector3 GetLeftVector(Vector3 cameraFront)
        {
            return new Vector3(-cameraFront.Y, cameraFront.X, 0f);
        }

        private static float GetForwardDistance(FlyDirection direction)
        {
            if (direction.HasFlag(FlyDirection.Forward))
            {
                return TargetDistance;
            }
            return direction.HasFlag(FlyDirection.Back) ? -TargetDistance : 0f;
        }

        private static float GetSidewaysDistance(FlyDirection direction)
        {
            if (direction.HasFlag(FlyDirection.Left))
            {
                return TargetDistance;
            }
            return direction.HasFlag(FlyDirection.Right) ? -TargetDistance : 0f;
        }
    }
}
