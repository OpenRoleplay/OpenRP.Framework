using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Database.Services;
using OpenRP.Framework.Features.Fishing.Enums;
using OpenRP.Framework.Features.Fishing.Helpers;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Fishing.Services
{
    public class FishService : IFishService
    {
        private IDataMemoryService _dataMemory;
        private static readonly Random _random = new Random();

        public FishService(IDataMemoryService dataMemory)
        {
            _dataMemory = dataMemory;
        }

        /// <summary>
        /// Returns a randomly selected fish species from those available for the given water type,
        /// taking into account the species’ OddsToCatch. The odds represent a 1 in X chance that the fish
        /// is caught. If none of the candidate fish pass the roll, null is returned.
        /// </summary>
        /// <param name="waterType">The water type to fish in.</param>
        /// <returns>A caught FishSpeciesModel or null if no fish was caught.</returns>
        public FishLootModel GetRandomFishLoot(FishLootType lootType)
        {
            // Filter the available fish species by checking if the species' water type flags match the given water type.
            var speciesList = _dataMemory.GetFishSpecies()
                .Where(f => f.FishLootType.HasFlag(lootType) || f.FishLootType.HasFlag(FishLootType.Misc))
                .ToList();

            if (!speciesList.Any())
                return null;

            // Roll for each species using its OddsToCatch.
            var caughtCandidates = new List<FishLootModel>();
            foreach (var species in speciesList)
            {
                // Roll a random number between 1 and OddsToCatch (inclusive).
                // If the result is 1, consider this species as caught.
                if (_random.Next(1, species.OddsToCatch + 1) == 1)
                {
                    caughtCandidates.Add(species);
                }
            }

            // If any candidates succeeded, return one at random.
            if (caughtCandidates.Any())
            {
                int index = _random.Next(caughtCandidates.Count);
                return caughtCandidates[index];
            }

            // No species passed the chance roll.
            return null;
        }

        public FishLootType GetFishLootTypeZone(Player player)
        {
            if (player.IsInFreshWaterZone())
            {
                return FishLootType.Freshwater;
            }
            else if (player.IsInMuddyWaterZone())
            {
                return FishLootType.Murkywater;
            }

            return FishLootType.Saltwater;
        }
    }
}
