using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Fishing.Enums;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Fishing.Services
{
    public interface IFishService
    {
        FishLootModel GetRandomFishLoot(FishLootType lootType);
        FishLootType GetFishLootTypeZone(Player player);
    }
}
