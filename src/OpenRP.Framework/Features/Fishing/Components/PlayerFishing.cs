using OpenRP.Framework.Features.Fishing.Entities;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;

namespace OpenRP.Framework.Features.Fishing.Components
{
    /// <summary>
    /// Marks a player who has a line in the water, until the catch is landed or the cast is cancelled.
    /// </summary>
    public class PlayerFishing : Component
    {
        private readonly FishingCast _cast;
        private DateTime? _biteEndsAt;

        public PlayerFishing(FishingCast cast)
        {
            _cast = cast;
        }

        public FishingCast GetCast()
        {
            return _cast;
        }

        public bool IsBiting()
        {
            return _biteEndsAt.HasValue;
        }

        public bool IsBiteDue()
        {
            return !IsBiting() && DateTime.UtcNow >= _cast.BiteAt;
        }

        public bool IsBiteOver()
        {
            return _biteEndsAt.HasValue && DateTime.UtcNow >= _biteEndsAt.Value;
        }

        public void BeginBite()
        {
            _biteEndsAt = DateTime.UtcNow.AddSeconds(FishingSettings.BiteWindowInSeconds);
        }

        /// <summary>
        /// The rod and the animation go away however the fishing ends. On a disconnect the player can already be gone.
        /// </summary>
        protected override void OnDestroyComponent()
        {
            Player? player = GetComponent<Player>();
            if (player == null || !player.IsComponentAlive)
            {
                return;
            }

            player.RemoveAttachedObject(FishingSettings.RodAttachedObjectSlot);
            player.ClearAnimations();
        }
    }
}
