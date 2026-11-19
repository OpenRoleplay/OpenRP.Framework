using SampSharp.Entities;
using System.Numerics;

namespace OpenRP.Framework.Features.Fishing.Components
{
    /// <summary>
    /// A spot on the water where fish gather. Its splash is a child object, so it goes away with the hotspot.
    /// </summary>
    public class FishHotspot : Component
    {
        private readonly Vector3 _position;

        public FishHotspot(Vector3 position)
        {
            _position = position;
        }

        public Vector3 GetPosition()
        {
            return _position;
        }

        /// <summary>
        /// True when the position is within range, measured flat: a pier high above the hotspot still counts as close.
        /// </summary>
        public bool IsPositionInRange(Vector3 position, float range)
        {
            return Vector2.Distance(new Vector2(_position.X, _position.Y), new Vector2(position.X, position.Y)) <= range;
        }
    }
}
