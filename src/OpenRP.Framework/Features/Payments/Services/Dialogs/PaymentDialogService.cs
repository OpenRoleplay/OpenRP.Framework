using OpenRP.Framework.Features.Currencies.Services;
using OpenRP.Framework.Features.Payments.Entities;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Extensions;
using SampSharp.Entities.SAMP;
using System;

namespace OpenRP.Framework.Features.Payments.Services.Dialogs
{
    public class PaymentDialogService : IPaymentDialogService
    {
        private readonly IDialogService _dialogService;
        private readonly ICurrencyService _currencyService;

        public PaymentDialogService(IDialogService dialogService, ICurrencyService currencyService)
        {
            _dialogService = dialogService;
            _currencyService = currencyService;
        }

        public void OpenPaymentMethodChoice(Player player, PaymentRequest request, Action onCash, Action onCard, Action onCancelled)
        {
            string amount = _currencyService.GetCurrencyFormat(request.CurrencyId, request.Amount);
            BetterListDialog methodDialog = new BetterListDialog("Pay", "Cancel");
            methodDialog.SetTitle(TitleType.Children, "Pay", $"{ChatColor.Highlight}{amount}{ChatColor.White} for {request.Description}");
            int? cashRow = request.AllowsCash ? methodDialog.AddRow("Cash") : null;
            int? cardRow = request.AllowsCard ? methodDialog.AddRow("Debit card") : null;

            _dialogService.ShowIgnoringDisconnect(player, methodDialog, response =>
            {
                if (response.Response != DialogResponse.LeftButton)
                    onCancelled();
                else if (response.ItemIndex == cashRow)
                    onCash();
                else if (response.ItemIndex == cardRow)
                    onCard();
                else
                    onCancelled();
            });
        }
    }
}
