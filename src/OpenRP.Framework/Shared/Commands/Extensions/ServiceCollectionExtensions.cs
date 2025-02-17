using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.Inventories.Services;
using OpenRP.Framework.Shared.Commands.Services;
using SampSharp.Entities.SAMP.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Shared.Commands.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommands(this IServiceCollection self)
        {
            return self
                .AddSingleton<IPlayerCommandService, PlayerServerCommandService>();
        }
    }
}
