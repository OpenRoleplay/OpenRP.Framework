using OpenRP.Framework.Database.Entities;

namespace OpenRP.Framework.Database.Models
{
    public class CharacterModel : BaseModel
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Accent { get; set; }
        public ulong? InventoryId { get; set; }
        public int Skin { get; set; }


        // Navigational Properties
        public AccountModel Account { get; set; }
        public InventoryModel? Inventory { get; set; }
        public List<CharacterSkillModel> Skills { get; set; }
        public CharacterPreferencesModel? Preferences { get; set; }
        public List<VehicleModel>? Vehicles { get; set; }
        public List<CharacterFactionModel>? Factions { get; set; }

        // Constructor
        public CharacterModel()
        {

        }
    }
}