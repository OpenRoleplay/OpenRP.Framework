using OpenRP.Framework.Database.Entities;
using System;

namespace OpenRP.Framework.Database.Models
{
    /// <summary>
    /// Cash put into an ATM. It reaches the account at CreditAt; until then it sits in the ATM at the stored position.
    /// </summary>
    public class BankPendingDepositModel : BaseModel
    {
        public ulong BankAccountId { get; set; }
        public ulong? BankCardId { get; set; }
        // Who deposited it.
        public ulong? CharacterId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreditAt { get; set; }
        // Null until the money is on the account.
        public DateTime? CreditedAt { get; set; }

        // Where the cash sits until then.
        public float AtmX { get; set; }
        public float AtmY { get; set; }
        public float AtmZ { get; set; }
        public int AtmInterior { get; set; }
        public int AtmVirtualWorld { get; set; }

        // Navigational Properties
        public BankAccountModel? BankAccount { get; set; }
        public BankCardModel? BankCard { get; set; }
        public CharacterModel? Character { get; set; }
    }
}
