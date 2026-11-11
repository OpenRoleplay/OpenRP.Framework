using OpenRP.Framework.Features.Characters.Services;
using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Helpers;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using OpenRP.Framework.Shared.Dialogs.Extensions;

namespace OpenRP.Framework.Features.Factions.Services.Dialogs
{
    /// <summary>
    /// All /faction menus. One file per flow: the menu, members, ranks, invitations and the faction picker for admin
    /// editors.
    /// </summary>
    public partial class FactionDialogService : IFactionDialogService
    {
        private const string FactionsTitle = "Factions";
        private const string RankIndentation = "    ";

        private readonly IDialogService _dialogService;
        private readonly IEntityManager _entityManager;
        private readonly IFactionService _factionService;
        private readonly IFactionRankService _factionRankService;
        private readonly IFactionMembershipService _factionMembershipService;
        private readonly IFactionPermissionService _factionPermissionService;
        private readonly ICharacterService _characterService;

        public FactionDialogService(IDialogService dialogService, IEntityManager entityManager, IFactionService factionService, IFactionRankService factionRankService, IFactionMembershipService factionMembershipService, IFactionPermissionService factionPermissionService, ICharacterService characterService)
        {
            _dialogService = dialogService;
            _entityManager = entityManager;
            _factionService = factionService;
            _factionRankService = factionRankService;
            _factionMembershipService = factionMembershipService;
            _factionPermissionService = factionPermissionService;
            _characterService = characterService;
        }

        /// <summary>
        /// Continues with onSuccess when there's no error; otherwise shows the error and continues with onAfterError.
        /// </summary>
        private void ContinueAfter(Player player, string? error, Action onSuccess, Action onAfterError)
        {
            if (error == null)
            {
                onSuccess();
                return;
            }

            ShowMessage(player, "Not possible", error, onAfterError);
        }

        private void ShowMessage(Player player, string title, string text, Action onClose)
        {
            BetterMessageDialog messageDialog = new BetterMessageDialog("Back");
            messageDialog.SetTitle(TitleType.Children, FactionsTitle, title);
            messageDialog.SetContent(text);
            _dialogService.ShowIgnoringDisconnect(player, messageDialog, _ => onClose());
        }

        private void ShowConfirmation(Player player, string title, string question, Action onConfirmed, Action onCancelled)
        {
            BetterMessageDialog confirmationDialog = new BetterMessageDialog(DialogHelper.Yes, DialogHelper.No);
            confirmationDialog.SetTitle(TitleType.Children, FactionsTitle, title);
            confirmationDialog.SetContent(question);
            _dialogService.ShowIgnoringDisconnect(player, confirmationDialog, response =>
            {
                if (response.Response == DialogResponse.LeftButton)
                {
                    onConfirmed();
                    return;
                }
                onCancelled();
            });
        }

        private void OpenNameInput(Player player, string title, string prompt, Action<string> onEntered, Action onBack)
        {
            BetterInputDialog inputDialog = new BetterInputDialog("Continue", "Back");
            inputDialog.SetTitle(TitleType.Children, FactionsTitle, title);
            inputDialog.SetContent(prompt);
            _dialogService.ShowIgnoringDisconnect(player, inputDialog, response =>
            {
                if (response.Response != DialogResponse.LeftButton)
                {
                    onBack();
                    return;
                }
                onEntered(response.InputText);
            });
        }

        /// <summary>
        /// Lets the player pick one of these ranks, shown as an indented tree.
        /// </summary>
        private void OpenRankPicker(Player player, string title, List<(FactionRank Rank, int Depth)> ranks, Action<FactionRank> onPicked, Action onBack)
        {
            if (ranks.Count == 0)
            {
                ShowMessage(player, title, "There are no ranks you can choose here.", onBack);
                return;
            }

            BetterListDialog rankDialog = new BetterListDialog("Choose", "Back");
            rankDialog.SetTitle(TitleType.Children, FactionsTitle, title);
            Dictionary<int, FactionRank> ranksByRow = new Dictionary<int, FactionRank>();
            foreach ((FactionRank rank, int depth) in ranks)
            {
                ranksByRow[rankDialog.AddRow(GetRankLabel(rank, depth))] = rank;
            }

            _dialogService.ShowIgnoringDisconnect(player, rankDialog, response =>
            {
                if (response.Response != DialogResponse.LeftButton || !ranksByRow.TryGetValue(response.ItemIndex, out FactionRank? rank))
                {
                    onBack();
                    return;
                }
                onPicked(rank);
            });
        }

        private static string GetRankLabel(FactionRank rank, int depth)
        {
            string indentation = string.Concat(System.Linq.Enumerable.Repeat(RankIndentation, depth));
            return rank.IsTopRank() ? $"{indentation}{ChatColor.Highlight}{rank.GetName()}" : $"{indentation}{rank.GetName()}";
        }
    }
}
