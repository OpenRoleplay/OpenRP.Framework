namespace OpenRP.Framework.Features.Radios.Helpers
{
    /// <summary>
    /// Frequencies are whole numbers. 1000 to 9999 are open to anyone with a radio unless a frequency row says
    /// otherwise; protected and encrypted frequencies can use any number from 1 to 99999.
    /// </summary>
    public static class RadioFrequencyNumbers
    {
        public const uint Lowest = 1;
        public const uint Highest = 99999;
        public const uint LowestOpen = 1000;
        public const uint HighestOpen = 9999;

        /// <summary>
        /// The frequency in the text, or null when it isn't a whole number from 1 to 99999.
        /// </summary>
        public static uint? Parse(string text)
        {
            if (!uint.TryParse(text.Trim(), out uint number) || number < Lowest || number > Highest)
            {
                return null;
            }
            return number;
        }

        /// <summary>
        /// Whether the number is in the range anyone may use when no frequency row covers it.
        /// </summary>
        public static bool IsInOpenRange(uint number)
        {
            return number >= LowestOpen && number <= HighestOpen;
        }
    }
}
