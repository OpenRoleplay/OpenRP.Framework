using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.Animations.Services;
using OpenRP.Framework.Features.VirtualWorlds.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.VirtualWorlds.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddVirtualWorldManager(this IServiceCollection self)
        {
            return self
                .AddSingleton<IVirtualWorldManager, VirtualWorldManager>();
        }
    }
}
