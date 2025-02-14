using OpenRP.Framework.Features.Fishing.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Database.Models
{
    public class FishLootModel
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public FishLootType FishLootType { get; set; }
        public int MinWeightInGrams { get; set; }
        public int MaxWeightInGrams { get; set; }
        public int OddsToCatch { get; set; }
        public ulong ItemId { get; set; }

        // Navigational Properties
        public ItemModel Item { get; set; }
    }
}
