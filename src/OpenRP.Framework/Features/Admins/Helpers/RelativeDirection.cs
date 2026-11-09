namespace OpenRP.Framework.Features.Admins.Helpers
{
    /// <summary>
    /// A direction relative to where someone faces, for teleporting them a distance that way.
    /// </summary>
    public enum RelativeDirection
    {
        Forward,
        Back,
        Left,
        Right,
        Up,
        Down
    }

    public static class RelativeDirections
    {
        public const string Names = "forward (f), back (b), left (l), right (r), up (u) or down (d)";

        private static readonly Dictionary<string, RelativeDirection> DirectionsByName = new Dictionary<string, RelativeDirection>(StringComparer.OrdinalIgnoreCase)
        {
            ["forward"] = RelativeDirection.Forward, ["f"] = RelativeDirection.Forward,
            ["back"] = RelativeDirection.Back, ["backward"] = RelativeDirection.Back, ["b"] = RelativeDirection.Back,
            ["left"] = RelativeDirection.Left, ["l"] = RelativeDirection.Left,
            ["right"] = RelativeDirection.Right, ["r"] = RelativeDirection.Right,
            ["up"] = RelativeDirection.Up, ["u"] = RelativeDirection.Up,
            ["down"] = RelativeDirection.Down, ["d"] = RelativeDirection.Down,
        };

        public static bool TryParse(string text, out RelativeDirection direction)
        {
            return DirectionsByName.TryGetValue(text.Trim(), out direction);
        }

        /// <summary>
        /// How far to move for <paramref name="distance"/> units in the direction, for someone facing
        /// <paramref name="facingAngle"/> degrees. Up and down ignore the facing.
        /// </summary>
        public static Vector3 GetOffset(RelativeDirection direction, float facingAngle, float distance)
        {
            return direction switch
            {
                RelativeDirection.Up => new Vector3(0, 0, distance),
                RelativeDirection.Down => new Vector3(0, 0, -distance),
                RelativeDirection.Left => GetHorizontalOffset(facingAngle + 90.0f, distance),
                RelativeDirection.Right => GetHorizontalOffset(facingAngle - 90.0f, distance),
                RelativeDirection.Back => GetHorizontalOffset(facingAngle + 180.0f, distance),
                _ => GetHorizontalOffset(facingAngle, distance)
            };
        }

        /// <summary>
        /// The same angle math as GetPointInFront: GTA angles turn the other way than the math ones.
        /// </summary>
        private static Vector3 GetHorizontalOffset(float angle, float distance)
        {
            float angleInRadians = -angle * MathF.PI / 180.0f;
            return new Vector3(distance * MathF.Sin(angleInRadians), distance * MathF.Cos(angleInRadians), 0);
        }
    }
}
