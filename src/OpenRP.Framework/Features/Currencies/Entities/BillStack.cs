using OpenRP.Framework.Features.Items.Components;

namespace OpenRP.Framework.Features.Currencies.Entities
{
    /// <summary>
    /// A number of bills (or coins) of one denomination, for example 3 x $20.
    /// </summary>
    public record BillStack(Item Item, uint Amount, decimal UnitValue)
    {
        public decimal GetValue()
        {
            return Amount * UnitValue;
        }

        public uint GetWeight()
        {
            return Item.GetWeight() * Amount;
        }
    }
}
