using OpenRP.Framework.Shared.Chat.Enums;

namespace OpenRP.Framework.Shared.Chat.Helpers
{
    /// <summary>
    /// Reads one written roleplay command line ("/me ...", "/do ..." or "/say ..."), as AI models write them. Only
    /// whole lines that start with a command count, so text such as "open 24/7" isn't cut apart.
    /// </summary>
    public static class RoleplayCommandLine
    {
        private static readonly (string Prefix, PlayerChatMessageType Type)[] Commands =
        {
            ("/me ", PlayerChatMessageType.ME),
            ("/do ", PlayerChatMessageType.DO),
            ("/say ", PlayerChatMessageType.TALK)
        };

        /// <summary>
        /// Whether the line is a /me, /do or /say with text; gives its type and its text without quotes around it.
        /// </summary>
        public static bool TryParse(string line, out PlayerChatMessageType type, out string text)
        {
            foreach ((string prefix, PlayerChatMessageType commandType) in Commands)
            {
                if (line.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    type = commandType;
                    text = line.Substring(prefix.Length).Trim().Trim('"').Trim();
                    return text.Length > 0;
                }
            }

            type = default;
            text = string.Empty;
            return false;
        }

        /// <summary>
        /// /me text without the name in front ("Finn waves" becomes "waves"), starting lowercase, because the chat
        /// already puts the name in front of /me.
        /// </summary>
        public static string CleanActionText(string text, string name)
        {
            if (text.StartsWith(name + " ", StringComparison.OrdinalIgnoreCase))
            {
                text = text.Substring(name.Length + 1);
            }

            return text.LowercaseFirstLetter();
        }

        /// <summary>
        /// The line as players see it in chat: "Name says: text", "* Name text *" or "text (( Name ))". Used to show AI
        /// actors what happened around them, in the same format as players.
        /// </summary>
        public static string FormatAsChat(PlayerChatMessageType type, string name, string text, string? accent = null)
        {
            return type switch
            {
                PlayerChatMessageType.ME => $"* {name} {text} *",
                PlayerChatMessageType.DO => $"{text} (( {name} ))",
                _ => String.IsNullOrEmpty(accent) ? $"{name} says: {text}" : $"{name} [{accent} accent] says: {text}"
            };
        }

        /// <summary>
        /// The line as the command it was written as, such as "/me waves".
        /// </summary>
        public static string ToCommandText(PlayerChatMessageType type, string text)
        {
            return type switch
            {
                PlayerChatMessageType.ME => $"/me {text}",
                PlayerChatMessageType.DO => $"/do {text}",
                _ => $"/say {text}"
            };
        }
    }
}
