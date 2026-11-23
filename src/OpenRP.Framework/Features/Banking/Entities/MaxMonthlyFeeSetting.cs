using OpenRP.Framework.Features.Banking.Helpers;
using OpenRP.Framework.Features.Currencies.Services;
using OpenRP.Framework.Features.Jurisdictions.Components;
using OpenRP.Framework.Features.Jurisdictions.Entities;
using OpenRP.Framework.Features.Jurisdictions.Services;
using System;
using System.Globalization;

namespace OpenRP.Framework.Features.Banking.Entities
{
    /// <summary>
    /// The highest monthly fee a bank in the jurisdiction may charge per account. A bank's own fee is held to it.
    /// </summary>
    public class MaxMonthlyFeeSetting : JurisdictionSettingDefinition<decimal>
    {
        private const string StoredFormat = "0.00";

        private readonly IJurisdictionService _jurisdictionService;
        private readonly ICurrencyService _currencyService;

        public MaxMonthlyFeeSetting(IJurisdictionService jurisdictionService, ICurrencyService currencyService)
        {
            _jurisdictionService = jurisdictionService;
            _currencyService = currencyService;
        }

        public override string Key => "banking.maxMonthlyFee";

        public override string Name => "Bank account fee cap";

        public override string Prompt => "Enter the highest monthly fee a bank in this jurisdiction may charge per account, from 0 to 10,000:";

        public override decimal Default => BankingRules.DefaultMaxMonthlyFee;

        protected override string? TryParse(string input, Jurisdiction jurisdiction, out decimal value)
        {
            decimal? fee = BankAmounts.ParseFee(input, BankingRules.HighestMaxMonthlyFee);
            value = fee ?? 0;
            return fee.HasValue ? null : "The fee is an amount from 0 to 10,000, with at most 2 decimals.";
        }

        public override string Format(decimal value, Jurisdiction jurisdiction)
        {
            ulong? currencyId = _jurisdictionService.GetCurrencyId(jurisdiction);
            return currencyId.HasValue
                ? _currencyService.GetCurrencyFormat(currencyId.Value, value)
                : value.ToString(StoredFormat, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// A bound jurisdiction keeps its own cap, but never above its parent's.
        /// </summary>
        public override decimal Bind(decimal own, decimal parent)
        {
            return Math.Min(own, parent);
        }

        /// <summary>
        /// A bank's own fee can't go above the cap, so the text says what the parent allows at most.
        /// </summary>
        public override string GetBoundError(decimal parentValue, Jurisdiction parent, Jurisdiction jurisdiction)
        {
            return $"{parent.GetName()} allows at most {Format(parentValue, jurisdiction)}.";
        }

        public override string Serialize(decimal value)
        {
            return value.ToString(StoredFormat, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// A stored value that can't be read falls back to the default, so a hand-edited row can't break the menus.
        /// </summary>
        public override decimal Deserialize(string storedValue)
        {
            return decimal.TryParse(storedValue, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal value) ? value : Default;
        }
    }
}
