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
        /// The fee cap when no government has set one.
        /// </summary>
        public const decimal DefaultMaxMonthlyFee = 100m;

        /// <summary>
        /// The highest fee cap a government may set.
        /// </summary>
        public const decimal HighestMaxMonthlyFee = 10000m;

        /// <summary>
        /// Only the fee can make an account negative, and never below this.
        /// </summary>
        public const decimal LowestBalance = -1000m;

        public static readonly TimeSpan FeeInterval = TimeSpan.FromDays(30);

        public const int HistoryLength = 20;
        public const int AccountNameMaxLength = 32;
        public const int BankNameMaxLength = 32;
        public const int TransactionDescriptionMaxLength = 64;

        /// <summary>
        /// The seeded "Debit card" item.
        /// </summary>
        public const int BankCardItemId = 68;

        public const int CardDigitsLength = 4;
        public const int CardNameMaxLength = 32;
        public const int PinLength = 4;

        /// <summary>
        /// The work factor of the PIN hash, the same as passwords.
        /// </summary>
        public const int PinHashWorkFactor = 11;

        /// <summary>
        /// Wrong PINs in a row that block a card until the bank unblocks it.
        /// </summary>
        public const int MaxWrongPinAttempts = 3;

        /// <summary>
        /// The model of the ATM object (kmb_atm1). Every placed map object of it is an ATM.
        /// </summary>
        public const int AtmModelId = 2942;

        /// <summary>
        /// How close a player must stand to an ATM to use it.
        /// </summary>
        public const float AtmRange = 2.0f;

        /// <summary>
        /// The most cash one card can take from ATMs in a period, in the account's currency.
        /// </summary>
        public const decimal AtmWithdrawalLimit = 1000m;
        public static readonly TimeSpan AtmWithdrawalLimitPeriod = TimeSpan.FromHours(24);

        /// <summary>
        /// How long cash put into an ATM takes to reach the account.
        /// </summary>
        public static readonly TimeSpan AtmDepositDelay = TimeSpan.FromHours(2);

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
            { BankOperation.ManageAccess, "bank.manage_access" },
            { BankOperation.ManageCards, "bank.cards" }
        };

        /// <summary>
        /// The access a customer needs on an account for each operation.
        /// </summary>
        public static BankAccessLevel GetRequiredAccess(BankOperation operation)
        {
            return operation switch
            {
                BankOperation.View => BankAccessLevel.View,
                BankOperation.Deposit or BankOperation.Withdraw or BankOperation.Transfer or BankOperation.ManageCards => BankAccessLevel.Full,
                _ => BankAccessLevel.Manage
            };
        }
    }
}
