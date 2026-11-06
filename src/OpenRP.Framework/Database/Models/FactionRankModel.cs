using OpenRP.Framework.Database.Entities;

namespace OpenRP.Framework.Database.Models
{
    /// <summary>
    /// A rank in a faction. Ranks form a tree: the rank without a parent is the faction's top (leader) rank.
    /// </summary>
    public class FactionRankModel : BaseModel
    {
        public ulong FactionId { get; set; }
        public ulong? ParentRankId { get; set; }
        public string Name { get; set; } = string.Empty;

        // Navigational Properties
        public FactionModel? Faction { get; set; }
        public FactionRankModel? ParentRank { get; set; }
    }
}
