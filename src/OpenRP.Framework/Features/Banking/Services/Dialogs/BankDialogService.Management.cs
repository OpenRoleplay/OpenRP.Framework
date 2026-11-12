using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Actors.Components;
using OpenRP.Framework.Features.Banking.Entities;
using OpenRP.Framework.Features.Banking.Enums;
using OpenRP.Framework.Features.Banking.Helpers;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Features.Jurisdictions.Components;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Extensions;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Banking.Services.Dialogs
{
    public partial class BankDialogService
    {
        private const string DefaultAccountName = "Checking";

        /// <summary>
        /// For whom (yourself, or a faction you bank for), then the name, then a confirmation with the currency and fee.
        /// </summary>
        private void OpenOpenAccount(Player player, ServerActor teller, Action onBack)
        {
            if (!player.IsPlayerPlayingAsCharacter())
            {
                return;
            }
            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();
            List<Faction> factions = _bankAccountService.GetFactionsToBankFor(character);
            if (factions.Count == 0)
            {
                OpenAccountName(player, teller, BankAccountOwnerType.Character, character.GetId(), "yourself", onBack);
                return;
            }

            BetterListDialog ownerDialog = new BetterListDialog("Choose", "Back");
            ownerDialog.SetTitle(TitleType.Children, "Bank", "Open an account", "For whom?");
            int yourselfRow = ownerDialog.AddRow("Yourself");
            Dictionary<int, Faction> factionsByRow = new Dictionary<int, Faction>();
            foreach (Faction faction in factions)
            {
                factionsByRow[ownerDialog.AddRow(faction.GetName())] = faction;
            }

            _dialogService.ShowIgnoringDisconnect(player, ownerDialog, response =>
            {
                Action reopenOwner = () => OpenOpenAccount(player, teller, onBack);
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (response.ItemIndex == yourselfRow)
                    OpenAccountName(player, teller, BankAccountOwnerType.Character, character.GetId(), "yourself", reopenOwner);
                else if (factionsByRow.TryGetValue(response.ItemIndex, out Faction? faction))
                    OpenAccountName(player, teller, BankAccountOwnerType.Faction, faction.GetId(), faction.GetName(), reopenOwner);
            });
        }

        private void OpenAccountName(Player player, ServerActor teller, BankAccountOwnerType ownerType, ulong ownerId, string ownerLabel, Action onBack)
        {
            void OnNamed(string text)
            {
                string name = String.IsNullOrWhiteSpace(text) ? DefaultAccountName : text.Trim();
                ConfirmOpenAccount(player, teller, ownerType, ownerId, ownerLabel, name, () => OpenAccountName(player, teller, ownerType, ownerId, ownerLabel, onBack));
            }

            OpenTextInput(player, "Bank", "Open an account", $"Give the account a name, for example Savings. Leave it empty for {Highlight(DefaultAccountName)}.", OnNamed, onBack);
        }

        private void ConfirmOpenAccount(Player player, ServerActor teller, BankAccountOwnerType ownerType, ulong ownerId, string ownerLabel, string name, Action onBack)
        {
            BankModel? bank = _tellerService.GetBank(teller);
            Jurisdiction? jurisdiction = bank == null ? null : _jurisdictionService.GetJurisdiction(bank.JurisdictionId);
            ulong? currencyId = jurisdiction == null ? null : _jurisdictionService.GetCurrencyId(jurisdiction);
            if (bank == null || currencyId == null)
            {
                ShowMessage(player, "Bank", "Open an account", "This bank can't open accounts right now.", onBack);
                return;
            }

            string currencyName = _currencyService.GetCurrencies().FirstOrDefault(currency => currency.Id == currencyId)?.Name ?? "the local currency";
            string fee = bank.MonthlyFee > 0 ? $"The fee is {Highlight(FormatMoney(currencyId.Value, bank.MonthlyFee))} every 30 days, taken from the balance." : "There's no monthly fee.";
            string question = $"Open the account {Highlight(name)} at {bank.Name} for {ownerLabel}?\n\nIt holds {currencyName}. {fee}";
            ShowConfirmation(player, "Bank", "Open an account", question, () => DoOpenAccount(player, teller, bank, ownerType, ownerId, name, onBack), onBack);
        }

        private void DoOpenAccount(Player player, ServerActor teller, BankModel bank, BankAccountOwnerType ownerType, ulong ownerId, string name, Action onBack)
        {
            WithTeller(player, teller, BankOperation.OpenAccount, () =>
            {
                (BankAccountModel? account, string? error) = _bankAccountService.OpenAccount(player.GetPlayerCurrentlyPlayingAsCharacter(), bank, ownerType, ownerId, name);
                ContinueAfter(player, "Bank", error, () => ShowMessage(player, account!.Name, "Opened",
                    $"The account is open. Its OBAN is {Highlight(Oban.Format(account.Oban))}; give it to whoever sends you money.",
                    () => OpenAccountMenu(player, teller, account.Id, () => { })), onBack);
            });
        }

        private void OpenCloseAccount(Player player, ServerActor teller, ulong accountId, Action onBack)
        {
            BankAccountModel? account = _bankAccountService.GetAccount(accountId);
            if (account == null)
            {
                onBack();
                return;
            }

            string question = $"Close {Highlight(account.Name)} ({Oban.Format(account.Oban)})? It can't be opened again.";
            if (account.Balance != 0)
            {
                ShowMessage(player, account.Name, "Close", $"The balance is {FormatMoney(account.CurrencyId, account.Balance)}. An account can only be closed at exactly 0.", onBack);
                return;
            }

            ShowConfirmation(player, account.Name, "Close", question, () => WithTeller(player, teller, BankOperation.CloseAccount, () =>
            {
                string? error = _bankAccountService.CloseAccount(player.GetPlayerCurrentlyPlayingAsCharacter(), account);
                ContinueAfter(player, "Bank", error, () => ShowMessage(player, account.Name, "Closed", "The account is closed.", () => { }), onBack);
            }), onBack);
        }

        private void OpenAccessList(Player player, ServerActor teller, ulong accountId, Action onBack)
        {
            BankAccountModel? account = _bankAccountService.GetAccount(accountId);
            if (account == null)
            {
                onBack();
                return;
            }

            BetterListDialog accessDialog = new BetterListDialog("Choose", "Back");
            accessDialog.SetTitle(TitleType.Children, account.Name, "Who has access");
            int addRow = accessDialog.AddRow($"{ChatColor.CornflowerBlue}Give someone access");
            Dictionary<int, BankAccessEntry> entriesByRow = new Dictionary<int, BankAccessEntry>();
            foreach (BankAccessEntry entry in _bankAccountService.GetAccessList(account))
            {
                entriesByRow[accessDialog.AddRow($"{entry.CharacterName} {ChatColor.LightGray}({GetLevelLabel(entry.Level)})")] = entry;
            }

            _dialogService.ShowIgnoringDisconnect(player, accessDialog, response =>
            {
                Action reopenList = () => OpenAccessList(player, teller, accountId, onBack);
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (response.ItemIndex == addRow)
                    OpenTextInput(player, account.Name, "Give access", "Enter their full name, for example John Smith:",
                        name => PickAccessLevel(player, name, level => GrantAccess(player, teller, account, name, level, reopenList), reopenList), reopenList);
                else if (entriesByRow.TryGetValue(response.ItemIndex, out BankAccessEntry? entry))
                    OpenAccessEntry(player, teller, account, entry, reopenList);
            });
        }

        private void OpenAccessEntry(Player player, ServerActor teller, BankAccountModel account, BankAccessEntry entry, Action onBack)
        {
            BetterListDialog entryDialog = new BetterListDialog("Choose", "Back");
            entryDialog.SetTitle(TitleType.Children, account.Name, entry.CharacterName);
            int levelRow = entryDialog.AddRow($"Change access {ChatColor.LightGray}(now {GetLevelLabel(entry.Level)})");
            int removeRow = entryDialog.AddRow("Remove access");

            _dialogService.ShowIgnoringDisconnect(player, entryDialog, response =>
            {
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (response.ItemIndex == levelRow)
                    PickAccessLevel(player, entry.CharacterName, level => GrantAccess(player, teller, account, entry.CharacterName, level, onBack), onBack);
                else if (response.ItemIndex == removeRow)
                    WithTeller(player, teller, BankOperation.ManageAccess, () =>
                    {
                        string? error = _bankAccountService.RevokeAccess(player.GetPlayerCurrentlyPlayingAsCharacter(), account, entry.AccessId);
                        ContinueAfter(player, account.Name, error, onBack, onBack);
                    });
            });
        }

        private void PickAccessLevel(Player player, string characterName, Action<BankAccessLevel> onPicked, Action onBack)
        {
            BetterListDialog levelDialog = new BetterListDialog("Choose", "Back");
            levelDialog.SetTitle(TitleType.Children, "Bank", characterName);
            int viewRow = levelDialog.AddRow($"View only {ChatColor.LightGray}(balance and history)");
            int fullRow = levelDialog.AddRow($"Full use {ChatColor.LightGray}(also deposit, withdraw and transfer)");

            _dialogService.ShowIgnoringDisconnect(player, levelDialog, response =>
            {
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (response.ItemIndex == viewRow)
                    onPicked(BankAccessLevel.View);
                else if (response.ItemIndex == fullRow)
                    onPicked(BankAccessLevel.Full);
            });
        }

        private void GrantAccess(Player player, ServerActor teller, BankAccountModel account, string characterName, BankAccessLevel level, Action onBack)
        {
            WithTeller(player, teller, BankOperation.ManageAccess, () =>
            {
                string? error = _bankAccountService.GrantAccess(player.GetPlayerCurrentlyPlayingAsCharacter(), account, characterName, level);
                ContinueAfter(player, account.Name, error, onBack, onBack);
            });
        }

        private static string GetLevelLabel(BankAccessLevel level)
        {
            return level == BankAccessLevel.Full ? "full use" : "view only";
        }
    }
}
