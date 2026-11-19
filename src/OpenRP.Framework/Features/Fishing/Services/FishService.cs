using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Database.Services;
using OpenRP.Framework.Features.Fishing.Entities;
using OpenRP.Framework.Features.Fishing.Enums;
using OpenRP.Framework.Features.Fishing.Helpers;
using OpenRP.Framework.Features.Items.Components;
using OpenRP.Framework.Features.Items.Entities;
using OpenRP.Framework.Features.Skills.Components;
using OpenRP.Framework.Features.Skills.Entities;
using OpenRP.Framework.Features.Skills.Services;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;
using OpenRP.Framework.Shared.ServerEvents.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Fishing.Services
{
    public class FishService : IFishService
    {
        private readonly IDataMemoryService _dataMemory;
        private readonly IEntityManager _entityManager;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IServerEventAggregator _serverEventAggregator;
        private readonly ILogger<FishService> _logger;

        public FishService(IDataMemoryService dataMemory, IEntityManager entityManager, IServiceScopeFactory scopeFactory, IServerEventAggregator serverEventAggregator, ILogger<FishService> logger)
        {
            _dataMemory = dataMemory;
            _entityManager = entityManager;
            _scopeFactory = scopeFactory;
            _serverEventAggregator = serverEventAggregator;
            _logger = logger;
        }

        public FishLootType GetFishLootTypeZone(Player player)
        {
            if (player.IsInFreshWaterZone())
            {
                return FishLootType.Freshwater;
            }
            if (player.IsInMuddyWaterZone())
            {
                return FishLootType.Murkywater;
            }
            return FishLootType.Saltwater;
        }

        public FishLootModel? GetRandomRodCatch(FishLootType waterType, uint fishingLevel, bool isOnBoat)
        {
            List<FishLootModel> candidates = _dataMemory.GetFishSpecies()
                .Where(loot => !loot.IsNetOnly && IsFoundInWater(loot, waterType) && fishingLevel >= loot.RequiredFishingLevel)
                .ToList();

            return PickByCatchWeight(candidates, loot => loot.CatchWeight * GetLocationOdds(loot, isOnBoat));
        }

        public IReadOnlyList<FishLootModel> GetRandomNetCatch(FishLootType waterType, uint fishingLevel)
        {
            List<FishLootModel> hugeFish = GetNetOnlyFish(waterType, fishingLevel);
            List<FishLootModel> biggerFish = GetBiggerFish(waterType, fishingLevel);
            return GetNetResult(hugeFish, biggerFish) switch
            {
                FishNetResult.OneHugeFish => PickNetFish(hugeFish, 1),
                FishNetResult.FewBiggerFish => PickNetFish(biggerFish, Random.Shared.Next(FishingSettings.MinFewBiggerFish, FishingSettings.MaxFewBiggerFish + 1)),
                _ => PickNetFish(GetSmallFish(waterType), Random.Shared.Next(FishingSettings.MinManySmallFish, FishingSettings.MaxManySmallFish + 1)),
            };
        }

        public uint GetRandomWeightInGrams(FishLootModel loot)
        {
            return (uint)Random.Shared.Next(loot.MinWeightInGrams, loot.MaxWeightInGrams + 1);
        }

        public uint GetExperienceForCatch(FishLootModel loot)
        {
            return loot.LootKind switch
            {
                FishLootKind.Junk => FishingSettings.JunkExperience,
                FishLootKind.Treasure => FishingSettings.TreasureExperience,
                _ => GetExperienceForFishSize(loot.SizeClass),
            };
        }

        public uint GetFishingLevel(Player player)
        {
            return player.GetComponent<CharacterSkills>()?.GetSkillLevel(FishingSettings.SkillName) ?? 0;
        }

        public void AddFishingExperience(Player player, uint experience)
        {
            // The skill service holds a database context, so it only lives as long as this scope.
            using IServiceScope scope = _scopeFactory.CreateScope();
            ISkillService skillService = scope.ServiceProvider.GetRequiredService<ISkillService>();
            SkillExperienceResult? result = skillService.AddExperience(player, FishingSettings.SkillName, experience);
            if (result != null && result.HasLeveledUp)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"Your fishing skill is now level {result.Skill.Level}!");
            }
        }

        public Item? GetLootItem(FishLootModel loot)
        {
            return _entityManager.GetComponent<Item>(ItemEntities.GetItemId((int)loot.ItemId));
        }

        public async void PublishFishCaught(Player player, FishLootModel loot, uint weightInGrams, FishCatchMethod method)
        {
            try
            {
                await _serverEventAggregator.PublishAsync(new OnFishCaughtEventArgs { Player = player, Loot = loot, WeightInGrams = weightInGrams, Method = method });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish that {LootName} was caught.", loot.Name);
            }
        }

        private static bool IsFoundInWater(FishLootModel loot, FishLootType waterType)
        {
            return loot.FishLootType.HasFlag(waterType) || loot.FishLootType.HasFlag(FishLootType.Misc);
        }

        /// <summary>
        /// Big fish are harder to land from the shore. Junk and treasures don't care where you stand.
        /// </summary>
        private static float GetLocationOdds(FishLootModel loot, bool isOnBoat)
        {
            if (isOnBoat || loot.LootKind != FishLootKind.Fish)
            {
                return 1f;
            }

            return loot.SizeClass switch
            {
                FishSizeClass.Medium => FishingSettings.MediumFishLandOdds,
                FishSizeClass.Large => FishingSettings.LargeFishLandOdds,
                FishSizeClass.Huge => FishingSettings.HugeFishLandOdds,
                _ => 1f,
            };
        }

        private static uint GetExperienceForFishSize(FishSizeClass sizeClass)
        {
            return sizeClass switch
            {
                FishSizeClass.Medium => FishingSettings.MediumFishExperience,
                FishSizeClass.Large => FishingSettings.LargeFishExperience,
                FishSizeClass.Huge => FishingSettings.HugeFishExperience,
                _ => FishingSettings.SmallFishExperience,
            };
        }

        /// <summary>
        /// Rolls the result of a trawl. A result with no fish the driver has the level for becomes the next smaller one.
        /// </summary>
        private static FishNetResult GetNetResult(List<FishLootModel> hugeFish, List<FishLootModel> biggerFish)
        {
            FishNetResult result = RollNetResult();
            if (result == FishNetResult.OneHugeFish && hugeFish.Count == 0)
            {
                result = FishNetResult.FewBiggerFish;
            }
            if (result == FishNetResult.FewBiggerFish && biggerFish.Count == 0)
            {
                result = FishNetResult.ManySmallFish;
            }
            return result;
        }

        private static FishNetResult RollNetResult()
        {
            int roll = Random.Shared.Next(100);
            if (roll < FishingSettings.ManySmallFishChance)
            {
                return FishNetResult.ManySmallFish;
            }
            return roll < FishingSettings.ManySmallFishChance + FishingSettings.FewBiggerFishChance
                ? FishNetResult.FewBiggerFish
                : FishNetResult.OneHugeFish;
        }

        private List<FishLootModel> GetNetOnlyFish(FishLootType waterType, uint fishingLevel)
        {
            return _dataMemory.GetFishSpecies()
                .Where(loot => loot.IsNetOnly && IsFoundInWater(loot, waterType) && fishingLevel >= loot.RequiredFishingLevel)
                .ToList();
        }

        private List<FishLootModel> GetBiggerFish(FishLootType waterType, uint fishingLevel)
        {
            return GetRegularFish()
                .Where(loot => (loot.SizeClass == FishSizeClass.Medium || loot.SizeClass == FishSizeClass.Large)
                    && IsFoundInWater(loot, waterType)
                    && fishingLevel >= loot.RequiredFishingLevel)
                .ToList();
        }

        private List<FishLootModel> GetSmallFish(FishLootType waterType)
        {
            return GetRegularFish()
                .Where(loot => loot.SizeClass == FishSizeClass.Small && IsFoundInWater(loot, waterType))
                .ToList();
        }

        private IEnumerable<FishLootModel> GetRegularFish()
        {
            return _dataMemory.GetFishSpecies().Where(loot => loot.LootKind == FishLootKind.Fish && !loot.IsNetOnly);
        }

        private static List<FishLootModel> PickNetFish(List<FishLootModel> candidates, int amount)
        {
            List<FishLootModel> caughtFish = new List<FishLootModel>();
            for (int i = 0; i < amount; i++)
            {
                FishLootModel? fish = PickByCatchWeight(candidates, loot => loot.CatchWeight);
                if (fish != null)
                {
                    caughtFish.Add(fish);
                }
            }
            return caughtFish;
        }

        private static FishLootModel? PickByCatchWeight(List<FishLootModel> candidates, Func<FishLootModel, float> getWeight)
        {
            float totalWeight = candidates.Sum(getWeight);
            if (totalWeight <= 0f)
            {
                return null;
            }

            float roll = Random.Shared.NextSingle() * totalWeight;
            foreach (FishLootModel candidate in candidates)
            {
                roll -= getWeight(candidate);
                if (roll < 0f)
                {
                    return candidate;
                }
            }
            return candidates[^1];
        }
    }
}
