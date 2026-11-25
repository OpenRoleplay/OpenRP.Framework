using SampSharp.Entities.SAMP;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Banking.Services
{
    /// <summary>
    /// Every change of a balance. Each one is a single database update in a transaction, logged in the account's
    /// history, so several servers can't lose or create money. Methods return why it didn't happen, or null.
    /// </summary>
    public interface IBankTransactionService
    {
        /// <summary>
        /// Takes the cash from the player's wallet, then puts it in the account. Gives the cash back when the account
        /// can't take it.
        /// </summary>
        Task<string?> DepositAsync(Player player, ulong accountId, decimal amount);

        /// <summary>
        /// Takes the money from the account, then gives the player the cash. Puts it back when the cash doesn't fit.
        /// </summary>
        Task<string?> WithdrawAsync(Player player, ulong accountId, decimal amount);

        /// <summary>
        /// Moves the money to the account with this OBAN, in one database transaction.
        /// </summary>
        string? Transfer(Player player, ulong fromAccountId, string toOban, decimal amount);

        /// <summary>
        /// Charges the monthly fee of every account that's due, into its bank's own account.
        /// </summary>
        void ChargeDueFees();

        /// <summary>
        /// Pays from the card's account to the receiving account, in one database transaction. The PIN must have been
        /// checked before. The description says what it was for, in both histories.
        /// </summary>
        string? PayWithCard(Player player, ulong cardId, ulong receiverAccountId, decimal amount, string description);

        /// <summary>
        /// Takes the cash from the player's wallet, with change, and pays it into the receiving account. Gives the cash
        /// back when the account can't take it.
        /// </summary>
        Task<string?> PayInCashAsync(Player player, ulong receiverAccountId, ulong currencyId, decimal amount, string description);
    }
}
