using OpenRP.Framework.Features.EmergencyCalls.Enums;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.EmergencyCalls.Entities
{
    /// <summary>
    /// What the AI dispatcher does next: what it says to the caller, an optional radio broadcast to units, and whether
    /// it hangs up.
    /// </summary>
    public class EmergencyDispatcherReply
    {
        public List<string> SayLines { get; } = new List<string>();
        public EmergencyServiceType? DispatchService { get; set; }
        public string? DispatchText { get; set; }
        public bool HangsUp { get; set; }

        public bool HasDispatch()
        {
            return DispatchService.HasValue && !string.IsNullOrWhiteSpace(DispatchText);
        }
    }
}
