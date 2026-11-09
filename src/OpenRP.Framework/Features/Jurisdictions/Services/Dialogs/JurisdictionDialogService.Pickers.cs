using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Factions.Components;
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
        public void OpenJurisdictionPicker(Player player, string title, string noneLabel, Action<Jurisdiction?> onPicked, Action onBack)
        {
            BetterListDialog pickerDialog = new BetterListDialog("Choose", "Back");
            pickerDialog.SetTitle(TitleType.Children, JurisdictionsTitle, title);
            int noneRow = pickerDialog.AddRow($"{ChatColor.CornflowerBlue}{noneLabel}");
            Dictionary<int, Jurisdiction> jurisdictionsByRow = new Dictionary<int, Jurisdiction>();
            foreach (JurisdictionTreeRow treeRow in _jurisdictionService.GetJurisdictionTree())
            {
                jurisdictionsByRow[pickerDialog.AddRow(GetTreeLabel(treeRow.Jurisdiction, treeRow.Depth))] = treeRow.Jurisdiction;
            }

            void OnPicked(ListDialogResponse response)
            {
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (response.ItemIndex == noneRow)
                    onPicked(null);
                else if (jurisdictionsByRow.TryGetValue(response.ItemIndex, out Jurisdiction? jurisdiction))
                    onPicked(jurisdiction);
            }

            _dialogService.ShowIgnoringDisconnect(player, pickerDialog, OnPicked);
        }

        private void OpenMoveJurisdiction(Player player, Jurisdiction jurisdiction, Action onBack)
        {
            void Move(Jurisdiction? parent)
            {
                string? error = _jurisdictionEditorService.MoveJurisdiction(jurisdiction, parent);
                ContinueAfter(player, error, onBack, () => OpenMoveJurisdiction(player, jurisdiction, onBack));
            }

            OpenJurisdictionPicker(player, $"Move {jurisdiction.GetName()}", "Nothing, make it a country", Move, onBack);
        }

        private void OpenSetCurrency(Player player, Jurisdiction jurisdiction, Action onBack)
        {
            BetterListDialog currencyDialog = new BetterListDialog("Choose", "Back");
            currencyDialog.SetTitle(TitleType.Children, JurisdictionsTitle, "Currency");
            int noneRow = currencyDialog.AddRow($"{ChatColor.CornflowerBlue}None, use the parent's currency");
            Dictionary<int, CurrencyModel> currenciesByRow = new Dictionary<int, CurrencyModel>();
            foreach (CurrencyModel currency in _currencyService.GetCurrencies().OrderBy(currency => currency.Name, StringComparer.OrdinalIgnoreCase))
            {
                currenciesByRow[currencyDialog.AddRow($"{currency.Name} {ChatColor.LightGray}({currency.CurrencyCode})")] = currency;
            }

            void OnCurrencyChosen(ListDialogResponse response)
            {
                if (response.Response == DialogResponse.LeftButton && response.ItemIndex == noneRow)
                    _jurisdictionEditorService.SetCurrency(jurisdiction, null);
                else if (response.Response == DialogResponse.LeftButton && currenciesByRow.TryGetValue(response.ItemIndex, out CurrencyModel? currency))
                    _jurisdictionEditorService.SetCurrency(jurisdiction, currency.Id);
                onBack();
            }

            _dialogService.ShowIgnoringDisconnect(player, currencyDialog, OnCurrencyChosen);
        }

        private void OpenSetGovernment(Player player, Jurisdiction jurisdiction, Action onBack)
        {
            void SetGovernment(Faction? faction)
            {
                _jurisdictionEditorService.SetGovernment(jurisdiction, faction);
                onBack();
            }

            OpenFactionPicker(player, "Government", "None, use the parent's government", SetGovernment, onBack);
        }

        /// <summary>
        /// Lets the player pick a faction, or none (null) through the first row when noneLabel isn't null.
        /// </summary>
        private void OpenFactionPicker(Player player, string title, string? noneLabel, Action<Faction?> onPicked, Action onBack)
        {
            BetterListDialog factionDialog = new BetterListDialog("Choose", "Back");
            factionDialog.SetTitle(TitleType.Children, JurisdictionsTitle, title);
            int? noneRow = noneLabel == null ? null : factionDialog.AddRow($"{ChatColor.CornflowerBlue}{noneLabel}");
            Dictionary<int, Faction> factionsByRow = new Dictionary<int, Faction>();
            foreach (Faction faction in GetFactions())
            {
                factionsByRow[factionDialog.AddRow($"{faction.GetName()} {ChatColor.LightGray}(#{faction.GetId()})")] = faction;
            }

            void OnFactionChosen(ListDialogResponse response)
            {
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (response.ItemIndex == noneRow)
                    onPicked(null);
                else if (factionsByRow.TryGetValue(response.ItemIndex, out Faction? faction))
                    onPicked(faction);
            }

            _dialogService.ShowIgnoringDisconnect(player, factionDialog, OnFactionChosen);
        }

        private List<Faction> GetFactions()
        {
            return _entityManager.GetComponents<Faction>()
                .OrderBy(faction => faction.GetName(), StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }
}
