using OpenRP.Framework.Features.EmergencyCalls.Services;
using SampSharp.Entities;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.EmergencyCalls.Systems
{
    public class EmergencyCallSystem : ISystem
    {
        // Replies wait for a pause of a few seconds (AiReplyTiming), so checking twice a second is precise enough.
        private const int ReplyCheckIntervalInMilliseconds = 500;

        /// <summary>
        /// Takes all 911 numbers, so this does nothing on a server without AddEmergencyCalls.
        /// </summary>
        [Timer(ReplyCheckIntervalInMilliseconds)]
        public void ReplyToQuietCallers(IEnumerable<EmergencyNumber> emergencyNumbers)
        {
            foreach (EmergencyNumber emergencyNumber in emergencyNumbers)
            {
                emergencyNumber.ReplyToQuietCallers();
            }
        }
    }
}
