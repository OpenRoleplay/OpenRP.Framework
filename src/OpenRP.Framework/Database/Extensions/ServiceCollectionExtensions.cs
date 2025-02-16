using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Database.Services;
using OpenRP.Framework.Features.Fishing.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Database.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection self)
        {
            return self
                .AddSingleton<IDataMemoryService, DataMemoryService>();
        }
    }
}
