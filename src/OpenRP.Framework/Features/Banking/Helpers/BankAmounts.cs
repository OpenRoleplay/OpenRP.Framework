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

        /// <summary>
        /// A monthly fee from 0 up to the maximum, with at most 2 decimals, such as "5", "$12.50" or "0.5". Null when it
        /// isn't one. Unlike Parse, it doesn't take thousands separators.
        /// </summary>
        public static decimal? ParseFee(string text, decimal maximum)
        {
            if (!decimal.TryParse(text.Trim().TrimStart('$'), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal fee))
            {
                return null;
            }
            return fee >= 0 && fee <= maximum && decimal.Round(fee, 2) == fee ? (decimal?)fee : null;
        }

        public static bool IsValid(decimal amount)
        {
            return amount > 0 && amount <= Maximum && decimal.Round(amount, 2) == amount;
        }
    }
}
