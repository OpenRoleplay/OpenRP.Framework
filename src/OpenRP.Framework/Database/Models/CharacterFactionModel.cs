namespace OpenRP.Framework.Database.Models
{
    public class CharacterFactionModel
    {
        public ulong Id { get; set; }
        public ulong CharacterId { get; set; }
        public ulong FactionId { get; set; }

        // Navigational Properties
        public CharacterModel Character { get; set; }
    }
}