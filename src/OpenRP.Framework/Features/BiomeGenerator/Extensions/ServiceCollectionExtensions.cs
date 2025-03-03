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
                .AddSingleton<IBiomeObjectGenerator, DryBushGenerator>()
                .AddSingleton<IBiomeObjectGenerator, FarmlandGenerator>()
                .AddSingleton<IBiomeObjectGenerator, FlowerGenerator>()
                .AddSingleton<IBiomeObjectGenerator, GrassGenerator>()
                .AddSingleton<IBiomeObjectGenerator, MultipleSunflowerGenerator>()
                .AddSingleton<IBiomeObjectGenerator, OakTreeGenerator>()
                .AddSingleton<IBiomeObjectGenerator, PineTreeGenerator>()
                .AddSingleton<IBiomeObjectGenerator, RedCountyBushGenerator>()
                .AddSingleton<IBiomeObjectGenerator, RubbleGenerator>()
                .AddSingleton<IBiomeObjectGenerator, SingleSunflowerGenerator>()
                .AddSingleton<BirchForestBiome>()
                .AddSingleton<ConiferousWoodlandBiome>()
                .AddSingleton<DeadForestBiome>()
                .AddSingleton<DeadForestBiomeWithTrees>()
                .AddSingleton<FarmlandBiome>()
                .AddSingleton<ForestPlainsBiome>()
                .AddSingleton<OakForestBiome>()
                .AddSingleton<PlainsBiome>()
                .AddSingleton<SunflowerBiome>()
                .AddSingleton<IBiomeObjectFactory, BiomeObjectFactory>();
        }
    }
}
