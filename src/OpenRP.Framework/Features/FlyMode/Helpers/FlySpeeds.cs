using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.FlyMode.Helpers
{
    /// <summary>
    /// How fast a flying player's camera object moves. Speed builds up while they keep flying.
    /// </summary>
    public static class FlySpeeds
    {
        private const float AccelerationStep = 0.03f;
        // Acceleration stops growing once it passes this value, so top speed is reached after about 3 seconds.
        private const float AccelerationLimit = 1f;
        private const float BaseSpeed = 30f;
        private const float SprintMultiplier = 2f;
        private const float WalkDivisor = 5f;

        public static float GetNextAcceleration(float acceleration)
        {
            return acceleration <= AccelerationLimit ? acceleration + AccelerationStep : acceleration;
        }

        /// <summary>
        /// The speed for the current acceleration: Sprint doubles it, Walk slows it down to a fifth.
        /// </summary>
        public static float GetSpeed(float acceleration, Keys keys)
        {
            float speed = BaseSpeed * acceleration;
            if (IsHeld(keys, Keys.Sprint))
            {
                speed *= SprintMultiplier;
            }
            if (IsHeld(keys, Keys.Walk))
            {
                speed /= WalkDivisor;
            }
            return speed;
        }

        private static bool IsHeld(Keys keys, Keys key)
        {
            return (keys & key) != 0;
        }
    }
}
