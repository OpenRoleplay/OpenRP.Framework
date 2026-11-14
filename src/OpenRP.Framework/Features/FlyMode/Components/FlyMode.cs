using OpenRP.Framework.Features.FlyMode.Entities;
using OpenRP.Framework.Features.FlyMode.Enums;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;

namespace OpenRP.Framework.Features.FlyMode.Components
{
    /// <summary>
    /// Marks a player who is flying. Holds the invisible object their camera is attached to, the movement state,
    /// and what to put back when they land. Destroying it destroys the object.
    /// </summary>
    public class FlyMode : Component
    {
        private readonly EntityId _flyObjectEntityId;
        private readonly FlyModeSavedState _savedState;

        public FlyMode(EntityId flyObjectEntityId, FlyModeSavedState savedState)
        {
            _flyObjectEntityId = flyObjectEntityId;
            _savedState = savedState;
        }

        public FlyDirection Direction { get; set; } = FlyDirection.None;

        public float Acceleration { get; set; }

        public DateTime LastMove { get; set; } = DateTime.UtcNow;

        public int LastUpDown { get; set; }

        public int LastLeftRight { get; set; }

        public EntityId GetFlyObjectEntityId()
        {
            return _flyObjectEntityId;
        }

        public FlyModeSavedState GetSavedState()
        {
            return _savedState;
        }

        /// <summary>
        /// The object the camera is attached to, or null when it no longer exists.
        /// </summary>
        public PlayerObject? GetFlyObject()
        {
            return Manager.Exists(_flyObjectEntityId) ? Manager.GetComponent<PlayerObject>(_flyObjectEntityId) : null;
        }

        protected override void OnDestroyComponent()
        {
            if (Manager.Exists(_flyObjectEntityId))
            {
                Manager.Destroy(_flyObjectEntityId);
            }
        }
    }
}
