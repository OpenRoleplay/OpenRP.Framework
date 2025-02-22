using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Accounts.Services.Dialogs
{
    public interface IAccountDialogService
    {
        void OpenLoginAskUsernameDialog(Player player, Action onGoBack);
        void OpenLoginAskPasswordDialog(Player player, string username, Action onGoBack);
        void OpenRegistrationAskUsernameDialog(Player player, Action onGoBack);
        void OpenRegistrationUsernameAlreadyExistsDialog(Player player, Action onGoBack);
        void OpenRegistrationAskPasswordDialog(Player player, Action onGoBack);
        void OpenRegistrationAskPasswordConfirmationDialog(Player player, Action onGoBack);
    }
}
