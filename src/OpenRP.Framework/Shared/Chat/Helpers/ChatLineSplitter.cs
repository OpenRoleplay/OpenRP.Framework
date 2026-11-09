using System.Text;

namespace OpenRP.Framework.Shared.Chat.Helpers
{
    /// <summary>
    /// Splits long text into parts that fit in one chat message. The SA-MP client drops messages over 144 characters,
    /// and the name and formatting around the text need room too.
    /// </summary>
    public static class ChatLineSplitter
    {
        public const int MaxPartLength = 110;
        private const string ContinuationMarker = " ...";

        /// <summary>
        /// The text in parts of at most <see cref="MaxPartLength"/> characters, split between words, with "..." where
        /// one part continues in the next.
        /// </summary>
        public static List<string> Split(string text)
        {
            List<string> parts = new List<string>();
            StringBuilder currentPart = new StringBuilder();

            foreach (string word in text.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                // Leave room for the marker that's added when the part continues.
                if (currentPart.Length > 0 && currentPart.Length + 1 + word.Length > MaxPartLength - ContinuationMarker.Length)
                {
                    parts.Add(currentPart + ContinuationMarker);
                    currentPart.Clear().Append("...");
                }

                if (currentPart.Length > 0)
                {
                    currentPart.Append(' ');
                }
                currentPart.Append(word);
            }

            if (currentPart.Length > 0)
            {
                parts.Add(currentPart.ToString());
            }

            return parts;
        }
    }
}
