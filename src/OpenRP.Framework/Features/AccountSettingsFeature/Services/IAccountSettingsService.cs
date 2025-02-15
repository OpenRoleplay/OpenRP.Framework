using OpenRP.Framework.Features.AccountSettingsFeature.Components;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.AccountSettingsFeature.Services
{
    public interface IAccountSettingsService
    {
        AccountSettings ReloadAccountSettings(Player player);
        AccountSettings GetAccountSettings(Player player);
        void OpenAccountSettingsDialog(Player player);
    }
}
