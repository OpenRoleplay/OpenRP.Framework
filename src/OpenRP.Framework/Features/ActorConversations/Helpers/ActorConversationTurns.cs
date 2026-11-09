using OpenRP.Framework.Features.ActorConversations.Entities;
using System.Text.RegularExpressions;
using OpenRP.Framework.Shared.Chat.Enums;

namespace OpenRP.Framework.Features.ActorConversations.Helpers
{
    /// <summary>
    /// Decides who speaks next in a conversation between actors.
    /// </summary>
    public static class ActorConversationTurns
    {
        /// <summary>
        /// The character addressed by first name in the latest turn's speech; otherwise the one who spoke longest ago,
        /// so everyone gets turns. Never the character who just spoke, unless they're alone.
        /// </summary>
        public static ulong ChooseNextSpeaker(IReadOnlyList<ActorConversationParticipant> participants, IReadOnlyList<ActorConversationLine> lines, ulong currentSpeakerId)
        {
            List<ActorConversationParticipant> others = participants.Where(participant => participant.ActorId != currentSpeakerId).ToList();
            if (others.Count == 0)
            {
                return currentSpeakerId;
            }

            return GetAddressedParticipant(others, GetLatestTurnSpeech(lines, currentSpeakerId))?.ActorId
                ?? GetLongestSilentParticipant(others, lines).ActorId;
        }

        /// <summary>
        /// What the current speaker said in their latest turn: their /say lines at the end of the conversation.
        /// </summary>
        private static string GetLatestTurnSpeech(IReadOnlyList<ActorConversationLine> lines, ulong currentSpeakerId)
        {
            IEnumerable<string> speech = lines.Reverse()
                .TakeWhile(line => line.ActorId == currentSpeakerId)
                .Where(line => line.Type == PlayerChatMessageType.TALK)
                .Select(line => line.Text);
            return String.Join(" ", speech);
        }

        private static ActorConversationParticipant? GetAddressedParticipant(List<ActorConversationParticipant> others, string speech)
        {
            return others.FirstOrDefault(participant => IsNameMentioned(speech, GetFirstName(participant.Name)));
        }

        private static ActorConversationParticipant GetLongestSilentParticipant(List<ActorConversationParticipant> others, IReadOnlyList<ActorConversationLine> lines)
        {
            return others.OrderBy(participant => GetLastLineIndex(lines, participant.ActorId)).First();
        }

        /// <summary>
        /// The index of the participant's last line, or -1 if they haven't spoken yet (so they go first).
        /// </summary>
        private static int GetLastLineIndex(IReadOnlyList<ActorConversationLine> lines, ulong actorId)
        {
            for (int index = lines.Count - 1; index >= 0; index--)
            {
                if (lines[index].ActorId == actorId)
                {
                    return index;
                }
            }

            return -1;
        }

        private static string GetFirstName(string name)
        {
            return name.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? name;
        }

        /// <summary>
        /// Whether the name appears as a whole word, so "Sue" doesn't match "Suede".
        /// </summary>
        private static bool IsNameMentioned(string speech, string name)
        {
            return Regex.IsMatch(speech, $@"\b{Regex.Escape(name)}\b", RegexOptions.IgnoreCase);
        }
    }
}
