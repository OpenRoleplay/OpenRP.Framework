using OpenRP.Framework.Features.EmergencyCalls.Enums;
using OpenRP.Framework.Features.Phones.Entities;
using SampSharp.Entities.SAMP;
using System;

namespace OpenRP.Framework.Features.EmergencyCalls.Services.Dialogs
{
    public interface IEmergencyCallDialogService
    {
        /// <summary>
        /// The 911 form for when there's no AI dispatcher: which service, what's happening and where. Calls onFilled
        /// with the answers, unless the call ended in the meantime.
        /// </summary>
        void OpenEmergencyForm(Player caller, IServerPhoneLine line, Action<EmergencyServiceType, string, string> onFilled);
    }
}
