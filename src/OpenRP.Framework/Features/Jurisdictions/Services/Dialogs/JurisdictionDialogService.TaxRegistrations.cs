using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Features.Jurisdictions.Components;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Jurisdictions.Services.Dialogs
{
    public partial class JurisdictionDialogService
    {
        private void OpenTaxRegistrations(Player player, Jurisdiction jurisdiction, Action onBack)
        {
            BetterListDialog registrationsDialog = new BetterListDialog("Choose", "Back");
            registrationsDialog.SetTitle(TitleType.Children, JurisdictionsTitle, jurisdiction.GetName(), "Tax registrations");
            int addRow = registrationsDialog.AddRow($"{ChatColor.CornflowerBlue}Register a faction");
            Dictionary<int, FactionTaxRegistration> registrationsByRow = new Dictionary<int, FactionTaxRegistration>();
            foreach (FactionTaxRegistration registration in _jurisdictionService.GetTaxRegistrations(jurisdiction))
            {
                registrationsByRow[registrationsDialog.AddRow(GetRegisteredFactionName(registration))] = registration;
            }

            void OnRegistrationChosen(ListDialogResponse response)
            {
                Action reopenRegistrations = () => OpenTaxRegistrations(player, jurisdiction, onBack);
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (response.ItemIndex == addRow)
                    OpenAddTaxRegistration(player, jurisdiction, reopenRegistrations);
                else if (registrationsByRow.TryGetValue(response.ItemIndex, out FactionTaxRegistration? registration))
                    OpenRemoveTaxRegistration(player, jurisdiction, registration, reopenRegistrations);
            }

            _dialogService.Show(player, registrationsDialog, OnRegistrationChosen);
        }

        private void OpenAddTaxRegistration(Player player, Jurisdiction jurisdiction, Action onBack)
        {
            void Register(Faction? faction)
            {
                string? error = _jurisdictionEditorService.AddTaxRegistration(jurisdiction, faction!);
                ContinueAfter(player, error, onBack, onBack);
            }

            OpenFactionPicker(player, "Register a faction", null, Register, onBack);
        }

        private void OpenRemoveTaxRegistration(Player player, Jurisdiction jurisdiction, FactionTaxRegistration registration, Action onBack)
        {
            void Remove()
            {
                _jurisdictionEditorService.RemoveTaxRegistration(registration);
                onBack();
            }

            ShowConfirmation(player, "Remove registration", $"Remove the registration of {ChatColor.Highlight}{GetRegisteredFactionName(registration)}{ChatColor.White} in {jurisdiction.GetName()}? It won't pay taxes there anymore.", Remove, onBack);
        }

        private string GetRegisteredFactionName(FactionTaxRegistration registration)
        {
            Faction? faction = _factionService.GetFaction(registration.GetFactionId());
            return faction?.GetName() ?? $"Faction #{registration.GetFactionId()}";
        }
    }
}
