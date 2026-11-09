using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Features.Jurisdictions.Components;
using OpenRP.Framework.Features.Jurisdictions.Entities;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Jurisdictions.Services
{
    /// <summary>
    /// Reading jurisdictions: the tree, who governs them, their currency, plate tags and tax registrations.
    /// </summary>
    public interface IJurisdictionService
    {
        /// <summary>
        /// Every loaded jurisdiction that isn't deleted, by name.
        /// </summary>
        List<Jurisdiction> GetJurisdictions();

        /// <summary>
        /// The loaded jurisdiction with this id, or null when it doesn't exist or is deleted.
        /// </summary>
        Jurisdiction? GetJurisdiction(ulong jurisdictionId);

        /// <summary>
        /// The jurisdiction this one lies in, or null for a country or a parent that isn't loaded.
        /// </summary>
        Jurisdiction? GetParentJurisdiction(Jurisdiction jurisdiction);

        /// <summary>
        /// The jurisdictions that lie directly in this one, by name.
        /// </summary>
        List<Jurisdiction> GetChildJurisdictions(Jurisdiction jurisdiction);

        /// <summary>
        /// Every jurisdiction above this one, from its parent up to its country.
        /// </summary>
        List<Jurisdiction> GetAncestors(Jurisdiction jurisdiction);

        /// <summary>
        /// The country this jurisdiction lies in, or the jurisdiction itself when it's a country.
        /// </summary>
        Jurisdiction GetCountry(Jurisdiction jurisdiction);

        /// <summary>
        /// Every jurisdiction in tree order: each country followed by the places in it, by name at every level.
        /// </summary>
        List<JurisdictionTreeRow> GetJurisdictionTree();

        /// <summary>
        /// The faction that governs the jurisdiction: its own government, otherwise its nearest parent's. Null when
        /// none of them has one.
        /// </summary>
        JurisdictionGovernment? GetGovernment(Jurisdiction jurisdiction);

        /// <summary>
        /// The currency used in the jurisdiction: its own, otherwise its nearest parent's. Null when none has one.
        /// </summary>
        ulong? GetCurrencyId(Jurisdiction jurisdiction);

        /// <summary>
        /// The jurisdictions this faction governs, its own and the ones that inherit its government.
        /// </summary>
        List<Jurisdiction> GetJurisdictionsGovernedBy(Faction faction);

        /// <summary>
        /// The tax registrations of this jurisdiction.
        /// </summary>
        List<FactionTaxRegistration> GetTaxRegistrations(Jurisdiction jurisdiction);

        /// <summary>
        /// Where the faction pays taxes: its own registrations, otherwise its nearest parent faction's.
        /// </summary>
        List<Jurisdiction> GetTaxJurisdictions(Faction faction);

        /// <summary>
        /// The tag a licence plate starts with: the two-letter tag of the jurisdiction's country, or SA when there's
        /// no jurisdiction or it isn't loaded.
        /// </summary>
        string GetPlateTag(ulong? jurisdictionId);
    }
}
