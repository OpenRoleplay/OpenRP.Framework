using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Actors.Components;
using OpenRP.Framework.Features.Banking.Entities;
using OpenRP.Framework.Features.Banking.Enums;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Extensions;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Banking.Services.Dialogs
{
    public partial class BankDialogService
    {
        public void OpenBankMenu(Player player, ServerActor teller)
        {
            WithTeller(player, teller, BankOperation.View, () => ShowBankMenu(player, teller));
        }

        public void OpenOperation(Player player, ServerActor teller, BankOperation operation, decimal? suggestedAmount)
        {
            WithTeller(player, teller, operation, () =>
            {
                Action close = () => { };
                switch (operation)
                {
                    case BankOperation.OpenAccount:
                        OpenOpenAccount(player, teller, close);
                        break;
                    case BankOperation.View:
                        OpenAccountOverview(player, teller);
                        break;
                    default:
                        PickAccount(player, teller, operation, account => OpenAccountOperation(player, teller, account, operation, suggestedAmount, close), close);
                        break;
                }
            });
        }

        private void ShowBankMenu(Player player, ServerActor teller)
        {
            BankModel? bank = _tellerService.GetBank(teller);
            if (bank == null || !player.IsPlayerPlayingAsCharacter())
            {
                return;
            }

            BetterListDialog menuDialog = new BetterListDialog("Choose", "Close");
            menuDialog.SetTitle(TitleType.Children, bank.Name);
            Dictionary<int, BankAccountModel> accountsByRow = new Dictionary<int, BankAccountModel>();
            foreach (BankAccountModel account in GetAccountsFor(player, bank, BankAccessLevel.View))
            {
                accountsByRow[menuDialog.AddRow(GetAccountLabel(account))] = account;
            }
            int? openRow = _tellerService.IsAllowed(teller, BankOperation.OpenAccount) ? menuDialog.AddRow($"{ChatColor.CornflowerBlue}Open an account") : null;
            if (accountsByRow.Count == 0 && openRow == null)
            {
                ShowMessage(player, bank.Name, "No accounts", $"You have no accounts at {bank.Name}.", () => { });
                return;
            }

            _dialogService.ShowIgnoringDisconnect(player, menuDialog, response =>
            {
                Action reopenMenu = () => OpenBankMenu(player, teller);
                if (response.Response != DialogResponse.LeftButton)
                    return;
                if (response.ItemIndex == openRow)
                    OpenOpenAccount(player, teller, reopenMenu);
                else if (accountsByRow.TryGetValue(response.ItemIndex, out BankAccountModel? account))
                    OpenAccountMenu(player, teller, account.Id, reopenMenu);
            });
        }

        /// <summary>
        /// Straight to the account when the player has one there, otherwise the menu with all of them.
        /// </summary>
        private void OpenAccountOverview(Player player, ServerActor teller)
        {
            BankModel? bank = _tellerService.GetBank(teller);
            List<BankAccountModel> accounts = bank == null ? new List<BankAccountModel>() : GetAccountsFor(player, bank, BankAccessLevel.View);
            if (accounts.Count == 1)
            {
                OpenAccountMenu(player, teller, accounts[0].Id, () => OpenBankMenu(player, teller));
                return;
            }
            ShowBankMenu(player, teller);
        }

        /// <summary>
        /// Picks one of the player's accounts at the teller's bank that allow the operation; with one, it's taken at once.
        /// </summary>
        private void PickAccount(Player player, ServerActor teller, BankOperation operation, Action<BankAccountModel> onPicked, Action onBack)
        {
            BankModel? bank = _tellerService.GetBank(teller);
            List<BankAccountModel> accounts = bank == null ? new List<BankAccountModel>() : GetAccountsFor(player, bank, BankingRules.GetRequiredAccess(operation));
            if (operation == BankOperation.CloseAccount)
            {
                accounts.RemoveAll(account => account.OwnerType == BankAccountOwnerType.Bank);
            }

            if (accounts.Count == 0)
            {
                ShowMessage(player, bank?.Name ?? "Bank", "No account", $"You have no account at {bank?.Name ?? "this bank"} you can do that with.", onBack);
                return;
            }
            if (accounts.Count == 1)
            {
                onPicked(accounts[0]);
                return;
            }

            BetterListDialog pickerDialog = new BetterListDialog("Choose", "Back");
            pickerDialog.SetTitle(TitleType.Children, bank!.Name, "Which account?");
            Dictionary<int, BankAccountModel> accountsByRow = new Dictionary<int, BankAccountModel>();
            foreach (BankAccountModel account in accounts)
            {
                accountsByRow[pickerDialog.AddRow(GetAccountLabel(account))] = account;
            }

            _dialogService.ShowIgnoringDisconnect(player, pickerDialog, response =>
            {
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (accountsByRow.TryGetValue(response.ItemIndex, out BankAccountModel? account))
                    onPicked(account);
            });
        }

        private void OpenAccountOperation(Player player, ServerActor teller, BankAccountModel account, BankOperation operation, decimal? suggestedAmount, Action onBack)
        {
            switch (operation)
            {
                case BankOperation.Deposit:
                case BankOperation.Withdraw:
                    OpenCashOperation(player, teller, account.Id, operation, suggestedAmount, onBack);
                    break;
                case BankOperation.Transfer:
                    OpenTransfer(player, teller, account.Id, suggestedAmount, onBack);
                    break;
                case BankOperation.ManageAccess:
                    OpenAccessList(player, teller, account.Id, onBack);
                    break;
                case BankOperation.CloseAccount:
                    OpenCloseAccount(player, teller, account.Id, onBack);
                    break;
                default:
                    OpenAccountMenu(player, teller, account.Id, onBack);
                    break;
            }
        }

        private List<BankAccountModel> GetAccountsFor(Player player, BankModel bank, BankAccessLevel minimumLevel)
        {
            if (!player.IsPlayerPlayingAsCharacter())
            {
                return new List<BankAccountModel>();
            }
            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();
            return _bankAccountService.GetAccountsAt(character, bank.Id, minimumLevel);
        }
    }
}
