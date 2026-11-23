using SampSharp.Entities;

namespace OpenRP.Framework.Features.Vehicles.Components
{
    /// <summary>
    /// Marks a player who just changed their turn signals with the look keys. It sits on the player, so it goes when the
    /// player leaves, and the look keys are ignored until the cooldown is over.
    /// </summary>
    public class IndicatorKeyCooldown : Component
    {
        private readonly long _startedAtTick;

        public IndicatorKeyCooldown(long startedAtTick)
        {
            _startedAtTick = startedAtTick;
        }

        /// <summary>
        /// When the cooldown started, in milliseconds from Environment.TickCount64.
        /// </summary>
        public long GetStartedAtTick()
        {
            return _startedAtTick;
        }
    }
}
