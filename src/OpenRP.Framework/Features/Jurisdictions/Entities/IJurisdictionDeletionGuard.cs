using OpenRP.Framework.Features.Jurisdictions.Components;

namespace OpenRP.Framework.Features.Jurisdictions.Entities
{
    /// <summary>
    /// Lets another feature keep a jurisdiction from being deleted while something of its own depends on it, such as a
    /// bank. Register one with AddJurisdictionDeletionGuard&lt;T&gt;().
    /// </summary>
    public interface IJurisdictionDeletionGuard
    {
        /// <summary>
        /// Why the jurisdiction can't be deleted, or null when this feature doesn't mind.
        /// </summary>
        string? GetDeletionError(Jurisdiction jurisdiction);
    }
}
