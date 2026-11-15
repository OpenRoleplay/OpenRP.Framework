using System;
using System.Globalization;

namespace OpenRP.Framework.Features.Maps.Helpers
{
    /// <summary>
    /// Reads the short values the map dialogs ask for. Each parser returns false when the text isn't a valid value.
    /// </summary>
    public static class MapDialogInput
    {
        /// <summary>
        /// Reads a model ID. Model 0 isn't a model; negative IDs are custom models.
        /// </summary>
        public static bool TryParseModelId(string text, out int modelId)
        {
            return int.TryParse(text.Trim(), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out modelId) && modelId != 0;
        }

        /// <summary>
        /// Reads "world interior", two whole numbers separated by a space.
        /// </summary>
        public static bool TryParseWorldAndInterior(string text, out int virtualWorld, out int interior)
        {
            string[] words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            virtualWorld = 0;
            interior = 0;
            return words.Length == 2
                && int.TryParse(words[0], NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out virtualWorld)
                && int.TryParse(words[1], NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out interior);
        }

        /// <summary>
        /// Reads a stream distance in metres. Empty means the default, which reads as null. The editor checks the range, so
        /// a number that's out of range still parses.
        /// </summary>
        public static bool TryParseStreamDistance(string text, out float? streamDistance)
        {
            string trimmed = text.Trim();
            if (trimmed.Length == 0)
            {
                streamDistance = null;
                return true;
            }

            if (float.TryParse(trimmed, NumberStyles.Float, CultureInfo.InvariantCulture, out float distance) && float.IsFinite(distance))
            {
                streamDistance = distance;
                return true;
            }

            streamDistance = null;
            return false;
        }

        /// <summary>
        /// Splits "model description" at the first space. The description is null when nothing follows the model.
        /// </summary>
        public static (string ModelText, string? Description) SplitModelAndDescription(string input)
        {
            string trimmed = input.Trim();
            int separator = trimmed.IndexOf(' ');
            if (separator < 0)
            {
                return (trimmed, null);
            }

            string description = trimmed[(separator + 1)..].Trim();
            return (trimmed[..separator], description.Length == 0 ? null : description);
        }
    }
}
