using System.Collections.Generic;

namespace OpenRP.Framework.Features.Factions.Entities
{
    /// <summary>
    /// The permissions every faction can give to its ranks. Leaders have all of them.
    /// </summary>
    public static class FactionPermissions
    {
        public const string Invite = "faction.invite";
        public const string Remove = "faction.remove";
        public const string SetRank = "faction.setrank";
        public const string ManageRanks = "faction.manageranks";
        public const string Bank = "faction.bank";

        /// <summary>
        /// Every faction permission with the description shown in the rank permissions menu.
        /// </summary>
        public static IReadOnlyDictionary<string, string> GetAll()
        {
            return new Dictionary<string, string>
            {
                { Invite, "Invite players to the faction" },
                { Remove, "Remove members from the faction" },
                { SetRank, "Change the rank of members" },
                { ManageRanks, "Create, rename, move and delete ranks" },
                { Bank, "Use the faction's bank accounts and open new ones" },
            };
        }
    }
}
