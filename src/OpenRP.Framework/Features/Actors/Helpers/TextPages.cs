using System.Text;

namespace OpenRP.Framework.Features.Actors.Helpers
{
    /// <summary>
    /// Splits long text into pages of short lines for a message dialog. SA-MP dialogs don't wrap lines, so a long
    /// prompt would otherwise make one very wide line.
    /// </summary>
    public static class TextPages
    {
        public const int LineLength = 80;
        public const int LinesPerPage = 15;

        public static List<string> Split(string text)
        {
            List<string> lines = WrapLines(text);
            List<string> pages = new List<string>();
            for (int index = 0; index < lines.Count; index += LinesPerPage)
            {
                pages.Add(String.Join("\n", lines.Skip(index).Take(LinesPerPage)));
            }

            return pages.Count == 0 ? new List<string> { string.Empty } : pages;
        }

        private static List<string> WrapLines(string text)
        {
            List<string> lines = new List<string>();
            foreach (string paragraph in text.Replace("\r", "").Split('\n'))
            {
                lines.AddRange(WrapParagraph(paragraph));
            }
            return lines;
        }

        private static List<string> WrapParagraph(string paragraph)
        {
            List<string> lines = new List<string>();
            StringBuilder line = new StringBuilder();
            foreach (string word in paragraph.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                if (line.Length > 0 && line.Length + 1 + word.Length > LineLength)
                {
                    lines.Add(line.ToString());
                    line.Clear();
                }

                if (line.Length > 0)
                {
                    line.Append(' ');
                }
                line.Append(word);
            }

            lines.Add(line.ToString());
            return lines;
        }
    }
}
