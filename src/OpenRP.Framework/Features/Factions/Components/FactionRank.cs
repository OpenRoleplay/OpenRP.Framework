using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Shared.BaseManager.Entities;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.Factions.Components
{
    /// <summary>
    /// A rank in a faction. The rank without a parent rank is the faction's top (leader) rank.
    /// </summary>
    public class FactionRank : Component, IBaseDataComponent, IChangeable, IDeletable
    {
        private readonly FactionRankModel _factionRankModel;
        private bool _hasChanges;
        private bool _isDeleted;

        public FactionRank(FactionRankModel factionRankModel)
        {
            _factionRankModel = factionRankModel;
        }

        public bool HasChanges()
        {
            return _hasChanges;
        }

        public void ProcessChanges(bool processChanges = true)
        {
            _hasChanges = processChanges;
        }

        public bool IsDeleted()
        {
            return _isDeleted;
        }

        public void ProcessDeletion(bool processDeletion = true)
        {
            _isDeleted = processDeletion;
        }

        public FactionRankModel GetRawFactionRankModel()
        {
            return _factionRankModel;
        }

        public ulong GetId()
        {
            return _factionRankModel.Id;
        }

        public ulong GetFactionId()
        {
            return _factionRankModel.FactionId;
        }

        /// <summary>
        /// The id of the rank directly above this one, or null for the top rank.
        /// </summary>
        public ulong? GetParentRankId()
        {
            return _factionRankModel.ParentRankId;
        }

        public bool IsTopRank()
        {
            return !_factionRankModel.ParentRankId.HasValue;
        }

        public void SetParentRankId(ulong parentRankId)
        {
            _factionRankModel.ParentRankId = parentRankId;
            ProcessChanges();
        }

        public string GetName()
        {
            return _factionRankModel.Name;
        }

        public void SetName(string name)
        {
            _factionRankModel.Name = name;
            ProcessChanges();
        }
    }
}
