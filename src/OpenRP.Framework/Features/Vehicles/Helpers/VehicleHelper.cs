using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Vehicles.Helpers
{
    public class VehicleHelper
    {
        public VehicleHelper()
        {

        }

        public string GenerateCarPlate(ulong vehicleId, string countryCode)
        {
            // Ensure the country code is two uppercase letters
            if (string.IsNullOrWhiteSpace(countryCode) || countryCode.Length != 2)
                throw new ArgumentException("Country code must be exactly 2 letters.", nameof(countryCode));

            // Calculate the three-digit number (000 to 999)
            ulong numberPart = vehicleId % 1000;

            // Calculate the two-letter suffix
            ulong suffixIndex1 = (vehicleId / 1000) % 26;            // First letter (0-25)
            ulong suffixIndex2 = (vehicleId / (1000 * 26)) % 26;     // Second letter (0-25)

            char suffixLetter1 = (char)('A' + suffixIndex1);
            char suffixLetter2 = (char)('A' + suffixIndex2);

            string suffix = $"{suffixLetter1}{suffixLetter2}";

            // Format the plate as 'XX-123-YY'
            return $"{countryCode.ToUpper()}-{numberPart:D3}-{suffix}";
        }

    }
}
