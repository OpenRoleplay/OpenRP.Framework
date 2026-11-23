using OpenRP.Framework.Database.Entities;
using OpenRP.Framework.Features.Fishing.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Database.Models
{
    public class FishLootModel : BaseModel
    {
        public string Name { get; set; }
        public FishLootType FishLootType { get; set; }
        public FishLootKind LootKind { get; set; }
        public FishSizeClass SizeClass { get; set; }
        public uint RequiredFishingLevel { get; set; }
        public bool IsNetOnly { get; set; }
        public int MinWeightInGrams { get; set; }
        public int MaxWeightInGrams { get; set; }
        // How likely this catch is compared to the other catches that are possible on the same bite.
        public int CatchWeight { get; set; }
        public ulong ItemId { get; set; }

        // Navigational Properties
        public ItemModel Item { get; set; }
    }
}
