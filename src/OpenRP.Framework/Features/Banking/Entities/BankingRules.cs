using OpenRP.Framework.Features.Banking.Enums;
using System;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Banking.Entities
{
    /// <summary>
    /// The fixed numbers and names of the banking rules.
    /// </summary>
    public static class BankingRules
    {
        /// <summary>
        /// The highest fee a bank may charge per account per 30 days. Later, governments may set their own.
        /// </summary>
        public const decimal MaxMonthlyFee = 100m;

        /// <summary>
        /// Only the fee can make an account negative, and never below this.
        /// </summary>
        public const decimal LowestBalance = -1000m;

        public static readonly TimeSpan FeeInterval = TimeSpan.FromDays(30);

        public const int HistoryLength = 20;
        public const int AccountNameMaxLength = 32;
        public const int BankNameMaxLength = 32;

        /// <summary>
        /// The actor permission a teller needs for each operation.
        /// </summary>
        public static readonly IReadOnlyDictionary<BankOperation, string> TellerPermissions = new Dictionary<BankOperation, string>
        {
            { BankOperation.View, "bank.view" },
            { BankOperation.OpenAccount, "bank.open_account" },
            { BankOperation.CloseAccount, "bank.close_account" },
            { BankOperation.Deposit, "bank.deposit" },
            { BankOperation.Withdraw, "bank.withdraw" },
            { BankOperation.Transfer, "bank.transfer" },
            { BankOperation.ManageAccess, "bank.manage_access" }
        };

        /// <summary>
        /// The access a customer needs on an account for each operation.
        /// </summary>
        public static BankAccessLevel GetRequiredAccess(BankOperation operation)
        {
            return operation switch
            {
                BankOperation.View => BankAccessLevel.View,
                BankOperation.Deposit or BankOperation.Withdraw or BankOperation.Transfer => BankAccessLevel.Full,
                _ => BankAccessLevel.Manage
            };
        }
    }
}
