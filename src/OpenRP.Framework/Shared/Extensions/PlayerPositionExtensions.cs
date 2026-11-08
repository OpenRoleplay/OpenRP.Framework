using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Shared.Extensions
{
    public static class PlayerPositionExtensions
    {
        /// <summary>
        /// The point <paramref name="distance"/> units in front of the player, in the direction they're facing.
        /// </summary>
        public static Vector2 GetPointInFront(this Player player, float distance)
        {
            float angleInRadians = -player.Angle * MathF.PI / 180.0f;
            return new Vector2(
                player.Position.X + distance * MathF.Sin(angleInRadians),
                player.Position.Y + distance * MathF.Cos(angleInRadians));
        }
    }
}
