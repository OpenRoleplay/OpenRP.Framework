using OpenRP.Framework.Database.Entities;
using System;

namespace OpenRP.Framework.Database.Models
{
    /// <summary>
    /// A debit card of one character for one account. The card item in an inventory points to it, so the card works
    /// for whoever holds it and knows the PIN.
    /// </summary>
    public class BankCardModel : BaseModel
    {
        public ulong BankAccountId { get; set; }
        // Whose card it is: the character it was issued to.
        public ulong CharacterId { get; set; }
        // The 4 digits printed on the card, so cards of one account can be told apart.
        public string Digits { get; set; }
        // A name the holder gave the card, or null.
        public string? Name { get; set; }
        // BCrypt, like passwords.
        public string PinHash { get; set; }
        public int WrongPinAttempts { get; set; }
        // Set by the owner or by too many wrong PINs; the bank can unblock it.
        public DateTime? BlockedAt { get; set; }
        // Set when the card was replaced; a cancelled card never works again.
        public DateTime? CancelledAt { get; set; }

        // Navigational Properties
        public BankAccountModel? BankAccount { get; set; }
        public CharacterModel? Character { get; set; }

        public BankCardModel()
        {
            Digits = string.Empty;
            PinHash = string.Empty;
        }
    }
}
