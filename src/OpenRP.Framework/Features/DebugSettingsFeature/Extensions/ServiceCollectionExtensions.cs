using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.Characters.Services;
using OpenRP.Framework.Features.DebugSettingsFeature.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.DebugSettingsFeature.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDebugSettings(this IServiceCollection self)
        {
            return self
                .AddSingleton<IDebugSettingsService, DebugSettingsService>();
        }
    }
}
