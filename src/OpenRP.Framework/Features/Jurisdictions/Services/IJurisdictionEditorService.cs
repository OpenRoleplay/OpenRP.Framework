using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Features.Jurisdictions.Components;

namespace OpenRP.Framework.Features.Jurisdictions.Services
{
    /// <summary>
    /// Creating, changing and deleting jurisdictions and tax registrations, for the admin tools. Changes are saved
    /// with the next jurisdiction sync. Methods that can fail return the reason, or null.
    /// </summary>
    public interface IJurisdictionEditorService
    {
        /// <summary>
        /// Creates a jurisdiction in the parent, or a country when the parent is null. The tags are the 2-, 3- and
        /// 4-letter tags separated by spaces, for example "SA SAN USSA". The jurisdiction is null when the input
        /// isn't valid.
        /// </summary>
        (Jurisdiction? Jurisdiction, string? Error) CreateJurisdiction(string name, string tags, Jurisdiction? parent);

        string? RenameJurisdiction(Jurisdiction jurisdiction, string name);

        /// <summary>
        /// Sets the 2-, 3- and 4-letter tags, given separated by spaces. Each must be letters only, the right length,
        /// and not used by another jurisdiction.
        /// </summary>
        string? SetTags(Jurisdiction jurisdiction, string tags);

        /// <summary>
        /// Gives the jurisdiction its own currency, or none (null) to use its parent's.
        /// </summary>
        void SetCurrency(Jurisdiction jurisdiction, ulong? currencyId);

        /// <summary>
        /// Moves the jurisdiction into another one, or makes it a country when the parent is null. Refused when the
        /// new parent is the jurisdiction itself or lies in it.
        /// </summary>
        string? MoveJurisdiction(Jurisdiction jurisdiction, Jurisdiction? parent);

        /// <summary>
        /// Gives the jurisdiction its own government, or none (null) to fall back to its parent's.
        /// </summary>
        void SetGovernment(Jurisdiction jurisdiction, Faction? faction);

        /// <summary>
        /// Registers the faction in the jurisdiction, so it pays taxes there. Refused when it's registered already.
        /// </summary>
        string? AddTaxRegistration(Jurisdiction jurisdiction, Faction faction);

        void RemoveTaxRegistration(FactionTaxRegistration registration);

        /// <summary>
        /// Deletes the jurisdiction and its tax registrations. Refused while other jurisdictions lie in it.
        /// </summary>
        string? DeleteJurisdiction(Jurisdiction jurisdiction);
    }
}
