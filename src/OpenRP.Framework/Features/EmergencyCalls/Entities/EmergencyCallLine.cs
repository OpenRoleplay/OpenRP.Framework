namespace OpenRP.Framework.Features.EmergencyCalls.Entities
{
    /// <summary>
    /// One line of a 911 call: said by the caller, or by the dispatcher in the reply format ("SAY: ..." or
    /// "DISPATCH: ...").
    /// </summary>
    public record EmergencyCallLine(bool IsDispatcher, string Text);
}
