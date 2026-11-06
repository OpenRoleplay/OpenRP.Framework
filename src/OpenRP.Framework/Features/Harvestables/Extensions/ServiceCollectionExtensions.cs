using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.Harvestables.Entities;
using OpenRP.Framework.Features.Harvestables.Services;

namespace OpenRP.Framework.Features.Harvestables.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHarvestables(this IServiceCollection self)
        {
            return self
                .AddSingleton<IHarvestableService, HarvestableService>()
                .AddHarvestableModel<IndianHempHarvestableModel>();
        }

        /// <summary>
        /// Registers a harvestable plant type. Gamemodes can call this for their own plants.
        /// </summary>
        public static IServiceCollection AddHarvestableModel<TModel>(this IServiceCollection self)
            where TModel : class, IHarvestableModel
        {
            return self.AddSingleton<IHarvestableModel, TModel>();
        }
    }
}
