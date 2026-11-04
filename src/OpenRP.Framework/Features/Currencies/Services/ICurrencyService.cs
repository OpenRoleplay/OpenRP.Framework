using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Currencies.Entities;
using OpenRP.Framework.Features.Currencies.Enums;
using OpenRP.Framework.Features.Inventories.Components;
using SampSharp.Entities.SAMP;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Currencies.Services
{
    public interface ICurrencyService
    {
        /// <summary>
        /// All currencies, for example to list their codes.
        /// </summary>
        List<CurrencyModel> GetCurrencies();

        /// <summary>
        /// The currency with this code (case-insensitive), or null when it doesn't exist.
        /// </summary>
        CurrencyModel? GetCurrencyByCode(string currencyCode);

        /// <summary>
        /// Lists every currency code with its name to the player, for command syntax help.
        /// </summary>
        void SendCurrencyCodes(Player player);

        /// <summary>
        /// The amount written as money in this currency, for example "$12.50".
        /// </summary>
        string GetCurrencyFormat(ulong currencyId, decimal amount);

        /// <summary>
        /// How much money of this currency the character has in their wallet.
        /// </summary>
        decimal GetCharacterCurrencyAmount(Character character, ulong currencyId);

        /// <summary>
        /// Puts this amount into the character's wallet, in as few bills as possible.
        /// </summary>
        Task<CurrencyChangeResult> GiveCharacterCurrency(Character character, ulong currencyId, decimal amount);

        /// <summary>
        /// Takes this amount from the character's wallet. When the bills don't add up exactly, a larger bill is
        /// broken and the change goes back into the wallet.
        /// </summary>
        Task<CurrencyChangeResult> TakeCharacterCurrency(Character character, ulong currencyId, decimal amount);

        /// <summary>
        /// The bills in the wallet that add up to exactly this amount, or null when no combination does.
        /// </summary>
        List<BillStack>? GetExactBillsFromWallet(Inventory wallet, ulong currencyId, decimal amount);

        /// <summary>
        /// Moves the bills from one wallet to another. Moves nothing and returns false when they aren't there or don't fit.
        /// </summary>
        bool TransferBills(Inventory fromWallet, Inventory toWallet, BillStack bills);

        /// <summary>
        /// Shows the character's money in their default currency as the player's GTA money.
        /// </summary>
        void SetPlayerMoneyToCharacterCurrency(Player player, Character character);
    }
}
