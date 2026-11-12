using System;
using System.Linq;
using System.Numerics;
using System.Text;

namespace OpenRP.Framework.Features.Banking.Helpers
{
    /// <summary>
    /// Open Bank Account Numbers, built like IBANs: a 2-letter jurisdiction tag, 2 check digits, a 4-letter bank code and
    /// 10 digits, for example SA42PALO0000123456. The check digits are the IBAN mod-97 check, which catches typos.
    /// </summary>
    public static class Oban
    {
        public const int Length = 18;
        private const int AccountNumberLength = 10;

        /// <summary>
        /// A new OBAN for the tag, the bank code and a 10-digit account number.
        /// </summary>
        public static string Create(string jurisdictionTag, string bankCode, long accountNumber)
        {
            string tag = jurisdictionTag.ToUpperInvariant();
            string number = accountNumber.ToString().PadLeft(AccountNumberLength, '0');
            string checkDigits = GetCheckDigits(tag, bankCode.ToUpperInvariant() + number);
            return tag + checkDigits + bankCode.ToUpperInvariant() + number;
        }

        /// <summary>
        /// A random 10-digit account number.
        /// </summary>
        public static long CreateRandomAccountNumber()
        {
            return Random.Shared.NextInt64(0, 10_000_000_000);
        }

        /// <summary>
        /// The OBAN without spaces and in capitals, or null when it isn't one or its check digits are wrong.
        /// </summary>
        public static string? Parse(string text)
        {
            string oban = new string(text.Where(character => !char.IsWhiteSpace(character)).ToArray()).ToUpperInvariant();
            return HasValidShape(oban) && IsChecksumValid(oban) ? oban : null;
        }

        /// <summary>
        /// In groups of four, as people write it: SA42 PALO 0000 1234 56.
        /// </summary>
        public static string Format(string oban)
        {
            StringBuilder formatted = new StringBuilder();
            for (int index = 0; index < oban.Length; index++)
            {
                if (index > 0 && index % 4 == 0)
                {
                    formatted.Append(' ');
                }
                formatted.Append(oban[index]);
            }
            return formatted.ToString();
        }

        private static bool HasValidShape(string oban)
        {
            return oban.Length == Length
                && oban[..2].All(char.IsAsciiLetterUpper)
                && oban[2..4].All(char.IsAsciiDigit)
                && oban[4..8].All(char.IsAsciiLetterUpper)
                && oban[8..].All(char.IsAsciiDigit);
        }

        /// <summary>
        /// Moving the first four characters to the end and reading letters as 10 to 35 gives 1 modulo 97.
        /// </summary>
        private static bool IsChecksumValid(string oban)
        {
            return ToNumber(oban[4..] + oban[..4]) % 97 == 1;
        }

        private static string GetCheckDigits(string tag, string bankCodeAndNumber)
        {
            BigInteger remainder = ToNumber(bankCodeAndNumber + tag + "00") % 97;
            return (98 - (int)remainder).ToString("00");
        }

        private static BigInteger ToNumber(string text)
        {
            StringBuilder digits = new StringBuilder();
            foreach (char character in text)
            {
                digits.Append(char.IsAsciiLetter(character) ? (character - 'A' + 10).ToString() : character.ToString());
            }
            return BigInteger.Parse(digits.ToString());
        }
    }
}
