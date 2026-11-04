using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Currencies.Services;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Currencies.Commands
{
    public class PayCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Pay money to another player.",
            CommandGroups = new string[] { "Currencies" })]
        public void Pay(Player player, Player playerid, IPaymentService paymentService, string currency_code, int amount)
        {
            paymentService.RequestPayment(player, playerid, currency_code, amount);
        }

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Pay money to another player.",
            CommandGroups = new string[] { "Currencies" })]
        public void Pay(Player player, ICurrencyService currencyService)
        {
            player.SendPlayerInfoMessage(PlayerInfoMessageType.SYNTAX, "/pay [playerid] [currency code] [amount]");
            currencyService.SendCurrencyCodes(player);
        }
    }
}
