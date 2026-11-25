using OpenRP.Framework.Database.Entities;
using OpenRP.Framework.Features.Banking.Enums;

namespace OpenRP.Framework.Database.Models
{
    /// <summary>
    /// One line of an account's history. CreatedOn is when it happened.
    /// </summary>
    public class BankTransactionModel : BaseModel
    {
        public ulong BankAccountId { get; set; }
        // Positive for money in, negative for money out.
        public decimal Amount { get; set; }
        public decimal BalanceAfter { get; set; }
        public BankTransactionType Type { get; set; }
        // The other account of a transfer or fee.
        public string? OtherOban { get; set; }
        // Who did it, when a character did.
        public ulong? CharacterId { get; set; }
        // The card used, for ATM withdrawals and deposits and card payments.
        public ulong? BankCardId { get; set; }
        // What a payment was for, for example "Card payment: groceries".
        public string? Description { get; set; }

        // Navigational Properties
        public BankAccountModel? BankAccount { get; set; }
        public CharacterModel? Character { get; set; }
        public BankCardModel? BankCard { get; set; }
    }
}
