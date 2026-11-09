using OpenRP.Framework.Features.Currencies.Services;
using OpenRP.Framework.Features.Factions.Services;
using OpenRP.Framework.Features.Jurisdictions.Components;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Helpers;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Linq;
using OpenRP.Framework.Shared.Dialogs.Extensions;

namespace OpenRP.Framework.Features.Jurisdictions.Services.Dialogs
{
    /// <summary>
    /// The /editjurisdictions menus. One file per flow: the jurisdictions, the pickers and the tax registrations.
    /// </summary>
    public partial class JurisdictionDialogService : IJurisdictionDialogService
    {
        private const string JurisdictionsTitle = "Jurisdictions";
        private const string TreeIndentation = "    ";

        private readonly IDialogService _dialogService;
        private readonly IEntityManager _entityManager;
        private readonly IJurisdictionService _jurisdictionService;
        private readonly IJurisdictionEditorService _jurisdictionEditorService;
        private readonly ICurrencyService _currencyService;
        private readonly IFactionService _factionService;

        public JurisdictionDialogService(IDialogService dialogService, IEntityManager entityManager, IJurisdictionService jurisdictionService,
            IJurisdictionEditorService jurisdictionEditorService, ICurrencyService currencyService, IFactionService factionService)
        {
            _dialogService = dialogService;
            _entityManager = entityManager;
            _jurisdictionService = jurisdictionService;
            _jurisdictionEditorService = jurisdictionEditorService;
            _currencyService = currencyService;
            _factionService = factionService;
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
            messageDialog.SetTitle(TitleType.Children, JurisdictionsTitle, title);
            messageDialog.SetContent(text);
            _dialogService.ShowIgnoringDisconnect(player, messageDialog, _ => onClose());
        }

        private void ShowConfirmation(Player player, string title, string question, Action onConfirmed, Action onCancelled)
        {
            BetterMessageDialog confirmationDialog = new BetterMessageDialog(DialogHelper.Yes, DialogHelper.No);
            confirmationDialog.SetTitle(TitleType.Children, JurisdictionsTitle, title);
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

        private void OpenTextInput(Player player, string title, string prompt, Action<string> onEntered, Action onBack)
        {
            BetterInputDialog inputDialog = new BetterInputDialog("Continue", "Back");
            inputDialog.SetTitle(TitleType.Children, JurisdictionsTitle, title);
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

        private static string GetJurisdictionLabel(Jurisdiction jurisdiction)
        {
            return $"{jurisdiction.GetName()} {ChatColor.LightGray}({jurisdiction.GetTags()}){ChatColor.White}";
        }

        /// <summary>
        /// The jurisdiction's label, indented by how deep it lies in the tree.
        /// </summary>
        private static string GetTreeLabel(Jurisdiction jurisdiction, int depth)
        {
            string indentation = string.Concat(Enumerable.Repeat(TreeIndentation, depth));
            return depth == 0 ? GetJurisdictionLabel(jurisdiction) : $"{indentation}- {GetJurisdictionLabel(jurisdiction)}";
        }
    }
}
