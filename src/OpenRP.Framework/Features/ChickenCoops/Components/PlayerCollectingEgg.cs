using SampSharp.Entities;
using System;

namespace OpenRP.Framework.Features.ChickenCoops.Components
{
    /// <summary>
    /// Marks a player who is collecting an egg from a coop, until the collection ends.
    /// </summary>
    public class PlayerCollectingEgg : Component
    {
        private static readonly TimeSpan StepDuration = TimeSpan.FromSeconds(2.5);

        private readonly EntityId _coopEntityId;
        private readonly bool _isCareful;
        private int _step;
        private DateTime _nextStepAt = DateTime.UtcNow.Add(StepDuration);

        public PlayerCollectingEgg(EntityId coopEntityId, bool isCareful)
        {
            _coopEntityId = coopEntityId;
            _isCareful = isCareful;
        }

        public EntityId GetCoopEntityId()
        {
            return _coopEntityId;
        }

        /// <summary>
        /// True when the player started crouched, so they can see what they're doing and may get an egg.
        /// </summary>
        public bool IsCareful()
        {
            return _isCareful;
        }

        public int GetStep()
        {
            return _step;
        }

        public bool IsNextStepDue()
        {
            return DateTime.UtcNow >= _nextStepAt;
        }

        public void AdvanceStep()
        {
            _step++;
            _nextStepAt = DateTime.UtcNow.Add(StepDuration);
        }
    }
}
