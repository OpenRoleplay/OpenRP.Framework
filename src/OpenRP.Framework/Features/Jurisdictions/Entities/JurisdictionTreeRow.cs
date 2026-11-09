using OpenRP.Framework.Features.Jurisdictions.Components;

namespace OpenRP.Framework.Features.Jurisdictions.Entities
{
    /// <summary>
    /// A jurisdiction in the tree, with how deep it lies: 0 for a country, 1 for a place in it, and so on.
    /// </summary>
    public record JurisdictionTreeRow(Jurisdiction Jurisdiction, int Depth);
}
