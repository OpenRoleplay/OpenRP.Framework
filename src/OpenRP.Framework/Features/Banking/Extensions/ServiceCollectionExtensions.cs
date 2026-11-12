using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.ActorRoleplay.Extensions;
using OpenRP.Framework.Features.Actors.Extensions;
using OpenRP.Framework.Features.Banking.Actions;
using OpenRP.Framework.Features.Banking.Entities;
using OpenRP.Framework.Features.Banking.Enums;
using OpenRP.Framework.Features.Banking.Services;
using OpenRP.Framework.Features.Banking.Services.Dialogs;
using OpenRP.Framework.Features.Jurisdictions.Extensions;
using System;

namespace OpenRP.Framework.Features.Banking.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        private const string AmountDetail = "the amount, only if they said one";

        public static IServiceCollection AddBanking(this IServiceCollection self)
        {
            return self
                .AddSingleton<IBankingDatabase, BankingDatabase>()
                .AddSingleton<IBankService, BankService>()
                .AddSingleton<IBankAccountService, BankAccountService>()
                .AddSingleton<IBankTransactionService, BankTransactionService>()
                .AddSingleton<ITellerService, TellerService>()
                .AddSingleton<IBankEditorService, BankEditorService>()
                .AddSingleton<IBankDialogService, BankDialogService>()
                .AddJurisdictionDeletionGuard<BankJurisdictionGuard>()
                .AddTellerPermissions()
                .AddTellerActions();
        }

        private static IServiceCollection AddTellerPermissions(this IServiceCollection self)
        {
            return self
                .AddActorPermission(BankingRules.TellerPermissions[BankOperation.View], "Show customers their accounts")
                .AddActorPermission(BankingRules.TellerPermissions[BankOperation.OpenAccount], "Open accounts for customers")
                .AddActorPermission(BankingRules.TellerPermissions[BankOperation.CloseAccount], "Close customers' accounts")
                .AddActorPermission(BankingRules.TellerPermissions[BankOperation.Deposit], "Take cash deposits")
                .AddActorPermission(BankingRules.TellerPermissions[BankOperation.Withdraw], "Pay out cash withdrawals")
                .AddActorPermission(BankingRules.TellerPermissions[BankOperation.Transfer], "Make transfers for customers")
                .AddActorPermission(BankingRules.TellerPermissions[BankOperation.ManageAccess], "Change who has access to an account");
        }

        /// <summary>
        /// One action per bank dialog, so a teller opens exactly what the customer asks for.
        /// </summary>
        private static IServiceCollection AddTellerActions(this IServiceCollection self)
        {
            return self
                .AddTellerAction("bank_menu", "open the full bank menu, when they want to see everything or it's unclear what they want", null, BankOperation.View, opensFullMenu: true)
                .AddTellerAction("bank_view", "show their account and balance, when they ask about their balance or account", null, BankOperation.View)
                .AddTellerAction("bank_open_account", "open a new account, when they want one", null, BankOperation.OpenAccount)
                .AddTellerAction("bank_deposit", "take a cash deposit, when they want to put money in", AmountDetail, BankOperation.Deposit)
                .AddTellerAction("bank_withdraw", "pay out cash, when they want to take money out", AmountDetail, BankOperation.Withdraw)
                .AddTellerAction("bank_transfer", "make a transfer, when they want to send money to another account", AmountDetail, BankOperation.Transfer)
                .AddTellerAction("bank_access", "change who has access to their account, when they want to add or remove someone", null, BankOperation.ManageAccess)
                .AddTellerAction("bank_close_account", "close an account, when they want to close one", null, BankOperation.CloseAccount);
        }

        private static IServiceCollection AddTellerAction(this IServiceCollection self, string name, string description, string? detailDescription, BankOperation operation, bool opensFullMenu = false)
        {
            return self.AddActorAction(provider => new BankTellerAction(name, description, detailDescription, operation, opensFullMenu,
                provider.GetRequiredService<ITellerService>(), provider.GetRequiredService<IBankService>(), provider.GetRequiredService<IBankDialogService>()));
        }
    }
}
