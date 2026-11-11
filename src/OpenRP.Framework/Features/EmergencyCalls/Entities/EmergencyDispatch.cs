using OpenRP.Framework.Features.EmergencyCalls.Enums;

namespace OpenRP.Framework.Features.EmergencyCalls.Entities
{
    /// <summary>
    /// One radio broadcast to one service's units.
    /// </summary>
    public sealed record EmergencyDispatch(EmergencyServiceType Service, string Text);
}
