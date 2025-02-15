using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.AccountSettingsFeature.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.AccountSettingsFeature.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAccountSettings(this IServiceCollection self)
        {
            return self
                .AddSingleton<IAccountSettingsService, AccountSettingsService>();
        }
    }
}
