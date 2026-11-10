using System.Linq;

namespace OpenRP.Framework.Features.Phones.Helpers
{
    /// <summary>
    /// Writing and reading phone numbers. Player phones have 7 digits, written 382-0142; numbers below 1000 belong to
    /// the server, such as 911.
    /// </summary>
    public static class PhoneNumbers
    {
        public const uint LowestPlayerNumber = 1000000;
        public const uint HighestPlayerNumber = 9999999;
        // Numbers below this are reserved for the server.
        public const uint ServerNumberLimit = 1000;

        public static bool IsServerNumber(uint number)
        {
            return number < ServerNumberLimit;
        }

        /// <summary>
        /// The number as people write it: 382-0142 for 7 digits, otherwise just the digits.
        /// </summary>
        public static string Format(uint number)
        {
            string digits = number.ToString();
            return digits.Length == 7 ? $"{digits[..3]}-{digits[3..]}" : digits;
        }

        /// <summary>
        /// Reads "3820142", "382-0142" or "382 0142". Null when it isn't a number of 1 to 7 digits.
        /// </summary>
        public static uint? Parse(string text)
        {
            string digits = new string(text.Where(character => character != '-' && character != ' ').ToArray());
            if (digits.Length == 0 || digits.Length > 7 || !digits.All(char.IsAsciiDigit))
            {
                return null;
            }
            return uint.Parse(digits);
        }
    }
}
