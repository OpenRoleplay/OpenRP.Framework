using OpenRP.Framework.Database.Entities;

namespace OpenRP.Framework.Database.Models
{
    /// <summary>
    /// A character's membership of a faction, with their rank in it. A character has at most one per faction.
    /// </summary>
    public class FactionMemberModel : BaseModel
    {
        public ulong CharacterId { get; set; }
        public ulong FactionId { get; set; }
        public ulong FactionRankId { get; set; }

        /// <summary>
        /// True for the character's active faction, which faction chat goes to when they're in several factions.
        /// </summary>
        public bool IsActive { get; set; }

        // Navigational Properties
        public CharacterModel? Character { get; set; }
        public FactionModel? Faction { get; set; }
        public FactionRankModel? FactionRank { get; set; }
    }
}
