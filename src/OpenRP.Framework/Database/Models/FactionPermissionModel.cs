using OpenRP.Framework.Database.Entities;

namespace OpenRP.Framework.Database.Models
{
    /// <summary>
    /// A permission granted to a faction, by an admin or by its parent faction. A sub-faction's grant only counts while
    /// its parent faction has the permission too.
    /// </summary>
    public class FactionPermissionModel : BaseModel
    {
        public ulong FactionId { get; set; }
        public ulong PermissionId { get; set; }

        // Navigational Properties
        public FactionModel? Faction { get; set; }
        public PermissionModel? Permission { get; set; }
    }
}
