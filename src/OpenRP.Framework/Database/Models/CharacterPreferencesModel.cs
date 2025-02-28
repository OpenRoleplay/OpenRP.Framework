using OpenRP.Framework.Database.Entities;

namespace OpenRP.Framework.Database.Models
{
    public class CharacterPreferencesModel : BaseModel
    {
        public ulong CharacterId { get; set; }
        public bool HardcoreMode { get; set; }
        public bool AllowCharacterKill { get; set; }
        public bool AllowRape { get; set; }
        public bool AllowSlavery { get; set; }
        public bool AllowWorldEvents { get; set; }
        public ulong DefaultCurrencyId { get; set; }

        // Navigational Properties
        public CharacterModel Character { get; set; }

        // Constructor
        public CharacterPreferencesModel()
        {
            HardcoreMode = false;
            AllowCharacterKill = false;
            AllowRape = false;
            AllowSlavery = false;
            AllowWorldEvents = false;
            DefaultCurrencyId = 1;
        }
    }
}