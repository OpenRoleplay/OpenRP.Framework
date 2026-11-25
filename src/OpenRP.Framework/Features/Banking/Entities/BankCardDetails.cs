using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Banking.Enums;

namespace OpenRP.Framework.Features.Banking.Entities
{
    /// <summary>
    /// A card with what's printed on it: its account and bank.
    /// </summary>
    public record BankCardDetails(BankCardModel Card, BankAccountModel Account, BankModel Bank)
    {
        /// <summary>
        /// "SAB debit card: Savings ...4821", or "Groceries (SAB debit card: Savings ...4821)" for a named card.
        /// </summary>
        public string GetLabel()
        {
            string printed = $"{Bank.Code} debit card: {Account.Name} ...{Card.Digits}";
            return string.IsNullOrEmpty(Card.Name) ? printed : $"{Card.Name} ({printed})";
        }

        public BankCardStatus GetStatus()
        {
            if (Card.CancelledAt != null)
                return BankCardStatus.Cancelled;
            if (Card.BlockedAt != null)
                return BankCardStatus.Blocked;
            if (Account.ClosedAt != null)
                return BankCardStatus.AccountClosed;
            return BankCardStatus.Active;
        }

        /// <summary>
        /// The status in a few words, for example "Blocked".
        /// </summary>
        public string GetStatusText()
        {
            return GetStatus() switch
            {
                BankCardStatus.Cancelled => "Cancelled",
                BankCardStatus.Blocked => "Blocked",
                BankCardStatus.AccountClosed => "Account closed",
                _ => "Active"
            };
        }
    }
}
