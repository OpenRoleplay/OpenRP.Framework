using OpenRP.Framework.Database.Entities;

namespace OpenRP.Framework.Database.Models
{
    public class CharacterFactionModel : BaseModel
    {
        public ulong CharacterId { get; set; }
        public ulong FactionId { get; set; }

        // Navigational Properties
        public CharacterModel Character { get; set; }
    }
}