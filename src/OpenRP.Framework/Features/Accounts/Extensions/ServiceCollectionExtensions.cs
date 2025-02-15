using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.Accounts.Services;
using OpenRP.Framework.Features.Animations.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Accounts.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAccounts(this IServiceCollection self)
        {
            return self
                .AddSingleton<IAccountService, AccountService>();
        }
    }
}
