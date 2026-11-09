using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Jurisdictions.Components;
using OpenRP.Framework.Features.Jurisdictions.Entities;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenRP.Framework.Shared.Dialogs.Extensions;

namespace OpenRP.Framework.Features.Jurisdictions.Services.Dialogs
{
    public partial class JurisdictionDialogService
    {
        public void OpenJurisdictionEditor(Player player)
        {
            BetterListDialog jurisdictionsDialog = new BetterListDialog("Choose", "Close");
            jurisdictionsDialog.SetTitle(TitleType.Children, JurisdictionsTitle);
            int createRow = jurisdictionsDialog.AddRow($"{ChatColor.CornflowerBlue}Create a country");
            Dictionary<int, Jurisdiction> jurisdictionsByRow = new Dictionary<int, Jurisdiction>();
            foreach (JurisdictionTreeRow treeRow in _jurisdictionService.GetJurisdictionTree())
            {
                jurisdictionsByRow[jurisdictionsDialog.AddRow(GetTreeLabel(treeRow.Jurisdiction, treeRow.Depth))] = treeRow.Jurisdiction;
            }

            void OnJurisdictionChosen(ListDialogResponse response)
            {
                Action reopenEditor = () => OpenJurisdictionEditor(player);
                if (response.Response != DialogResponse.LeftButton)
                    return;
                if (response.ItemIndex == createRow)
                    OpenCreateJurisdiction(player, null, reopenEditor);
                else if (jurisdictionsByRow.TryGetValue(response.ItemIndex, out Jurisdiction? jurisdiction))
                    OpenJurisdictionActions(player, jurisdiction, reopenEditor);
            }

            _dialogService.ShowIgnoringDisconnect(player, jurisdictionsDialog, OnJurisdictionChosen);
        }

        private void OpenJurisdictionActions(Player player, Jurisdiction jurisdiction, Action onBack)
        {
            if (jurisdiction.IsDeleted())
            {
                ShowMessage(player, jurisdiction.GetName(), "This jurisdiction has been deleted.", onBack);
                return;
            }

            BetterListDialog actionsDialog = new BetterListDialog("Choose", "Back");
            actionsDialog.SetTitle(TitleType.Children, JurisdictionsTitle, jurisdiction.GetName());
            int renameRow = actionsDialog.AddRow("Rename");
            int tagsRow = actionsDialog.AddRow($"Tags: {ChatColor.Highlight}{jurisdiction.GetTags()}");
            int currencyRow = actionsDialog.AddRow($"Currency: {ChatColor.Highlight}{GetCurrencyLabel(jurisdiction)}");
            int parentRow = actionsDialog.AddRow($"Lies in: {ChatColor.Highlight}{GetParentLabel(jurisdiction)}");
            int governmentRow = actionsDialog.AddRow($"Governed by: {ChatColor.Highlight}{GetGovernmentLabel(jurisdiction)}");
            int taxRegistrationsRow = actionsDialog.AddRow($"Tax registrations ({_jurisdictionService.GetTaxRegistrations(jurisdiction).Count})");
            int createChildRow = actionsDialog.AddRow("Create a sub-jurisdiction");
            int deleteRow = actionsDialog.AddRow("Delete");

            void OnActionChosen(ListDialogResponse response)
            {
                Action reopenActions = () => OpenJurisdictionActions(player, jurisdiction, onBack);
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (response.ItemIndex == renameRow)
                    OpenRenameJurisdiction(player, jurisdiction, reopenActions);
                else if (response.ItemIndex == tagsRow)
                    OpenSetTags(player, jurisdiction, reopenActions);
                else if (response.ItemIndex == currencyRow)
                    OpenSetCurrency(player, jurisdiction, reopenActions);
                else if (response.ItemIndex == parentRow)
                    OpenMoveJurisdiction(player, jurisdiction, reopenActions);
                else if (response.ItemIndex == governmentRow)
                    OpenSetGovernment(player, jurisdiction, reopenActions);
                else if (response.ItemIndex == taxRegistrationsRow)
                    OpenTaxRegistrations(player, jurisdiction, reopenActions);
                else if (response.ItemIndex == createChildRow)
                    OpenCreateJurisdiction(player, jurisdiction, reopenActions);
                else if (response.ItemIndex == deleteRow)
                    OpenDeleteJurisdiction(player, jurisdiction, onBack, reopenActions);
            }

            _dialogService.ShowIgnoringDisconnect(player, actionsDialog, OnActionChosen);
        }

        /// <summary>
        /// Asks for the name, then the tags. Creates a country when the parent is null.
        /// </summary>
        private void OpenCreateJurisdiction(Player player, Jurisdiction? parent, Action onBack)
        {
            string where = parent == null ? "the new country" : $"the new jurisdiction in {ChatColor.Highlight}{parent.GetName()}{ChatColor.White}";
            OpenTextInput(player, "Create", $"Enter the name of {where}:", name => OpenCreateJurisdictionTags(player, name, parent, onBack), onBack);
        }

        private void OpenCreateJurisdictionTags(Player player, string name, Jurisdiction? parent, Action onBack)
        {
            void Create(string tags)
            {
                (Jurisdiction? jurisdiction, string? error) = _jurisdictionEditorService.CreateJurisdiction(name, tags, parent);
                ContinueAfter(player, error, () => OpenJurisdictionActions(player, jurisdiction!, onBack), () => OpenCreateJurisdictionTags(player, name, parent, onBack));
            }

            OpenTextInput(player, "Create", GetTagsPrompt(name), Create, onBack);
        }

        private void OpenRenameJurisdiction(Player player, Jurisdiction jurisdiction, Action onBack)
        {
            void Rename(string name)
            {
                string? error = _jurisdictionEditorService.RenameJurisdiction(jurisdiction, name);
                ContinueAfter(player, error, onBack, () => OpenRenameJurisdiction(player, jurisdiction, onBack));
            }

            OpenTextInput(player, $"Rename {jurisdiction.GetName()}", $"Enter the new name for {ChatColor.Highlight}{jurisdiction.GetName()}{ChatColor.White}:", Rename, onBack);
        }

        private void OpenSetTags(Player player, Jurisdiction jurisdiction, Action onBack)
        {
            void SetTags(string tags)
            {
                string? error = _jurisdictionEditorService.SetTags(jurisdiction, tags);
                ContinueAfter(player, error, onBack, () => OpenSetTags(player, jurisdiction, onBack));
            }

            OpenTextInput(player, "Tags", GetTagsPrompt(jurisdiction.GetName()), SetTags, onBack);
        }

        private void OpenDeleteJurisdiction(Player player, Jurisdiction jurisdiction, Action onDeleted, Action onBack)
        {
            void Delete()
            {
                string? error = _jurisdictionEditorService.DeleteJurisdiction(jurisdiction);
                ContinueAfter(player, error, onDeleted, onBack);
            }

            ShowConfirmation(player, "Delete jurisdiction", $"Are you sure you want to delete {ChatColor.Highlight}{jurisdiction.GetName()}{ChatColor.White}? Properties and vehicles in it will have no jurisdiction.", Delete, onBack);
        }

        private static string GetTagsPrompt(string name)
        {
            return $"Enter the 2-, 3- and 4-letter tags of {ChatColor.Highlight}{name}{ChatColor.White}, separated by spaces, for example SA SAN USSA. A country's 2-letter tag is used on licence plates.";
        }

        private string GetCurrencyLabel(Jurisdiction jurisdiction)
        {
            ulong? currencyId = _jurisdictionService.GetCurrencyId(jurisdiction);
            CurrencyModel? currency = _currencyService.GetCurrencies().FirstOrDefault(currency => currency.Id == currencyId);
            if (currency == null)
                return "None";
            return jurisdiction.GetCurrencyId().HasValue ? currency.Name : $"{currency.Name} {ChatColor.LightGray}(from a parent)";
        }

        private string GetParentLabel(Jurisdiction jurisdiction)
        {
            Jurisdiction? parent = _jurisdictionService.GetParentJurisdiction(jurisdiction);
            return parent == null ? "Nothing, it's a country" : parent.GetName();
        }

        /// <summary>
        /// The government, with where it comes from when it's inherited, for example "Federal Government (from United
        /// States of San Andreas)".
        /// </summary>
        private string GetGovernmentLabel(Jurisdiction jurisdiction)
        {
            JurisdictionGovernment? government = _jurisdictionService.GetGovernment(jurisdiction);
            if (government == null)
                return "None";
            if (government.GovernedFrom.GetId() == jurisdiction.GetId())
                return government.Faction.GetName();
            return $"{government.Faction.GetName()} {ChatColor.LightGray}(from {government.GovernedFrom.GetName()})";
        }
    }
}
