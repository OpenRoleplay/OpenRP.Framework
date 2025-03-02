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
using OpenRP.Framework.Features.BiomeGenerator.Services.ObjectGenerators;

namespace OpenRP.Framework.Features.BiomeGenerator.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBiomeGenerator(this IServiceCollection self)
        {
            return self
                .AddSingleton<IBiomeObjectGenerator, DeadBirchTreeGenerator>()
                .AddSingleton<IBiomeObjectGenerator, DeadTreeGenerator>()
                .AddSingleton<IBiomeObjectGenerator, GrassGenerator>()
                .AddSingleton<IBiomeObjectGenerator, RedCountyBushGenerator>()
                .AddSingleton<IBiomeObjectGenerator, SingleSunflowerGenerator>()
                .AddSingleton<IBiomeObjectGenerator, BirchTreeGenerator>()
                .AddSingleton<IBiomeObjectGenerator, FlowerGenerator>()
                .AddSingleton<IBiomeObjectFactory, BiomeObjectFactory>();
        }
    }
}
