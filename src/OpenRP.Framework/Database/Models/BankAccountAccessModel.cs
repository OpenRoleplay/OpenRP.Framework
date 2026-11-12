using OpenRP.Framework.Database.Entities;
using OpenRP.Framework.Features.Banking.Enums;

namespace OpenRP.Framework.Database.Models
{
    /// <summary>
    /// A character the owner gave access to an account: view only, or full use.
    /// </summary>
    public class BankAccountAccessModel : BaseModel
    {
        public ulong BankAccountId { get; set; }
        public ulong CharacterId { get; set; }
        public BankAccessLevel Level { get; set; }

        // Navigational Properties
        public BankAccountModel? BankAccount { get; set; }
        public CharacterModel? Character { get; set; }
    }
}
