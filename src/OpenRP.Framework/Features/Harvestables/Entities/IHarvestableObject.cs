using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Harvestables.Entities
{
    public interface IHarvestableObject
    {
        bool IsPlayerNearby(Player player);
        void BeginHarvest();
        void EndHarvest();
    }
}
