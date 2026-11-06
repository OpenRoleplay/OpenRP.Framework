using OpenRP.Framework.Features.Factions.Components;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Factions.Services
{
    public interface IFactionService
    {
        /// <summary>
        /// The loaded faction with this id, or null.
        /// </summary>
        Faction? GetFaction(ulong factionId);

        /// <summary>
        /// The faction this one belongs to, or null for a top-level faction or a parent that isn't loaded.
        /// </summary>
        Faction? GetParentFaction(Faction faction);

        /// <summary>
        /// The loaded factions that belong directly to this one.
        /// </summary>
        List<Faction> GetChildFactions(Faction faction);
    }
}
