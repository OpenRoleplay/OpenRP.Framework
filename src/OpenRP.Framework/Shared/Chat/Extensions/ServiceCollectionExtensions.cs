using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.Accounts.Services;
using OpenRP.Framework.Shared.Chat.Entities;
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

        /// <summary>
        /// Adds a provider that labels what a player talks into, for example "(phone)" during a call.
        /// </summary>
        public static IServiceCollection AddChatSpeechLabelProvider<TProvider>(this IServiceCollection self)
            where TProvider : class, IChatSpeechLabelProvider
        {
            return self.AddSingleton<IChatSpeechLabelProvider, TProvider>();
        }
    }
}
