using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.BiomeGenerator.Helpers
{
    public class WeightedRandom<T>
    {
        private readonly List<(T item, float min, float max)> _weightedRanges;
        private readonly Random _random;

        public WeightedRandom(Dictionary<T, int> probabilities)
        {
            _random = new Random();
            _weightedRanges = new List<(T, float, float)>();

            // Sum up all the probabilities
            float totalWeight = probabilities.Values.Sum();

            // Calculate cumulative ranges
            float cumulative = 0f;
            foreach (var item in probabilities)
            {
                float normalizedWeight = (item.Value / totalWeight) * 100f; // Normalize to percentage
                _weightedRanges.Add((item.Key, cumulative, cumulative + normalizedWeight));
                cumulative += normalizedWeight;
            }
        }

        public T GetRandomItem()
        {
            float roll = (float)_random.NextDouble() * 100;  // Random float in [0, 100)
            return _weightedRanges.First(range => roll >= range.min && roll < range.max).item;
        }
    }
}
