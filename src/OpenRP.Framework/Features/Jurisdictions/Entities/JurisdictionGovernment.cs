using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Features.Jurisdictions.Components;

namespace OpenRP.Framework.Features.Jurisdictions.Entities
{
    /// <summary>
    /// The faction that governs a jurisdiction, and the jurisdiction it got that from: the jurisdiction itself, or
    /// the nearest parent with a government of its own.
    /// </summary>
    public record JurisdictionGovernment(Faction Faction, Jurisdiction GovernedFrom);
}
