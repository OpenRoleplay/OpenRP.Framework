using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Database;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Banking.Enums;
using System.Linq;

namespace OpenRP.Framework.Features.Banking.Helpers
{
    /// <summary>
    /// What one side of a money movement records in the account's history, besides the amount.
    /// </summary>
    internal record BankLedgerEntry(BankTransactionType Type, ulong? CharacterId, string? OtherOban = null, ulong? BankCardId = null, string? Description = null);

    /// <summary>
    /// The only code that changes balances: one database update per side, run inside a transaction by the caller, and
    /// a history line for it. Shared by the teller, ATMs, cards and payments.
    /// </summary>
    internal static class BankLedger
    {
        /// <summary>
        /// Adds the money to an open account and logs it. False when the account is closed.
        /// </summary>
        public static bool Credit(BaseDataContext db, ulong accountId, decimal amount, BankLedgerEntry entry)
        {
            int updated = db.BankAccounts
                .Where(row => row.Id == accountId && row.ClosedAt == null)
                .ExecuteUpdate(setters => setters.SetProperty(row => row.Balance, row => row.Balance + amount));
            if (updated != 1)
            {
                return false;
            }
            AddTransaction(db, accountId, amount, entry);
            return true;
        }

        /// <summary>
        /// Takes the money from an open account when the balance stays at least <paramref name="minimumBalanceAfter"/>,
        /// and logs it. False otherwise.
        /// </summary>
        public static bool Debit(BaseDataContext db, ulong accountId, decimal amount, decimal minimumBalanceAfter, BankLedgerEntry entry)
        {
            int updated = db.BankAccounts
                .Where(row => row.Id == accountId && row.ClosedAt == null && row.Balance - amount >= minimumBalanceAfter)
                .ExecuteUpdate(setters => setters.SetProperty(row => row.Balance, row => row.Balance - amount));
            if (updated != 1)
            {
                return false;
            }
            AddTransaction(db, accountId, -amount, entry);
            return true;
        }

        private static void AddTransaction(BaseDataContext db, ulong accountId, decimal amount, BankLedgerEntry entry)
        {
            decimal balanceAfter = db.BankAccounts.Where(row => row.Id == accountId).Select(row => row.Balance).First();
            db.BankTransactions.Add(new BankTransactionModel
            {
                BankAccountId = accountId,
                Amount = amount,
                BalanceAfter = balanceAfter,
                Type = entry.Type,
                OtherOban = entry.OtherOban,
                CharacterId = entry.CharacterId,
                BankCardId = entry.BankCardId,
                Description = entry.Description
            });
            db.SaveChanges();
        }
    }
}
