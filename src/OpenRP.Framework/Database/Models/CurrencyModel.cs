using OpenRP.Framework.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Database.Models
{
    public class CurrencyModel : BaseModel
    {
        public string CurrencyCode { get; set; }
        public string Name { get; set; }

        // Navigational Properties
        public List<CurrencyUnitModel> CurrencyUnits { get; set; }
    }
}
