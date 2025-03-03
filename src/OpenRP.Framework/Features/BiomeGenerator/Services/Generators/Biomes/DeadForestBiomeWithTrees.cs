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
    [Biome(255, "Dead Forest With Trees", BiomeType.DeadForest)]
    public class DeadForestBiomeWithTrees : IBiome
    {
        private readonly IBiomeObjectFactory _factory;
        private readonly WeightedRandom<string> _weightedRandom;
        private readonly Color _biomeOutputColor;

        public DeadForestBiomeWithTrees(IBiomeObjectFactory factory)
        {
            _factory = factory;
            _weightedRandom = new WeightedRandom<string>(new Dictionary<string, int>
            {
                { "DryBush", 15 },
                { "Rubble", 10 },
                { "DeadTree", 10 },
                { "DeadTreeStanding", 5 },
                { "Nothing", 960 }
            });
            _biomeOutputColor = GetBiomeOutputColor();
        }

        public ConcurrentBag<BiomeObject> Generate(Vector2 virtualPosition, Vector3 gamePosition, Vector3 gameRotation, Vector3 defaultRotation)
        {
            string selectedType = _weightedRandom.GetRandomItem();
            var assets = new ConcurrentBag<BiomeObject>();

            if (selectedType != "Nothing")
            {
                BiomeObject element = _factory.Generate(selectedType, virtualPosition, gamePosition, gameRotation, defaultRotation, _biomeOutputColor);
                assets.Add(element);
            }

            return assets;
        }

        public virtual Color GetBiomeOutputColor() => new Color(48, 32, 19);
    }
}
