using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Shared.BaseManager.Entities;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.Factions.Components
{
    /// <summary>
    /// A faction, which can belong to a parent faction (for example a division of a police department).
    /// </summary>
    public class Faction : Component, IBaseDataComponent, IChangeable
    {
        private readonly FactionModel _factionModel;
        private bool _hasChanges;

        public Faction(FactionModel factionModel)
        {
            _factionModel = factionModel;
        }

        public bool HasChanges()
        {
            return _hasChanges;
        }

        public void ProcessChanges(bool processChanges = true)
        {
            _hasChanges = processChanges;
        }

        public FactionModel GetRawFactionModel()
        {
            return _factionModel;
        }

        public ulong GetId()
        {
            return _factionModel.Id;
        }

        public string GetName()
        {
            return _factionModel.Name;
        }

        /// <summary>
        /// The id of the faction this one belongs to, or null for a top-level faction.
        /// </summary>
        public ulong? GetParentFactionId()
        {
            return _factionModel.ParentFactionId;
        }

        public bool HasParentFaction()
        {
            return _factionModel.ParentFactionId.HasValue;
        }
    }
}
