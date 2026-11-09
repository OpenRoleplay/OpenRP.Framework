using OpenRP.Framework.Features.ActorRoleplay.Entities;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Helpers;

namespace OpenRP.Framework.Features.ActorRoleplay.Helpers
{
    /// <summary>
    /// Turns a model's reply into /me, /do and /say lines; anything else the model writes is ignored.
    /// </summary>
    public static class ActorReplyParser
    {
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
            if (!RoleplayCommandLine.TryParse(line, out PlayerChatMessageType type, out string text))
            {
                return null;
            }

            if (type == PlayerChatMessageType.ME)
            {
                text = RoleplayCommandLine.CleanActionText(text, actorName);
            }

            return new ActorRoleplayLine(type, text);
        }
    }
}
