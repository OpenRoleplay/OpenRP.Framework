using OpenRP.Framework.Features.EmergencyCalls.Enums;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.EmergencyCalls.Entities
{
    /// <summary>
    /// What the AI dispatcher does next: what it says to the caller, at most one radio broadcast per service, and
    /// whether it hangs up.
    /// </summary>
    public class EmergencyDispatcherReply
    {
        public List<string> SayLines { get; } = new List<string>();
        public List<EmergencyDispatch> Dispatches { get; } = new List<EmergencyDispatch>();
        public bool HangsUp { get; set; }

        /// <summary>
        /// Adds the broadcast, unless this reply already has one for that service.
        /// </summary>
        public void AddDispatch(EmergencyServiceType service, string text)
        {
            if (!string.IsNullOrWhiteSpace(text) && !Dispatches.Any(dispatch => dispatch.Service == service))
            {
                Dispatches.Add(new EmergencyDispatch(service, text));
            }
        }
    }
}
