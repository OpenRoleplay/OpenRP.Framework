using OpenRP.Framework.Features.AccountSettingsFeature.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Models
{
    public class AccountSettingsModel
    {
        public ulong Id { get; set; }
        public ulong AccountId { get; set; }
        public AccountGraphicPreset AccountGraphicPreset { get; set; }

        // Navigational Properties
        public AccountModel AccountModel { get; set; }

        // Constructor
        public AccountSettingsModel()
        {
            AccountGraphicPreset = AccountGraphicPreset.High;
        }
    }
}
