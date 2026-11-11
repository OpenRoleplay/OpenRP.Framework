using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Extensions;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Factions.Services.Dialogs
{
    public partial class FactionDialogService
    {
        public void OpenFactionPicker(Player player, string title, string? noneLabel, Action<Faction?> onPicked, Action onBack)
        {
            BetterListDialog factionDialog = new BetterListDialog("Choose", "Back");
            factionDialog.SetTitle(TitleType.Children, FactionsTitle, title);
            int? noneRow = noneLabel == null ? null : factionDialog.AddRow($"{ChatColor.CornflowerBlue}{noneLabel}");
            Dictionary<int, Faction> factionsByRow = new Dictionary<int, Faction>();
            foreach (Faction faction in GetFactionsByName())
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

        private List<Faction> GetFactionsByName()
        {
            return _entityManager.GetComponents<Faction>()
                .OrderBy(faction => faction.GetName(), StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }
}
