using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.Inventories.Services;
using OpenRP.Framework.Features.Permissions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Permissions.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPermissions(this IServiceCollection self)
        {
            return self
                .AddTransient<IPermissionService, PermissionService>(); // Transient because it uses BaseDataContext.
        }
    }
}
