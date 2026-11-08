using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Currencies.Enums;
using OpenRP.Framework.Features.Currencies.Services;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Admins.Commands
{
    public class GiveMoneyCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Give money of a currency to a player. Use a negative amount to take money.",
            CommandGroups = new string[] { "Admin" })]
        public async void GiveMoney(Player player, ICurrencyService currencyService, Player receiver, string currency_code, int amount)
        {
            CurrencyModel? currency = currencyService.GetCurrencyByCode(currency_code);
            if (currency == null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, $"'{currency_code}' is not a currency code.");
                currencyService.SendCurrencyCodes(player);
                return;
            }

            if (!receiver.IsPlayerPlayingAsCharacter())
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "That player is not playing as a character.");
                return;
            }

            Character character = receiver.GetPlayerCurrentlyPlayingAsCharacter();
            SendCurrencyAmountMessage(player, currencyService, character, currency.Id, "before the update");

            CurrencyChangeResult result = await ChangeCharacterCurrency(currencyService, character, currency.Id, amount);

            // Either player may have left during the update; a destroyed component throws when used.
            if (!player.IsComponentAlive || !character.IsComponentAlive)
            {
                return;
            }

            SendCurrencyChangeResultMessage(player, currencyService, character, currency, amount, result);

            SendCurrencyAmountMessage(player, currencyService, character, currency.Id, "after the update");
        }

        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Give money of a currency to a player. Use a negative amount to take money.",
            CommandGroups = new string[] { "Admin" })]
        public void GiveMoney(Player player, ICurrencyService currencyService)
        {
            player.SendPlayerInfoMessage(PlayerInfoMessageType.SYNTAX, "/givemoney [playerid] [currency code] [amount]");
            currencyService.SendCurrencyCodes(player);
        }

        private static Task<CurrencyChangeResult> ChangeCharacterCurrency(ICurrencyService currencyService, Character character, ulong currencyId, int amount)
        {
            return amount >= 0
                ? currencyService.GiveCharacterCurrency(character, currencyId, amount)
                : currencyService.TakeCharacterCurrency(character, currencyId, Math.Abs(amount));
        }

        private static void SendCurrencyAmountMessage(Player player, ICurrencyService currencyService, Character character, ulong currencyId, string moment)
        {
            decimal amount = currencyService.GetCharacterCurrencyAmount(character, currencyId);
            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"{character.GetName()} had {currencyService.GetCurrencyFormat(currencyId, amount)} {moment}.");
        }

        private static void SendCurrencyChangeResultMessage(Player player, ICurrencyService currencyService, Character character, CurrencyModel currency, int amount, CurrencyChangeResult result)
        {
            string formattedAmount = currencyService.GetCurrencyFormat(currency.Id, Math.Abs(amount));
            switch (result)
            {
                case CurrencyChangeResult.Success:
                    string change = amount >= 0 ? $"Gave {formattedAmount} to" : $"Took {formattedAmount} from";
                    player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"{change} {character.GetName()}.");
                    break;
                case CurrencyChangeResult.NotEnoughFunds:
                    player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, $"{character.GetName()} does not have {formattedAmount}!");
                    break;
                case CurrencyChangeResult.NotEnoughSpace:
                    player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, $"{character.GetName()} does not have enough space in their wallet or inventory!");
                    break;
                case CurrencyChangeResult.NoWallet:
                    player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, $"{character.GetName()}'s inventory is not loaded yet.");
                    break;
                case CurrencyChangeResult.AmountNotPayableInBills:
                    player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, $"{currency.Name} has no bills that add up to {formattedAmount}.");
                    break;
            }
        }
    }
}
