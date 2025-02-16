using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.Vehicles.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Vehicles.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddVehicles(this IServiceCollection self)
        {
            return self
                .AddSingleton<IVehicleManager, VehicleManager>();
        }
    }
}
