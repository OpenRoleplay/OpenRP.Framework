using System;
using System.Globalization;
using System.Linq;

namespace OpenRP.Framework.Features.Banking.Helpers
{
    /// <summary>
    /// Reads amounts that players type or say, such as "500", "$1,250.50" or "1250.5".
    /// </summary>
    public static class BankAmounts
    {
        public const decimal Maximum = 100_000_000m;

        /// <summary>
        /// The amount, or null when it isn't a positive number with at most 2 decimals and at most the maximum.
        /// </summary>
        public static decimal? Parse(string? text)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            string cleaned = new string(text.Where(character => char.IsDigit(character) || character == '.').ToArray());
            if (!decimal.TryParse(cleaned, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal amount))
            {
                return null;
            }
            return IsValid(amount) ? amount : null;
        }

        public static bool IsValid(decimal amount)
        {
            return amount > 0 && amount <= Maximum && decimal.Round(amount, 2) == amount;
        }
    }
}
