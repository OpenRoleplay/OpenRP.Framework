using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Actors.Components;
using OpenRP.Framework.Features.Banking.Entities;
using OpenRP.Framework.Features.Banking.Helpers;
using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Features.Jurisdictions.Components;
using OpenRP.Framework.Features.Properties.Components;
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
        public void OpenBankEditor(Player player)
        {
            BetterListDialog banksDialog = new BetterListDialog("Choose", "Close");
            banksDialog.SetTitle(TitleType.Children, EditorTitle);
            int createRow = banksDialog.AddRow($"{ChatColor.CornflowerBlue}Create a bank");
            int findRow = banksDialog.AddRow($"{ChatColor.CornflowerBlue}Look up an OBAN");
            Dictionary<int, BankModel> banksByRow = new Dictionary<int, BankModel>();
            foreach (BankModel bank in _bankService.GetBanks())
            {
                banksByRow[banksDialog.AddRow($"{bank.Name} {ChatColor.LightGray}({bank.Code})")] = bank;
            }

            _dialogService.ShowIgnoringDisconnect(player, banksDialog, response =>
            {
                Action reopenEditor = () => OpenBankEditor(player);
                if (response.Response != DialogResponse.LeftButton)
                    return;
                if (response.ItemIndex == createRow)
                    OpenCreateBank(player, reopenEditor);
                else if (response.ItemIndex == findRow)
                    OpenFindOban(player, reopenEditor);
                else if (banksByRow.TryGetValue(response.ItemIndex, out BankModel? bank))
                    OpenBankActions(player, bank.Id, reopenEditor);
            });
        }

        private void OpenBankActions(Player player, ulong bankId, Action onBack)
        {
            BankModel? bank = _bankService.GetBank(bankId);
            if (bank == null)
            {
                onBack();
                return;
            }

            BetterListDialog actionsDialog = new BetterListDialog("Choose", "Back");
            actionsDialog.SetTitle(TitleType.Children, EditorTitle, bank.Name);
            int renameRow = actionsDialog.AddRow("Rename");
            actionsDialog.AddRow($"Code: {Highlight(bank.Code)} {ChatColor.LightGray}(part of every OBAN, can't change)");
            actionsDialog.AddRow($"Jurisdiction: {Highlight(_jurisdictionService.GetJurisdiction(bank.JurisdictionId)?.GetName() ?? "none")}");
            int feeRow = actionsDialog.AddRow($"Monthly fee: {Highlight(bank.MonthlyFee.ToString("0.00"))} {ChatColor.LightGray}(at most {BankingRules.MaxMonthlyFee:0})");
            int factionRow = actionsDialog.AddRow($"Run by: {Highlight(GetFactionName(bank.RunByFactionId))}");
            int branchesRow = actionsDialog.AddRow($"Branches ({_bankService.GetBranches(bank.Id).Count})");
            int tellersRow = actionsDialog.AddRow($"Tellers ({_bankService.GetTellers(bank.Id).Count})");
            int propertyAccountRow = actionsDialog.AddRow("Open an account for a property");

            _dialogService.ShowIgnoringDisconnect(player, actionsDialog, response =>
            {
                Action reopenActions = () => OpenBankActions(player, bankId, onBack);
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (response.ItemIndex == renameRow)
                    OpenTextInput(player, EditorTitle, "Rename", $"Enter the new name for {Highlight(bank.Name)}:",
                        name => ContinueAfter(player, EditorTitle, _bankEditorService.RenameBank(bank, name), reopenActions, reopenActions), reopenActions);
                else if (response.ItemIndex == feeRow)
                    OpenTextInput(player, EditorTitle, "Monthly fee", $"Enter the fee per account per 30 days, from 0 to {BankingRules.MaxMonthlyFee:0}:",
                        fee => ContinueAfter(player, EditorTitle, _bankEditorService.SetMonthlyFee(bank, fee), reopenActions, reopenActions), reopenActions);
                else if (response.ItemIndex == factionRow)
                    _factionDialogService.OpenFactionPicker(player, "Run by", "Nobody", faction => { _bankEditorService.SetRunByFaction(bank, faction); reopenActions(); }, reopenActions);
                else if (response.ItemIndex == branchesRow)
                    OpenBranches(player, bank, reopenActions);
                else if (response.ItemIndex == tellersRow)
                    OpenTellers(player, bank, reopenActions);
                else if (response.ItemIndex == propertyAccountRow)
                    PickProperty(player, "Account for a property", property => OpenPropertyAccountName(player, bank, property, reopenActions), reopenActions);
                else
                    reopenActions();
            });
        }

        /// <summary>
        /// The name, the 4-letter code, then the jurisdiction, whose tag and currency the accounts get.
        /// </summary>
        private void OpenCreateBank(Player player, Action onBack)
        {
            OpenTextInput(player, EditorTitle, "Create", "Enter the name of the bank, for example Palomino Bank:", name =>
                OpenTextInput(player, EditorTitle, "Create", "Enter its 4-letter code, for example PALO. It's part of every OBAN of the bank and can't change later:", code =>
                    _jurisdictionDialogService.OpenJurisdictionPicker(player, "Jurisdiction of the bank", "Cancel", jurisdiction =>
                    {
                        if (jurisdiction == null)
                        {
                            onBack();
                            return;
                        }
                        (BankModel? bank, string? error) = _bankEditorService.CreateBank(name, code, jurisdiction);
                        ContinueAfter(player, EditorTitle, error, () => OpenBankActions(player, bank!.Id, onBack), onBack);
                    }, onBack), onBack), onBack);
        }

        private void OpenBranches(Player player, BankModel bank, Action onBack)
        {
            BetterListDialog branchesDialog = new BetterListDialog("Choose", "Back");
            branchesDialog.SetTitle(TitleType.Children, EditorTitle, bank.Name, "Branches");
            int addRow = branchesDialog.AddRow($"{ChatColor.CornflowerBlue}Add a branch");
            Dictionary<int, BankBranchModel> branchesByRow = new Dictionary<int, BankBranchModel>();
            foreach (BankBranchModel branch in _bankService.GetBranches(bank.Id))
            {
                branchesByRow[branchesDialog.AddRow(_propertyEditorService.GetProperty(branch.PropertyId)?.GetName() ?? $"Property #{branch.PropertyId}")] = branch;
            }

            _dialogService.ShowIgnoringDisconnect(player, branchesDialog, response =>
            {
                Action reopenBranches = () => OpenBranches(player, bank, onBack);
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (response.ItemIndex == addRow)
                    PickProperty(player, "Add a branch", property => AddBranch(player, bank, property, reopenBranches), reopenBranches);
                else if (branchesByRow.TryGetValue(response.ItemIndex, out BankBranchModel? branch))
                    ShowConfirmation(player, EditorTitle, "Remove branch", "Remove this branch? Tellers inside it stop serving.",
                        () => { _bankEditorService.RemoveBranch(branch.Id); reopenBranches(); }, reopenBranches);
            });
        }

        /// <summary>
        /// Adds the branch, and warns when the property lies outside the bank's jurisdiction.
        /// </summary>
        private void AddBranch(Player player, BankModel bank, Property property, Action onBack)
        {
            string? error = _bankEditorService.AddBranch(bank, property);
            if (error != null || IsInJurisdiction(property, bank.JurisdictionId))
            {
                ContinueAfter(player, EditorTitle, error, onBack, onBack);
                return;
            }
            ShowMessage(player, EditorTitle, "Branch added", $"{property.GetName()} isn't in the bank's jurisdiction. It works, but you may want to check it.", onBack);
        }

        private bool IsInJurisdiction(Property property, ulong jurisdictionId)
        {
            ulong? propertyJurisdictionId = property.GetJurisdictionId();
            Jurisdiction? jurisdiction = propertyJurisdictionId.HasValue ? _jurisdictionService.GetJurisdiction(propertyJurisdictionId.Value) : null;
            return jurisdiction != null
                && (jurisdiction.GetId() == jurisdictionId || _jurisdictionService.GetAncestors(jurisdiction).Any(ancestor => ancestor.GetId() == jurisdictionId));
        }

        private void OpenTellers(Player player, BankModel bank, Action onBack)
        {
            BetterListDialog tellersDialog = new BetterListDialog("Choose", "Back");
            tellersDialog.SetTitle(TitleType.Children, EditorTitle, bank.Name, "Tellers");
            ServerActor? nearbyActor = _actorEditorService.GetNearbyActor(player);
            int? nearbyRow = nearbyActor == null ? null : tellersDialog.AddRow($"{ChatColor.CornflowerBlue}Add the actor nearest to me: {ChatColor.White}{nearbyActor.GetName()}");
            int pickRow = tellersDialog.AddRow($"{ChatColor.CornflowerBlue}Add an actor from the list");
            Dictionary<int, BankTellerModel> tellersByRow = new Dictionary<int, BankTellerModel>();
            foreach (BankTellerModel teller in _bankService.GetTellers(bank.Id))
            {
                tellersByRow[tellersDialog.AddRow(GetActorName(teller.ActorId))] = teller;
            }

            _dialogService.ShowIgnoringDisconnect(player, tellersDialog, response =>
            {
                Action reopenTellers = () => OpenTellers(player, bank, onBack);
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (response.ItemIndex == nearbyRow && nearbyActor != null)
                    ContinueAfter(player, EditorTitle, _bankEditorService.AddTeller(bank, nearbyActor), reopenTellers, reopenTellers);
                else if (response.ItemIndex == pickRow)
                    PickActor(player, actor => ContinueAfter(player, EditorTitle, _bankEditorService.AddTeller(bank, actor), reopenTellers, reopenTellers), reopenTellers);
                else if (tellersByRow.TryGetValue(response.ItemIndex, out BankTellerModel? teller))
                    ShowConfirmation(player, EditorTitle, "Remove teller", $"Stop {GetActorName(teller.ActorId)} working for {bank.Name}?",
                        () => { _bankEditorService.RemoveTeller(teller.Id); reopenTellers(); }, reopenTellers);
            });
        }

        private void PickProperty(Player player, string title, Action<Property> onPicked, Action onBack)
        {
            BetterListDialog propertiesDialog = new BetterListDialog("Choose", "Back");
            propertiesDialog.SetTitle(TitleType.Children, EditorTitle, title);
            Dictionary<int, Property> propertiesByRow = new Dictionary<int, Property>();
            foreach (Property property in _propertyEditorService.GetProperties().OrderBy(property => property.GetName(), StringComparer.OrdinalIgnoreCase))
            {
                propertiesByRow[propertiesDialog.AddRow($"{property.GetName()} {ChatColor.LightGray}(#{property.GetId()})")] = property;
            }

            _dialogService.ShowIgnoringDisconnect(player, propertiesDialog, response =>
            {
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (propertiesByRow.TryGetValue(response.ItemIndex, out Property? property))
                    onPicked(property);
            });
        }

        private void PickActor(Player player, Action<ServerActor> onPicked, Action onBack)
        {
            BetterListDialog actorsDialog = new BetterListDialog("Choose", "Back");
            actorsDialog.SetTitle(TitleType.Children, EditorTitle, "Pick an actor");
            Dictionary<int, ServerActor> actorsByRow = new Dictionary<int, ServerActor>();
            foreach (ServerActor actor in _actorEditorService.GetEditableActors())
            {
                actorsByRow[actorsDialog.AddRow($"{actor.GetName()} {ChatColor.LightGray}(#{actor.GetId()})")] = actor;
            }

            _dialogService.ShowIgnoringDisconnect(player, actorsDialog, response =>
            {
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (actorsByRow.TryGetValue(response.ItemIndex, out ServerActor? actor))
                    onPicked(actor);
            });
        }

        private void OpenPropertyAccountName(Player player, BankModel bank, Property property, Action onBack)
        {
            OpenTextInput(player, EditorTitle, property.GetName(), $"Name the account of {Highlight(property.GetName())}, for example Rent:", name =>
            {
                (BankAccountModel? account, string? error) = _bankEditorService.OpenPropertyAccount(bank, property, name);
                ContinueAfter(player, EditorTitle, error, () => ShowMessage(player, EditorTitle, "Opened",
                    $"The account is open. Its OBAN is {Highlight(Oban.Format(account!.Oban))}. Give people access to it from a teller, or ask them to be added.", onBack), onBack);
            }, onBack);
        }

        private void OpenFindOban(Player player, Action onBack)
        {
            OpenTextInput(player, EditorTitle, "Look up an OBAN", "Enter the OBAN:", text =>
            {
                string? oban = Oban.Parse(text);
                BankAccountModel? account = oban == null ? null : _bankAccountService.GetAccountByOban(oban);
                if (account == null)
                {
                    ShowMessage(player, EditorTitle, "Look up an OBAN", oban == null ? "That isn't a valid OBAN." : "No account has that OBAN.", onBack);
                    return;
                }
                ShowMessage(player, EditorTitle, Oban.Format(account.Oban), GetAccountDetails(account), onBack);
            }, onBack);
        }

        private string GetAccountDetails(BankAccountModel account)
        {
            string status = account.ClosedAt == null ? "open" : $"closed on {account.ClosedAt.Value.ToLocalTime():dd MMM yyyy}";
            return $"Name: {account.Name}\nOwner: {_bankAccountService.GetOwnerName(account)} ({account.OwnerType})\n"
                + $"Bank: {_bankService.GetBank(account.BankId)?.Name ?? "unknown"}\nBalance: {FormatMoney(account.CurrencyId, account.Balance)}\nStatus: {status}";
        }

        private string GetFactionName(ulong? factionId)
        {
            return factionId.HasValue ? _factionService.GetFaction(factionId.Value)?.GetName() ?? $"Faction #{factionId}" : "nobody";
        }

        private string GetActorName(ulong actorId)
        {
            return _actorEditorService.GetEditableActors().FirstOrDefault(actor => actor.GetId() == actorId)?.GetName() ?? $"Actor #{actorId}";
        }
    }
}
