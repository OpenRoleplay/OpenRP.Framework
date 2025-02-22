using Microsoft.Extensions.DependencyInjection;
using OpenRP.Framework.Features.Accounts.Services.Dialogs;
using OpenRP.Framework.Features.Accounts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRP.Framework.Features.MainMenu.Services.Dialogs;
using OpenRP.Framework.Features.MainMenu.Entities.Scenes;
using OpenRP.Framework.Features.MainMenu.Entities;

namespace OpenRP.Framework.Features.MainMenu.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMainMenu(this IServiceCollection self)
        {
            return self
                .AddSingleton<IMainMenuDialogService, MainMenuDialogService>()
                .AddSingleton<IMainMenuScene, FamilyBarbecueScene>();
        }
    }
}
