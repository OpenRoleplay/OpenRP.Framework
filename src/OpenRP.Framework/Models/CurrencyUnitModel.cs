using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Models
{
    public class CurrencyUnitModel
    {
        public ulong Id { get; set; }
        public ulong CurrencyId { get; set; }
        public decimal UnitValue { get; set; }

        // Navigational Properties
        public CurrencyModel Currency { get; set; }
    }
}
