using OpenRP.Framework.Features.EmergencyCalls.Services;
using OpenRP.Framework.Shared.ServerEvents.Attributes;
using OpenRP.Framework.Shared.ServerEvents.Entities;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.EmergencyCalls.Systems
{
    public class EmergencyCallServerSystem : IServerSystem
    {
        /// <summary>
        /// Takes all dispatch services, so this does nothing on a server without AddEmergencyCalls.
        /// </summary>
        [ServerEvent]
        public void OnRadiosLoaded(OnRadiosLoadedEventArgs args, IEnumerable<IDispatchService> dispatchServices)
        {
            foreach (IDispatchService dispatchService in dispatchServices)
            {
                dispatchService.CreateDispatchFrequencyIfMissing();
            }
        }
    }
}
