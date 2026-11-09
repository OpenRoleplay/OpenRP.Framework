using OpenRP.Framework.Features.ActorRoleplay.Entities;
using OpenRP.Framework.Shared.Chat.Enums;

namespace OpenRP.Framework.Features.ActorRoleplay.Helpers
{
    /// <summary>
    /// Turns a model's reply into /me, /do and /say lines. Only whole lines that start with a command count, so text
    /// such as "open 24/7" isn't cut apart; anything else the model writes is ignored.
    /// </summary>
    public static class ActorReplyParser
    {
        private static readonly (string Prefix, PlayerChatMessageType Type)[] Commands =
        {
            ("/me ", PlayerChatMessageType.ME),
            ("/do ", PlayerChatMessageType.DO),
            ("/say ", PlayerChatMessageType.TALK)
        };

        /// <summary>
        /// The reply's command lines, at most <see cref="ActorRoleplayPromptBuilder.MaxReplyLines"/>.
        /// </summary>
        public static List<ActorRoleplayLine> Parse(string reply, string actorName)
        {
            return reply
                .Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(line => ParseLine(line, actorName))
                .OfType<ActorRoleplayLine>()
                .Take(ActorRoleplayPromptBuilder.MaxReplyLines)
                .ToList();
        }

        private static ActorRoleplayLine? ParseLine(string line, string actorName)
        {
            foreach ((string prefix, PlayerChatMessageType type) in Commands)
            {
                if (!line.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string text = CleanText(line.Substring(prefix.Length), type, actorName);
                return text.Length == 0 ? null : new ActorRoleplayLine(type, text);
            }

            return null;
        }

        private static string CleanText(string text, PlayerChatMessageType type, string actorName)
        {
            text = text.Trim().Trim('"').Trim();

            if (type != PlayerChatMessageType.ME)
            {
                return text;
            }

            // The chat already puts the name in front of /me, so "Finn waves" would show the name twice.
            if (text.StartsWith(actorName + " ", StringComparison.OrdinalIgnoreCase))
            {
                text = text.Substring(actorName.Length + 1);
            }

            return text.LowercaseFirstLetter();
        }
    }
}
