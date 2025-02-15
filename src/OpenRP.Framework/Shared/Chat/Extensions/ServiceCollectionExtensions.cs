using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.Accounts.Services;
using OpenRP.Framework.Shared.Chat.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Shared.Chat.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddChat(this IServiceCollection self)
        {
            return self
                .AddSingleton<IChatService, ChatService>();
        }
    }
}
