using OpenRP.Framework.Features.ActorRoleplay.Entities;
using SampSharp.Entities;
using OpenRP.Framework.Shared.AI.Helpers;

namespace OpenRP.Framework.Features.ActorRoleplay.Components
{
    /// <summary>
    /// What an AI actor recently saw, heard and said. Only added once the actor hears something, and removed again
    /// when it has been quiet for a while, so idle actors carry nothing.
    /// </summary>
    public class ActorRoleplayMemory : Component
    {
        private readonly List<ActorMemoryLine> _lines = new List<ActorMemoryLine>();
        // Who spoke to the actor lately, newest last, so an /action can find the player it's for.
        private readonly List<ActorSpeaker> _speakers = new List<ActorSpeaker>();
        private DateTime _lastHeardAt = DateTime.UtcNow;
        private bool _isReplying;

        /// <summary>
        /// Remembers a line the actor saw or heard, as it appeared in chat.
        /// </summary>
        public void AddHeardLine(string text)
        {
            AddLine(new ActorMemoryLine(false, text));
            _lastHeardAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Remembers who said something, replacing their earlier entry.
        /// </summary>
        public void AddSpeaker(EntityId player, string name)
        {
            _speakers.RemoveAll(speaker => speaker.Player == player);
            _speakers.Add(new ActorSpeaker(player, name, DateTime.UtcNow));
        }

        /// <summary>
        /// The players who spoke to the actor within this time, newest first.
        /// </summary>
        public List<ActorSpeaker> GetRecentSpeakers(TimeSpan within)
        {
            DateTime since = DateTime.UtcNow - within;
            return _speakers.Where(speaker => speaker.HeardAt >= since).Reverse().ToList();
        }

        /// <summary>
        /// Remembers a line the actor said or did itself.
        /// </summary>
        public void AddOwnLine(string text)
        {
            AddLine(new ActorMemoryLine(true, text));
        }

        /// <summary>
        /// A copy of the remembered lines, oldest first.
        /// </summary>
        public List<ActorMemoryLine> GetLines()
        {
            return _lines.ToList();
        }

        public bool IsQuietFor(TimeSpan timeSpan)
        {
            return DateTime.UtcNow - _lastHeardAt >= timeSpan;
        }

        public bool IsReplying()
        {
            return _isReplying;
        }

        public void BeginReply()
        {
            _isReplying = true;
        }

        public void EndReply()
        {
            _isReplying = false;
        }

        /// <summary>
        /// Drops old lines in chunks rather than one per line, so the start of each request stays the same and
        /// OpenAI's prompt cache keeps hitting.
        /// </summary>
        private void AddLine(ActorMemoryLine line)
        {
            _lines.Add(line);
            _lines.RemoveRange(0, ChunkedHistory.GetWindowStart(_lines.Count));
        }
    }
}
