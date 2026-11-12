using OpenRP.Framework.Database.Entities;

namespace OpenRP.Framework.Database.Models
{
    /// <summary>
    /// An actor working as a teller for a bank. What it may do is set by its actor permissions.
    /// </summary>
    public class BankTellerModel : BaseModel
    {
        public ulong ActorId { get; set; }
        public ulong BankId { get; set; }

        // Navigational Properties
        public ActorModel? Actor { get; set; }
        public BankModel? Bank { get; set; }
    }
}
