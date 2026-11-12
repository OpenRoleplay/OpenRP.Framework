using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Actors.Components;
using OpenRP.Framework.Features.Banking.Enums;
using OpenRP.Framework.Features.Banking.Helpers;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Extensions;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenRP.Framework.Features.Banking.Services.Dialogs
{
    public partial class BankDialogService
    {
        /// <summary>
        /// One account, read fresh from the database: its details and what the player may do with it at this teller.
        /// </summary>
        private void OpenAccountMenu(Player player, ServerActor teller, ulong accountId, Action onBack)
        {
            (BankAccountModel? account, BankAccessLevel level) = GetAccountAndLevel(player, accountId);
            if (account == null || level < BankAccessLevel.View)
            {
                ShowMessage(player, "Bank", "Account", "You can't see that account anymore.", onBack);
                return;
            }

            BetterListDialog accountDialog = new BetterListDialog("Choose", "Back");
            accountDialog.SetTitle(TitleType.Children, "Bank", account.Name);
            int obanRow = accountDialog.AddRow($"OBAN: {Highlight(Oban.Format(account.Oban))}");
            accountDialog.AddRow($"Balance: {Highlight(FormatMoney(account.CurrencyId, account.Balance))}");
            accountDialog.AddRow($"Owner: {Highlight(_bankAccountService.GetOwnerName(account))}");
            int historyRow = accountDialog.AddRow("Recent transactions");
            int? depositRow = AddOperationRow(accountDialog, teller, level, BankOperation.Deposit, "Deposit cash");
            int? withdrawRow = AddOperationRow(accountDialog, teller, level, BankOperation.Withdraw, "Withdraw cash");
            int? transferRow = AddOperationRow(accountDialog, teller, level, BankOperation.Transfer, "Transfer money");
            int? accessRow = AddOperationRow(accountDialog, teller, level, BankOperation.ManageAccess, "Who has access");
            int? closeRow = account.OwnerType == BankAccountOwnerType.Bank ? null : AddOperationRow(accountDialog, teller, level, BankOperation.CloseAccount, "Close the account");

            _dialogService.ShowIgnoringDisconnect(player, accountDialog, response =>
            {
                Action reopenAccount = () => OpenAccountMenu(player, teller, accountId, onBack);
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (response.ItemIndex == obanRow)
                    ShowMessage(player, account.Name, "OBAN", $"Give this number to whoever sends you money:\n\n{Highlight(Oban.Format(account.Oban))}", reopenAccount);
                else if (response.ItemIndex == historyRow)
                    WithTeller(player, teller, BankOperation.View, () => ShowHistory(player, accountId, reopenAccount));
                else if (response.ItemIndex == depositRow)
                    OpenCashOperation(player, teller, accountId, BankOperation.Deposit, null, reopenAccount);
                else if (response.ItemIndex == withdrawRow)
                    OpenCashOperation(player, teller, accountId, BankOperation.Withdraw, null, reopenAccount);
                else if (response.ItemIndex == transferRow)
                    OpenTransfer(player, teller, accountId, null, reopenAccount);
                else if (response.ItemIndex == accessRow)
                    OpenAccessList(player, teller, accountId, reopenAccount);
                else if (response.ItemIndex == closeRow)
                    OpenCloseAccount(player, teller, accountId, onBack);
                else
                    reopenAccount();
            });
        }

        /// <summary>
        /// A row only when both the teller and the player are allowed.
        /// </summary>
        private int? AddOperationRow(BetterListDialog dialog, ServerActor teller, BankAccessLevel level, BankOperation operation, string label)
        {
            return _tellerService.IsAllowed(teller, operation) && level >= Entities.BankingRules.GetRequiredAccess(operation) ? dialog.AddRow(label) : null;
        }

        private void ShowHistory(Player player, ulong accountId, Action onBack)
        {
            (BankAccountModel? account, BankAccessLevel level) = GetAccountAndLevel(player, accountId);
            if (account == null || level < BankAccessLevel.View)
            {
                onBack();
                return;
            }

            List<BankTransactionModel> transactions = _bankAccountService.GetHistory(account);
            StringBuilder text = new StringBuilder();
            foreach (BankTransactionModel transaction in transactions)
            {
                text.AppendLine($"{transaction.CreatedOn.ToLocalTime():dd MMM HH:mm}  {GetTransactionLabel(transaction)}  {FormatMoney(account.CurrencyId, transaction.Amount, withSign: true)}  {ChatColor.LightGray}(balance {FormatMoney(account.CurrencyId, transaction.BalanceAfter)}){ChatColor.White}");
            }
            ShowMessage(player, account.Name, "Recent transactions", transactions.Count == 0 ? "Nothing has happened on this account yet." : text.ToString(), onBack);
        }

        private static string GetTransactionLabel(BankTransactionModel transaction)
        {
            string other = transaction.OtherOban == null ? string.Empty : $" {Oban.Format(transaction.OtherOban)}";
            return transaction.Type switch
            {
                BankTransactionType.Deposit => "Deposit",
                BankTransactionType.Withdrawal => "Withdrawal",
                BankTransactionType.TransferIn => $"From{other}",
                BankTransactionType.TransferOut => $"To{other}",
                BankTransactionType.Fee => "Monthly fee",
                _ => $"Fee from{other}"
            };
        }

        /// <summary>
        /// Deposit or withdraw: a suggested amount is shown for confirmation, otherwise the player types one.
        /// </summary>
        private void OpenCashOperation(Player player, ServerActor teller, ulong accountId, BankOperation operation, decimal? suggestedAmount, Action onBack)
        {
            BankAccountModel? account = _bankAccountService.GetAccount(accountId);
            if (account == null)
            {
                onBack();
                return;
            }

            string verb = operation == BankOperation.Deposit ? "Deposit" : "Withdraw";
            string direction = operation == BankOperation.Deposit ? "into" : "from";
            void Confirm(decimal amount) => ShowConfirmation(player, account.Name, verb, $"{verb} {Highlight(FormatMoney(account.CurrencyId, amount))} {direction} {account.Name}?",
                () => DoCashOperation(player, teller, accountId, operation, amount, onBack), () => AskAmount(player, account, verb, Confirm, onBack), "Confirm", "Change");

            if (suggestedAmount.HasValue && BankAmounts.IsValid(suggestedAmount.Value))
            {
                Confirm(suggestedAmount.Value);
                return;
            }
            AskAmount(player, account, verb, Confirm, onBack);
        }

        private void AskAmount(Player player, BankAccountModel account, string verb, Action<decimal> onEntered, Action onBack)
        {
            void OnEntered(string text)
            {
                decimal? amount = BankAmounts.Parse(text);
                if (amount == null)
                {
                    ShowMessage(player, account.Name, verb, "Enter an amount above 0, with at most 2 decimals, for example 250 or 99.50.", () => AskAmount(player, account, verb, onEntered, onBack));
                    return;
                }
                onEntered(amount.Value);
            }

            OpenTextInput(player, account.Name, verb, $"How much? The balance is {Highlight(FormatMoney(account.CurrencyId, account.Balance))}.", OnEntered, onBack);
        }

        private void DoCashOperation(Player player, ServerActor teller, ulong accountId, BankOperation operation, decimal amount, Action onBack)
        {
            WithTeller(player, teller, operation, () => _ = RunAsync(player, async () =>
            {
                string? error = operation == BankOperation.Deposit
                    ? await _bankTransactionService.DepositAsync(player, accountId, amount)
                    : await _bankTransactionService.WithdrawAsync(player, accountId, amount);
                if (!player.IsComponentAlive)
                {
                    return;
                }
                ContinueAfter(player, "Bank", error, () => OpenAccountMenu(player, teller, accountId, onBack), onBack);
            }));
        }

        /// <summary>
        /// The OBAN first, then the amount, then a confirmation that shows who receives it.
        /// </summary>
        private void OpenTransfer(Player player, ServerActor teller, ulong accountId, decimal? suggestedAmount, Action onBack)
        {
            BankAccountModel? account = _bankAccountService.GetAccount(accountId);
            if (account == null)
            {
                onBack();
                return;
            }

            void OnObanEntered(string text)
            {
                string? oban = Oban.Parse(text);
                BankAccountModel? target = oban == null ? null : _bankAccountService.GetAccountByOban(oban);
                if (target == null || target.ClosedAt != null)
                {
                    ShowMessage(player, account.Name, "Transfer", oban == null ? "That isn't a valid OBAN. Check the letters and numbers." : "No open account has that OBAN.",
                        () => OpenTransfer(player, teller, accountId, suggestedAmount, onBack));
                    return;
                }
                ChooseTransferAmount(player, teller, account, target, suggestedAmount, onBack);
            }

            OpenTextInput(player, account.Name, "Transfer", "Enter the OBAN of the account to send money to, for example SA42 PALO 0000 1234 56:", OnObanEntered, onBack);
        }

        private void ChooseTransferAmount(Player player, ServerActor teller, BankAccountModel account, BankAccountModel target, decimal? suggestedAmount, Action onBack)
        {
            void Confirm(decimal amount)
            {
                string question = $"Transfer {Highlight(FormatMoney(account.CurrencyId, amount))} from {account.Name} to {Highlight(Oban.Format(target.Oban))} ({_bankAccountService.GetOwnerName(target)})?";
                ShowConfirmation(player, account.Name, "Transfer", question, () => DoTransfer(player, teller, account.Id, target.Oban, amount, onBack),
                    () => AskAmount(player, account, "Transfer", Confirm, onBack), "Confirm", "Change");
            }

            if (suggestedAmount.HasValue && BankAmounts.IsValid(suggestedAmount.Value))
            {
                Confirm(suggestedAmount.Value);
                return;
            }
            AskAmount(player, account, "Transfer", Confirm, onBack);
        }

        private void DoTransfer(Player player, ServerActor teller, ulong accountId, string targetOban, decimal amount, Action onBack)
        {
            WithTeller(player, teller, BankOperation.Transfer, () =>
            {
                string? error = _bankTransactionService.Transfer(player, accountId, targetOban, amount);
                ContinueAfter(player, "Bank", error, () => OpenAccountMenu(player, teller, accountId, onBack), onBack);
            });
        }

        private (BankAccountModel? Account, BankAccessLevel Level) GetAccountAndLevel(Player player, ulong accountId)
        {
            BankAccountModel? account = _bankAccountService.GetAccount(accountId);
            if (account == null || !player.IsPlayerPlayingAsCharacter())
            {
                return (null, BankAccessLevel.None);
            }
            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();
            return (account, _bankAccountService.GetAccessLevel(character, account));
        }
    }
}
