using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.Phones.Entities;
using OpenRP.Framework.Features.Phones.Services;
using OpenRP.Framework.Features.Phones.Services.Dialogs;

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
                .AddSingleton<IPhoneDialogService, PhoneDialogService>();
        }

        /// <summary>
        /// Adds a number below 1000 that the server answers itself, such as 911.
        /// </summary>
        public static IServiceCollection AddServerPhoneNumber<TServerPhoneNumber>(this IServiceCollection self)
            where TServerPhoneNumber : class, IServerPhoneNumber
        {
            return self.AddSingleton<IServerPhoneNumber, TServerPhoneNumber>();
        }
    }
}
