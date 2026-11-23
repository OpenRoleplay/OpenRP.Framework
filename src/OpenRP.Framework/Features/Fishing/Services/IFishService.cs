using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Fishing.Enums;
using OpenRP.Framework.Features.Items.Components;
using SampSharp.Entities.SAMP;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Fishing.Services
{
    public interface IFishService
    {
        /// <summary>
        /// The kind of water at the player's position.
        /// </summary>
        FishLootType GetFishLootTypeZone(Player player);

        /// <summary>
        /// Picks what a rod brings up, by catch weight. Null only when the catch table has nothing for this water and level.
        /// </summary>
        FishLootModel? GetRandomRodCatch(FishLootType waterType, uint fishingLevel, bool isOnBoat);

        /// <summary>
        /// Picks what the net brings up at the end of a trawl: many small fish, fewer bigger fish, or one huge fish.
        /// </summary>
        IReadOnlyList<FishLootModel> GetRandomNetCatch(FishLootType waterType, uint fishingLevel);

        /// <summary>
        /// A random weight for one catch, from the minimum up to and including the maximum.
        /// </summary>
        uint GetRandomWeightInGrams(FishLootModel loot);

        /// <summary>
        /// The experience one catch is worth. Bigger catches are worth more.
        /// </summary>
        uint GetExperienceForCatch(FishLootModel loot);

        /// <summary>
        /// The fishing level of the player's character.
        /// </summary>
        uint GetFishingLevel(Player player);

        /// <summary>
        /// Gives the player fishing experience, and tells them when they reach a new level.
        /// </summary>
        void AddFishingExperience(Player player, uint experience);

        /// <summary>
        /// The item this catch is, or null when the item is missing from the item table.
        /// </summary>
        Item? GetLootItem(FishLootModel loot);

        /// <summary>
        /// Tells other features that the player brought something up.
        /// </summary>
        void PublishFishCaught(Player player, FishLootModel loot, uint weightInGrams, FishCatchMethod method);
    }
}
