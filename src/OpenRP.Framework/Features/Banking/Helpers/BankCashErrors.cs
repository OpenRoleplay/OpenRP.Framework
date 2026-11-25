using OpenRP.Framework.Features.Currencies.Enums;

namespace OpenRP.Framework.Features.Banking.Helpers
{
    /// <summary>
    /// What to tell a player when cash couldn't be taken from or put into their wallet, at a teller or an ATM.
    /// </summary>
    internal static class BankCashErrors
    {
        public static string Get(CurrencyChangeResult result, string currencyName, bool isTaking)
        {
            return result switch
            {
                CurrencyChangeResult.NoWallet => "You have no wallet to hold the cash.",
                CurrencyChangeResult.NotEnoughFunds => $"You don't have that much cash in {currencyName}.",
                CurrencyChangeResult.NotEnoughSpace => "The cash doesn't fit in your wallet.",
                CurrencyChangeResult.AmountNotPayableInBills => $"That amount can't be paid in {currencyName} bills and coins.",
                _ => isTaking ? "The cash couldn't be taken." : "The cash couldn't be handed over."
            };
        }
    }
}
