using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.MainMenu.Services.Dialogs
{
    public interface IMainMenuDialogService
    {
        void OpenMainMenu(Player player);
        void OpenMainMenuChoiceMenu(Player player);
    }
}
