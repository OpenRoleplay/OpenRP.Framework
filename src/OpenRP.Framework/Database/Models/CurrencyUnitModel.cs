using OpenRP.Framework.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Database.Models
{
    public class CurrencyUnitModel : BaseModel
    {
        public ulong CurrencyId { get; set; }
        public decimal UnitValue { get; set; }

        // Navigational Properties
        public CurrencyModel Currency { get; set; }
    }
}
