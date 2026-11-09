using SampSharp.Entities;

namespace OpenRP.Framework.Features.ActorConversations.Components
{
    /// <summary>
    /// On a player who watches a conversation: which line they see next, and when. Every viewer plays at their own
    /// pace, so nobody waits for anybody else.
    /// </summary>
    public class ActorConversationViewer : Component
    {
        private DateTime _nextLineDueAt = DateTime.UtcNow;

        public ActorConversationViewer(string conversationKey)
        {
            ConversationKey = conversationKey;
        }

        public string ConversationKey { get; }
        public int NextLineIndex { get; private set; }
        public bool HasSeenLoginReminder { get; private set; }

        public bool IsNextLineDue()
        {
            return DateTime.UtcNow >= _nextLineDueAt;
        }

        /// <summary>
        /// Moves on to the next line, which is due after <paramref name="delay"/>.
        /// </summary>
        public void MoveToNextLine(TimeSpan delay)
        {
            NextLineIndex++;
            _nextLineDueAt = DateTime.UtcNow + delay;
        }

        public void MarkLoginReminderSeen()
        {
            HasSeenLoginReminder = true;
        }
    }
}
