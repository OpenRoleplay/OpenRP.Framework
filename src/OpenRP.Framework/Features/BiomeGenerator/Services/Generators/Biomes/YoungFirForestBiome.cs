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
    [Biome(59, "Young Fir Forest", BiomeType.AngelPineMain)]
    public class YoungFirForestBiome : IBiome
    {
        private readonly IBiomeObjectFactory _factory;
        private readonly WeightedRandom<string> _weightedRandom;
        private readonly Color _biomeOutputColor;

        public YoungFirForestBiome(IBiomeObjectFactory factory)
        {
            _factory = factory;
            // Weights sum to 900 to preserve original probabilities exactly.
            _weightedRandom = new WeightedRandom<string>(new Dictionary<string, int>
            {
                { "YoungFirTree", 18 }, 
                { "Flower", 9 },  
                { "Grass", 44 },  
                { "ConiferousBush", 22 }, 
                { "Nothing", 807 }  
            });
            _biomeOutputColor = GetBiomeOutputColor();
        }

        public ConcurrentBag<BiomeObject> Generate(Vector2 virtualPosition, Vector3 gamePosition, Vector3 gameRotation, Vector3 defaultRotation, Vector3 maxAngleRotation)
        {
            var assets = new ConcurrentBag<BiomeObject>();
            string selectedType = _weightedRandom.GetRandomItem();

            if (selectedType != "Nothing")
            {
                // Create the object
                BiomeObject element = _factory.Generate(
                    selectedType,
                    virtualPosition,
                    gamePosition,
                    gameRotation,
                    defaultRotation,
                    maxAngleRotation,
                    _biomeOutputColor
                );

                assets.Add(element);
            }

            return assets;
        }

        public Color GetBiomeOutputColor() => new Color(0, 83, 0);
    }

}
