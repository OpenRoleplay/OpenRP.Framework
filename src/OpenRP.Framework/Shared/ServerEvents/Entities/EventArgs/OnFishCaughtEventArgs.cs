using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Fishing.Enums;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs
{
    /// <summary>
    /// A player brought something up with a rod or a net: a fish, junk or a treasure.
    /// </summary>
    public class OnFishCaughtEventArgs : ServerEventArgs
    {
        public Player? Player { get; set; }
        public FishLootModel? Loot { get; set; }
        public uint WeightInGrams { get; set; }
        public FishCatchMethod Method { get; set; }
    }
}
