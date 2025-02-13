namespace OpenRP.Framework.Models
{
    public class CharacterPreferencesModel
    {
        public ulong Id { get; set; }
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