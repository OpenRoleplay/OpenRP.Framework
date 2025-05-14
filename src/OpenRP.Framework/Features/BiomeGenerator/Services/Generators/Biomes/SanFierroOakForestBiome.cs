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
    [Biome(35, "Oak Forest", BiomeType.SanFierroMain)]
    public class SanFierroOakForestBiome : OakForestBiome
    {
        public SanFierroOakForestBiome(IBiomeObjectFactory factory) : base(factory) { }

        public override Color GetBiomeOutputColor() => new Color(128, 0, 0);
    }
}
