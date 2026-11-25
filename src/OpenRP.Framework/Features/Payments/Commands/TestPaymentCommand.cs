using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Banking.Services;
using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Currencies.Services;
using OpenRP.Framework.Features.Payments.Entities;
using OpenRP.Framework.Features.Payments.Services;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Payments.Commands
{
    /// <summary>
    /// Lets testers try the payment flow before any feature uses it.
    /// </summary>
    public class TestPaymentCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Admin", "Tester" },
            Description = "Ask yourself to pay an amount into a bank account, to test cash and card payments.",
            CommandGroups = new string[] { "Tester" })]
        public async Task TestPayment(Player player, ICheckoutService checkoutService, IBankAccountService bankAccountService, ICurrencyService currencyService,
            int amount, string oban)
        {
            BankAccountModel? account = bankAccountService.GetAccountByOban(oban.Replace(" ", string.Empty).ToUpperInvariant());
            if (account == null || account.ClosedAt != null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "No open account has that OBAN.");
                return;
            }

            PaymentResult result = await checkoutService.RequestPaymentAsync(player, new PaymentRequest(amount, account.CurrencyId, account.Id, "a test payment"));
            if (!player.IsComponentAlive)
            {
                return;
            }
            string amountText = currencyService.GetCurrencyFormat(account.CurrencyId, amount);
            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"Test payment of {amountText}: {result.Outcome}.");
        }

        [ServerCommand(PermissionGroups = new string[] { "Admin", "Tester" },
            Description = "Ask yourself to pay an amount into a bank account, to test cash and card payments.",
            CommandGroups = new string[] { "Tester" })]
        public void TestPayment(Player player)
        {
            player.SendPlayerInfoMessage(PlayerInfoMessageType.SYNTAX, "/testpayment [amount] [OBAN]");
        }
    }
}
