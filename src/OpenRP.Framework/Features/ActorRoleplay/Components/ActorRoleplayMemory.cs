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
