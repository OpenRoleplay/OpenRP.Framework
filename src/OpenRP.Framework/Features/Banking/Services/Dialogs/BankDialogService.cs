using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Actors.Components;
using OpenRP.Framework.Features.Actors.Services;
using OpenRP.Framework.Features.Banking.Enums;
using OpenRP.Framework.Features.Banking.Helpers;
using OpenRP.Framework.Features.Currencies.Services;
using OpenRP.Framework.Features.Factions.Services;
using OpenRP.Framework.Features.Factions.Services.Dialogs;
using OpenRP.Framework.Features.Jurisdictions.Services;
using OpenRP.Framework.Features.Jurisdictions.Services.Dialogs;
using OpenRP.Framework.Features.Properties.Services;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Extensions;
using OpenRP.Framework.Shared.Dialogs.Helpers;
using SampSharp.Entities.SAMP;
using System;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Banking.Services.Dialogs
{
    /// <summary>
    /// The bank menus. One file per flow: the menu and account picker, one account, opening, closing and access, and
    /// the admin editor. Every step that does something checks the teller and the player's access again.
    /// </summary>
    public partial class BankDialogService : IBankDialogService
    {
        private const string EditorTitle = "Banks";

        private readonly IDialogService _dialogService;
        private readonly ITellerService _tellerService;
        private readonly IBankService _bankService;
        private readonly IBankAccountService _bankAccountService;
        private readonly IBankTransactionService _bankTransactionService;
        private readonly IBankEditorService _bankEditorService;
        private readonly ICurrencyService _currencyService;
        private readonly IJurisdictionService _jurisdictionService;
        private readonly IJurisdictionDialogService _jurisdictionDialogService;
        private readonly IFactionDialogService _factionDialogService;
        private readonly IFactionService _factionService;
        private readonly IPropertyEditorService _propertyEditorService;
        private readonly IActorEditorService _actorEditorService;
        private readonly ILogger<BankDialogService> _logger;

        public BankDialogService(IDialogService dialogService, ITellerService tellerService, IBankService bankService, IBankAccountService bankAccountService,
            IBankTransactionService bankTransactionService, IBankEditorService bankEditorService, ICurrencyService currencyService, IJurisdictionService jurisdictionService,
            IJurisdictionDialogService jurisdictionDialogService, IFactionDialogService factionDialogService, IFactionService factionService,
            IPropertyEditorService propertyEditorService, IActorEditorService actorEditorService, ILogger<BankDialogService> logger)
        {
            _factionService = factionService;
            _dialogService = dialogService;
            _tellerService = tellerService;
            _bankService = bankService;
            _bankAccountService = bankAccountService;
            _bankTransactionService = bankTransactionService;
            _bankEditorService = bankEditorService;
            _currencyService = currencyService;
            _jurisdictionService = jurisdictionService;
            _jurisdictionDialogService = jurisdictionDialogService;
            _factionDialogService = factionDialogService;
            _propertyEditorService = propertyEditorService;
            _actorEditorService = actorEditorService;
            _logger = logger;
        }

        /// <summary>
        /// Runs the action when the teller can still do this for the player; otherwise tells the player why.
        /// </summary>
        private void WithTeller(Player player, ServerActor teller, BankOperation operation, Action action)
        {
            string? error = _tellerService.GetServiceError(teller, player, operation);
            if (error != null)
            {
                ShowMessage(player, "Bank", "Not possible", error, () => { });
                return;
            }
            action();
        }

        /// <summary>
        /// Fire-and-forget from a dialog, so every error is caught and logged here instead of being lost.
        /// </summary>
        private async Task RunAsync(Player player, Func<Task> work)
        {
            try
            {
                await work();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "A bank operation for {PlayerName} failed.", player.IsComponentAlive ? player.Name : "a player who left");
            }
        }

        private void ContinueAfter(Player player, string title, string? error, Action onSuccess, Action onAfterError)
        {
            if (error == null)
            {
                onSuccess();
                return;
            }
            ShowMessage(player, title, "Not possible", error, onAfterError);
        }

        private void ShowMessage(Player player, string title, string subtitle, string text, Action onClose)
        {
            BetterMessageDialog messageDialog = new BetterMessageDialog("Back");
            messageDialog.SetTitle(TitleType.Children, title, subtitle);
            messageDialog.SetContent(text);
            _dialogService.ShowIgnoringDisconnect(player, messageDialog, _ => onClose());
        }

        private void ShowConfirmation(Player player, string title, string subtitle, string question, Action onConfirmed, Action onCancelled,
            string confirmButton = DialogHelper.Yes, string cancelButton = DialogHelper.No)
        {
            BetterMessageDialog confirmationDialog = new BetterMessageDialog(confirmButton, cancelButton);
            confirmationDialog.SetTitle(TitleType.Children, title, subtitle);
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

        private void OpenTextInput(Player player, string title, string subtitle, string prompt, Action<string> onEntered, Action onBack)
        {
            BetterInputDialog inputDialog = new BetterInputDialog("Continue", "Back");
            inputDialog.SetTitle(TitleType.Children, title, subtitle);
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
        /// The amount in the account's currency, with a sign when <paramref name="withSign"/> is set: "+$500.00".
        /// </summary>
        private string FormatMoney(ulong currencyId, decimal amount, bool withSign = false)
        {
            string formatted = _currencyService.GetCurrencyFormat(currencyId, Math.Abs(amount));
            if (amount < 0)
                return $"-{formatted}";
            return withSign ? $"+{formatted}" : formatted;
        }

        private string GetAccountLabel(BankAccountModel account)
        {
            return $"{account.Name} {ChatColor.LightGray}({Oban.Format(account.Oban)}){ChatColor.White}  {FormatMoney(account.CurrencyId, account.Balance)}";
        }

        private static string Highlight(string text)
        {
            return $"{ChatColor.Highlight}{text}{ChatColor.White}";
        }
    }
}
