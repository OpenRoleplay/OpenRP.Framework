using OpenRP.Framework.Database.Entities;

namespace OpenRP.Framework.Database.Models
{
    /// <summary>
    /// A permission given to everyone with a faction rank.
    /// </summary>
    public class FactionRankPermissionModel : BaseModel
    {
        public ulong FactionRankId { get; set; }
        public ulong PermissionId { get; set; }

        // Navigational Properties
        public FactionRankModel? FactionRank { get; set; }
        public PermissionModel? Permission { get; set; }
    }
}
