using System.Globalization;

namespace OpenRP.Framework.Features.Fishing.Helpers
{
    public static class FishWeightText
    {
        private const uint GramsPerKilogram = 1000;

        /// <summary>
        /// The weight as players read it, for example "850 g" or "12.4 kg".
        /// </summary>
        public static string GetWeightText(uint weightInGrams)
        {
            if (weightInGrams < GramsPerKilogram)
            {
                return $"{weightInGrams} g";
            }

            double weightInKilograms = weightInGrams / (double)GramsPerKilogram;
            return $"{weightInKilograms.ToString("0.#", CultureInfo.InvariantCulture)} kg";
        }
    }
}
