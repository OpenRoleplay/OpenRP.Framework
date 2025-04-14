using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.Accounts.Services.Dialogs;
using OpenRP.Framework.Features.Accounts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRP.Framework.Features.BiomeGenerator.Entities;
using OpenRP.Framework.Features.BiomeGenerator.Services;
using OpenRP.Framework.Features.BiomeGenerator.Services.Generators.Objects;
using OpenRP.Framework.Features.BiomeGenerator.Services.Generators.Biomes;

namespace OpenRP.Framework.Features.BiomeGenerator.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBiomeGenerator(this IServiceCollection self)
        {
            return self
                .AddSingleton<IBiomeObjectGenerator, BirchTreeGenerator>()
                .AddSingleton<IBiomeObjectGenerator, ConiferousBushGenerator>()
                .AddSingleton<IBiomeObjectGenerator, ConiferousDeadTreeGenerator>()
                .AddSingleton<IBiomeObjectGenerator, DeadBirchTreeGenerator>()
                .AddSingleton<IBiomeObjectGenerator, DeadTreeGenerator>()
                .AddSingleton<IBiomeObjectGenerator, DeadTreeStandingGenerator>()
                .AddSingleton<IBiomeObjectGenerator, DenseFirTreeGenerator>()
                .AddSingleton<IBiomeObjectGenerator, DryBushGenerator>()
                .AddSingleton<IBiomeObjectGenerator, ElmTreeGenerator>()
                .AddSingleton<IBiomeObjectGenerator, FarmlandGenerator>()
                .AddSingleton<IBiomeObjectGenerator, FlowerGenerator>()
                .AddSingleton<IBiomeObjectGenerator, GrassGenerator>()
                .AddSingleton<IBiomeObjectGenerator, LosSantosPlantGenerator>()
                .AddSingleton<IBiomeObjectGenerator, LosSantosRockGenerator>()
                .AddSingleton<IBiomeObjectGenerator, LosSantosTreeGenerator>()
                .AddSingleton<IBiomeObjectGenerator, MultipleSunflowerGenerator>()
                .AddSingleton<IBiomeObjectGenerator, OakTreeGenerator>()
                .AddSingleton<IBiomeObjectGenerator, PineTreeGenerator>()
                .AddSingleton<IBiomeObjectGenerator, RedCountyBushGenerator>()
                .AddSingleton<IBiomeObjectGenerator, RedwoodTreeGenerator>()
                .AddSingleton<IBiomeObjectGenerator, RubbleGenerator>()
                .AddSingleton<IBiomeObjectGenerator, SingleSunflowerGenerator>()
                .AddSingleton<BirchForestBiome>()
                .AddSingleton<ConiferousWoodlandBiome>()
                .AddSingleton<DeadForestBiome>()
                .AddSingleton<DeadForestBiomeWithTrees>()
                .AddSingleton<DenseFirForestBiome>()
                .AddSingleton<ElmForestBiome>()
                .AddSingleton<FarmlandBiome>()
                .AddSingleton<ForestPlainsBiome>()
                .AddSingleton<OakForestBiome>()
                .AddSingleton<PalmBeachBiome>()
                .AddSingleton<PalmPlainsBiome>()
                .AddSingleton<PlainsBiome>()
                .AddSingleton<RedwoodForestBiome>()
                .AddSingleton<SanFierroBirchForestBiome>()
                .AddSingleton<SanFierroOakForestBiome>()
                .AddSingleton<SunflowerBiome>()
                .AddSingleton<IBiomeObjectFactory, BiomeObjectFactory>();
        }
    }
}
