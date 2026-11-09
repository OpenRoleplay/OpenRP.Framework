using OpenRP.Framework.Features.ActorConversations.Entities;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.ActorConversations.Components
{
    /// <summary>
    /// A conversation between actors, with every line written so far. Viewers replay these lines from the start, so
    /// joining costs nothing; new turns are only written once a viewer gets close to the end.
    /// </summary>
    public class ActorConversation : Component
    {
        private readonly List<ActorConversationLine> _lines = new List<ActorConversationLine>();
        private bool _isGenerating;
        private DateTime _canGenerateAfter = DateTime.MinValue;

        public ActorConversation(string key, List<ulong> actorIds, string? situation)
        {
            Key = key;
            ActorIds = actorIds;
            Situation = situation;
            NextSpeakerId = actorIds.FirstOrDefault();
        }

        public string Key { get; }
        public IReadOnlyList<ulong> ActorIds { get; }

        /// <summary>
        /// Where the conversation takes place, such as "A family barbecue on the beach", told to every speaker.
        /// </summary>
        public string? Situation { get; }

        /// <summary>
        /// The actors' names and prompts. Filled in when first needed, because conversations are created at startup,
        /// possibly before the actors are loaded.
        /// </summary>
        public IReadOnlyList<ActorConversationParticipant> Participants { get; private set; } = new List<ActorConversationParticipant>();

        public ulong NextSpeakerId { get; private set; }

        public bool HasParticipants()
        {
            return Participants.Count > 0;
        }

        public void SetParticipants(List<ActorConversationParticipant> participants)
        {
            Participants = participants;
        }

        public ActorConversationParticipant? GetParticipant(ulong actorId)
        {
            return Participants.FirstOrDefault(participant => participant.ActorId == actorId);
        }

        public void SetNextSpeaker(ulong actorId)
        {
            NextSpeakerId = actorId;
        }

        public ActorConversationLine GetLine(int index)
        {
            return _lines[index];
        }

        public int GetLineCount()
        {
            return _lines.Count;
        }

        /// <summary>
        /// The last <paramref name="count"/> lines, oldest first, as what the next speaker remembers.
        /// </summary>
        public List<ActorConversationLine> GetLastLines(int count)
        {
            return _lines.Skip(Math.Max(0, _lines.Count - count)).ToList();
        }

        /// <summary>
        /// All lines, oldest first, to find out who spoke longest ago.
        /// </summary>
        public IReadOnlyList<ActorConversationLine> GetLines()
        {
            return _lines;
        }

        /// <summary>
        /// Adds new lines, but never more than <paramref name="maxLines"/> in total.
        /// </summary>
        public void AddLines(IEnumerable<ActorConversationLine> lines, int maxLines)
        {
            _lines.AddRange(lines.Take(Math.Max(0, maxLines - _lines.Count)));
        }

        public bool IsFull(int maxLines)
        {
            return _lines.Count >= maxLines;
        }

        /// <summary>
        /// Whether a new turn may be requested now: no request is running, and the last failed one was long enough ago.
        /// </summary>
        public bool CanGenerate()
        {
            return !_isGenerating && DateTime.UtcNow >= _canGenerateAfter;
        }

        public void BeginGenerating()
        {
            _isGenerating = true;
        }

        /// <summary>
        /// Ends the request; after a failure, <paramref name="retryAfter"/> keeps the next one from following right away.
        /// </summary>
        public void EndGenerating(TimeSpan retryAfter)
        {
            _isGenerating = false;
            _canGenerateAfter = DateTime.UtcNow + retryAfter;
        }
    }
}
