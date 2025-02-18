using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.AccountSettingsFeature.Enums;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.AccountSettingsFeature.Components
{
    public class AccountSettings : Component
    {
        private AccountSettingsModel _accountSettingsModel;
        public AccountSettings(AccountSettingsModel accountSettingsModel)
        {
            _accountSettingsModel = accountSettingsModel;
        }

        public ulong GetDatabaseId()
        {
            return _accountSettingsModel.Id;
        }

        public AccountGraphicPreset GetAccountGraphicPreset()
        {
            return _accountSettingsModel.AccountGraphicPreset;
        }

        public bool GetGlobalChatEnabled()
        {
            return _accountSettingsModel.GlobalChatEnabled;
        }
    }
}
