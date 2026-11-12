using OpenRP.Framework.Database.Entities;
using OpenRP.Framework.Features.Banking.Enums;
using System;

namespace OpenRP.Framework.Database.Models
{
    /// <summary>
    /// A bank account. Its balance only changes through single database updates (see Features/Banking), never by
    /// saving a loaded copy, so several servers can't lose or create money.
    /// </summary>
    public class BankAccountModel : BaseModel
    {
        // Open Bank Account Number, without spaces, for example SA42PALO0000123456.
        public string Oban { get; set; }
        public ulong BankId { get; set; }
        public ulong CurrencyId { get; set; }
        public decimal Balance { get; set; }
        public BankAccountOwnerType OwnerType { get; set; }
        // The character, faction, property or bank id, depending on OwnerType.
        public ulong OwnerId { get; set; }
        public string Name { get; set; }
        // Null while open; a closed account keeps its history and its OBAN is never reused.
        public DateTime? ClosedAt { get; set; }
        // When the next monthly fee is due; null for accounts that pay none (house accounts).
        public DateTime? NextFeeAt { get; set; }

        // Navigational Properties
        public BankModel? Bank { get; set; }
        public CurrencyModel? Currency { get; set; }

        public BankAccountModel()
        {
            Oban = string.Empty;
            Name = string.Empty;
        }
    }
}
