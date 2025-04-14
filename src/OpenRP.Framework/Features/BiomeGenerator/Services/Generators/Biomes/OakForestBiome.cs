using OpenRP.Framework.Features.BiomeGenerator.Attributes;
using OpenRP.Framework.Features.BiomeGenerator.Entities;
using OpenRP.Framework.Features.BiomeGenerator.Enums;
using OpenRP.Framework.Features.BiomeGenerator.Helpers;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.BiomeGenerator.Services.Generators.Biomes
{
    [Biome(57, "Oak Forest", BiomeType.RedCounty)]
    public class OakForestBiome : IBiome
    {
        private readonly IBiomeObjectFactory _factory;
        private readonly WeightedRandom<string> _weightedRandom;
        private readonly Color _biomeOutputColor;

        public OakForestBiome(IBiomeObjectFactory factory)
        {
            _factory = factory;
            _weightedRandom = new WeightedRandom<string>(new Dictionary<string, int>
            {
                { "Grass", 100 },
                { "RedCountyBush", 15 },
                { "OakTree", 15 },
                { "Flower", 48 },
                { "DeadTree", 5 },
                { "Sunflower", 2 },
                { "Nothing", 810 }
            });
            _biomeOutputColor = GetBiomeOutputColor();
        }

        public ConcurrentBag<BiomeObject> Generate(Vector2 virtualPosition, Vector3 gamePosition, Vector3 gameRotation, Vector3 defaultRotation, Vector3 maxAngleRotation)
        {
            string selectedType = _weightedRandom.GetRandomItem();
            var assets = new ConcurrentBag<BiomeObject>();

            if (selectedType != "Nothing")
            {
                BiomeObject element = _factory.Generate(selectedType, virtualPosition, gamePosition, gameRotation, defaultRotation, maxAngleRotation, _biomeOutputColor);
                assets.Add(element);
            }

            return assets;
        }

        public virtual Color GetBiomeOutputColor() => new Color(0, 128, 0);
    }
}
