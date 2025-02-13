namespace OpenRP.Framework.Models
{
    public class CharacterSkillModel
    {
        public ulong Id { get; set; }
        public ulong CharacterId { get; set; }
        public ulong SkillId { get; set; }
        public uint Level { get; set; }
        public uint Experience { get; set; }
        public DateTime? LastUsedDate { get; set; } = null;

        // Navigational Properties
        public CharacterModel Character { get; set; }
        public SkillModel Skill { get; set; }

        // Constructor
        public CharacterSkillModel()
        {
            Level = 0;
            Experience = 0;
        }
    }
}