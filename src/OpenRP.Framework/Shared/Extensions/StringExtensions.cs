using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenRP.Framework.Shared.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Inserts newlines into the given text so that no line exceeds the specified length by too much,
        /// while preserving existing newlines and attempting to break at word boundaries.
        /// Also does not count SA-MP color codes (e.g., {FFFFFF}) towards line length.
        /// </summary>
        /// <param name="text">The original text, which may contain existing newline characters and SA-MP color codes.</param>
        /// <param name="lineLength">The approximate maximum visible length of each line before inserting a newline.</param>
        /// <returns>The processed string with inserted newlines, preserving color codes and existing newlines.</returns>
        public static string InsertNewlinesAtLength(this string text, int lineLength = 200)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            // Split on existing newline sequences: \r\n, \r, or \n
            string[] originalLines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            var resultLines = new List<string>();

            foreach (string line in originalLines)
            {
                // If line is empty, just add an empty line.
                if (string.IsNullOrEmpty(line))
                {
                    resultLines.Add(string.Empty);
                    continue;
                }

                // Split this line on spaces to identify words
                string[] words = line.Split(' ');

                // We will accumulate words here, ensuring we don't exceed the visible limit
                var currentLine = new List<string>();
                int currentLength = 0; // This will track visible length (without color codes)

                foreach (var word in words)
                {
                    int visibleLength = word.GetVisibleLength();

                    // If adding this word would exceed the visible limit
                    int addedLength = (currentLine.Count == 0) ? visibleLength : (visibleLength + 1);
                    if (currentLine.Count > 0 && (currentLength + addedLength) > lineLength)
                    {
                        // Join the current line and add to result
                        resultLines.Add(string.Join(" ", currentLine));
                        currentLine.Clear();
                        currentLength = 0;
                    }

                    // Add the word to the current line
                    currentLine.Add(word);
                    currentLength = (currentLine.Count == 1) ? visibleLength : currentLength + visibleLength + 1;
                }

                // Add the last line if there are any leftover words
                if (currentLine.Count > 0)
                {
                    resultLines.Add(string.Join(" ", currentLine));
                }
            }

            // Rejoin the lines with '\n'
            return string.Join("\n", resultLines);
        }

        /// <summary>
        /// Gets the visible length of a string by removing SA-MP color codes (e.g., {FFFFFF})
        /// </summary>
        public static int GetVisibleLength(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return 0;

            // Remove color codes of the form {XXXXXX}
            // Regex: \{[^\}]*\} will match any sequence { ... }
            string withoutCodes = Regex.Replace(input, @"\{[^\}]*\}", "");
            return withoutCodes.Length;
        }
    }
}
