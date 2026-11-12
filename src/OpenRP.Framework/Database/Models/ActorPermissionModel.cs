using OpenRP.Framework.Database.Entities;

namespace OpenRP.Framework.Database.Models
{
    /// <summary>
    /// Something an actor is allowed to do, such as "bank.deposit". Actors can do nothing without one.
    /// </summary>
    public class ActorPermissionModel : BaseModel
    {
        public ulong ActorId { get; set; }
        public string Permission { get; set; }

        // Navigational Properties
        public ActorModel? Actor { get; set; }

        public ActorPermissionModel()
        {
            Permission = string.Empty;
        }
    }
}
