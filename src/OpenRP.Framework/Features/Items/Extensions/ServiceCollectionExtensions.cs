using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.Actors.Services;
using OpenRP.Framework.Features.Items.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Items.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddItems(this IServiceCollection self)
        {
            return self
                .AddSingleton<IItemManager, ItemManager>();
        }
    }
}
