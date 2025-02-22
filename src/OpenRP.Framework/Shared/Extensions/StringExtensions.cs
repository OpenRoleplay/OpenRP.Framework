using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Shared.Extensions
{
    public static partial class StringExtensions
    {
        /// <summary>
        /// Converts the first character of the string to lowercase.
        /// If the string is empty, null, or the first character is not a letter, it returns the original string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="culture">Optional culture info. Defaults to current culture.</param>
        /// <returns>The string with the first character lowercased.</returns>
        public static string LowercaseFirstLetter(this string input, CultureInfo culture = null)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            culture ??= CultureInfo.CurrentCulture;

            char firstChar = input[0];
            char lowerChar = char.ToLower(firstChar, culture);

            // If the first character is already lowercase or not a letter, return the original string
            if (firstChar == lowerChar)
                return input;

            // If the string has only one character, return the lowercased character as a string
            if (input.Length == 1)
                return lowerChar.ToString();

            // Combine the lowercased first character with the rest of the string
            return lowerChar + input.Substring(1);
        }
    }
}
