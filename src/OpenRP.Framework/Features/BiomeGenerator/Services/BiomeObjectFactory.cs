using OpenRP.Framework.Features.BiomeGenerator.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.BiomeGenerator.Services
{
    public class BiomeObjectFactory : IBiomeObjectFactory
    {
        private readonly Dictionary<string, IBiomeObjectGenerator> _generators;

        public BiomeObjectFactory(IEnumerable<IBiomeObjectGenerator> generators)
        {
            _generators = generators.ToDictionary(g => g.ObjectType, g => g);
        }

        public BiomeObject Generate(string assetType, Vector2 virtualPosition, Vector3 gamePosition, Vector3 gameRotation, Vector3 defaultRotation, Color outputColor)
        {
            if (_generators.TryGetValue(assetType, out var generator))
            {
                return generator.Generate(virtualPosition, gamePosition, gameRotation, defaultRotation, outputColor);
            }
            throw new InvalidOperationException($"No generator registered for asset type: {assetType}");
        }
    }
}
