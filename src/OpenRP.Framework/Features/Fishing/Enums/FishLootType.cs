using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Fishing.Enums
{
    [Flags]
    public enum FishLootType
    {
        Saltwater = 1,
        Freshwater = 2,
        Murkywater = 4,
        Misc = 8
    }
}
