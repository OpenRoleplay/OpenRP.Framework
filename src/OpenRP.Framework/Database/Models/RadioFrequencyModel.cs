using OpenRP.Framework.Database.Entities;
using OpenRP.Framework.Features.Radios.Enums;

namespace OpenRP.Framework.Database.Models
{
    /// <summary>
    /// A protected or encrypted frequency. Frequencies without a row follow the default rules: 1000 to 9999 are open
    /// to everyone, anything else has no signal.
    /// </summary>
    public class RadioFrequencyModel : BaseModel
    {
        public uint Number { get; set; }
        public string Name { get; set; }
        public RadioFrequencyMode Mode { get; set; }

        public RadioFrequencyModel()
        {
            Name = string.Empty;
            Mode = RadioFrequencyMode.Protected;
        }
    }
}
