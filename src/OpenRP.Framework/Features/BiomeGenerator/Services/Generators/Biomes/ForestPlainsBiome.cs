using OpenRP.Framework.Features.BiomeGenerator.Attributes;
using OpenRP.Framework.Features.BiomeGenerator.Enums;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.BiomeGenerator.Services.Generators.Biomes
{
    [Biome(10, "Forest Plains", BiomeType.RedCounty)]
    public class ForestPlainsBiome : PlainsBiome
    {
        public ForestPlainsBiome(IBiomeObjectFactory factory) : base(factory) { }

        public override Color GetBiomeOutputColor() => new Color(0, 255, 0);
    }
}
