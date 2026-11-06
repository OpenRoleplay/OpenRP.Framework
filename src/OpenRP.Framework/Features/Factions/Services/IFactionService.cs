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

        /// <summary>
        /// The highest loaded faction above this one, or the faction itself when it's at the top.
        /// </summary>
        Faction GetTopFaction(Faction faction);

        /// <summary>
        /// The government this faction falls under: the highest government among the faction and the factions above
        /// it. Null when it isn't under a government, for example a mafia or a corporation.
        /// </summary>
        Faction? GetGovernment(Faction faction);

        /// <summary>
        /// True when the controller is this faction itself or one of the factions above it.
        /// </summary>
        bool IsControlledBy(Faction faction, Faction controller);
    }
}
