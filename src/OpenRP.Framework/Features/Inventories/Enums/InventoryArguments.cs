using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Inventories.Enums
{
    [Flags]
    public enum InventoryArguments
    {
        None = 0,
        HideTotalWeight = 1,
        HideExtraInformation = 2,
    }
}
