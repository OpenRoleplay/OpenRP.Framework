using SampSharp.Entities.SAMP.Commands.Parsers;
using System;
using System.Globalization;

namespace OpenRP.Framework.Shared.Commands.Helpers
{
    /// <summary>
    /// Reads float command arguments with a decimal point (or comma) on any server culture; SampSharp's own parser
    /// uses the server culture, where a point can mean thousands (nl-NL turns 1234.56 into 123456).
    /// </summary>
    public class InvariantFloatParser : ICommandParameterParser
    {
        private readonly WordParser _wordParser = new WordParser();

        public bool TryParse(IServiceProvider services, ref string inputText, out object? result)
        {
            result = null;
            if (!_wordParser.TryParse(services, ref inputText, out object? word) || word is not string text)
            {
                return false;
            }

            // Coordinates and similar values never use thousands separators, so a comma can only be a decimal one.
            if (!float.TryParse(text.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
            {
                return false;
            }

            result = value;
            return true;
        }
    }
}
