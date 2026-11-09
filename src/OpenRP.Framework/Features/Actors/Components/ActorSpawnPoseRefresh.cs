using SampSharp.Entities;

namespace OpenRP.Framework.Features.Actors.Components
{
    /// <summary>
    /// Marks an actor that streamed in for a player and gets its position and animation sent again once the mapping
    /// under it has loaded. The refresh timer only looks at these, so idle actors cost nothing.
    /// </summary>
    public class ActorSpawnPoseRefresh : Component
    {
        private DateTime _dueAt;

        public ActorSpawnPoseRefresh(TimeSpan delay)
        {
            Postpone(delay);
        }

        /// <summary>
        /// Moves the refresh to <paramref name="delay"/> from now, so players streaming in shortly after each other share one refresh.
        /// </summary>
        public void Postpone(TimeSpan delay)
        {
            _dueAt = DateTime.UtcNow + delay;
        }

        public bool IsDue()
        {
            return DateTime.UtcNow >= _dueAt;
        }
    }
}
