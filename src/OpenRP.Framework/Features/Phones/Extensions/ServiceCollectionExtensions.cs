using Microsoft.Extensions.DependencyInjection;
using System;
using OpenRP.Framework.Features.Phones.Entities;
using OpenRP.Framework.Features.Phones.Services;
using OpenRP.Framework.Features.Phones.Services.Dialogs;
using OpenRP.Framework.Shared.Chat.Extensions;

namespace OpenRP.Framework.Features.Phones.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPhones(this IServiceCollection self)
        {
            return self
                .AddSingleton<IPhoneService, PhoneService>()
                .AddSingleton<IPhoneManager, PhoneManager>()
                .AddSingleton<IPhoneContactManager, PhoneContactManager>()
                .AddSingleton<IPhoneMessageManager, PhoneMessageManager>()
                .AddSingleton<IPhoneCallLogManager, PhoneCallLogManager>()
                .AddSingleton<IPhoneAlertService, PhoneAlertService>()
                .AddSingleton<IPhoneRoleplayService, PhoneRoleplayService>()
                .AddSingleton<IPhoneEditorService, PhoneEditorService>()
                .AddSingleton<IPhoneCallService, PhoneCallService>()
                .AddSingleton<IPhoneDialogService, PhoneDialogService>()
                .AddChatSpeechLabelProvider<PhoneSpeechLabelProvider>();
        }

        /// <summary>
        /// Adds a number below 1000 that the server answers itself, such as 911.
        /// </summary>
        public static IServiceCollection AddServerPhoneNumber<TServerPhoneNumber>(this IServiceCollection self)
            where TServerPhoneNumber : class, IServerPhoneNumber
        {
            return self.AddSingleton<IServerPhoneNumber, TServerPhoneNumber>();
        }

        /// <summary>
        /// Adds a server number made by a factory, for a number that's also registered as a service of its own.
        /// </summary>
        public static IServiceCollection AddServerPhoneNumber(this IServiceCollection self, Func<IServiceProvider, IServerPhoneNumber> factory)
        {
            return self.AddSingleton(factory);
        }
    }
}
