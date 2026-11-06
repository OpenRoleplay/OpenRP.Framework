using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Shared.BaseManager.Entities;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.Factions.Components
{
    /// <summary>
    /// A character's membership of a faction. Loaded for every member, online or not.
    /// </summary>
    public class FactionMember : Component, IBaseDataComponent, IChangeable, IDeletable
    {
        private readonly FactionMemberModel _factionMemberModel;
        private bool _hasChanges;
        private bool _isDeleted;

        public FactionMember(FactionMemberModel factionMemberModel)
        {
            _factionMemberModel = factionMemberModel;
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

        public FactionMemberModel GetRawFactionMemberModel()
        {
            return _factionMemberModel;
        }

        public ulong GetId()
        {
            return _factionMemberModel.Id;
        }

        public ulong GetCharacterId()
        {
            return _factionMemberModel.CharacterId;
        }

        public ulong GetFactionId()
        {
            return _factionMemberModel.FactionId;
        }

        public ulong GetRankId()
        {
            return _factionMemberModel.FactionRankId;
        }

        public void SetRankId(ulong rankId)
        {
            _factionMemberModel.FactionRankId = rankId;
            ProcessChanges();
        }

        /// <summary>
        /// True when this is the character's chosen active faction. With only one membership, use that one regardless.
        /// </summary>
        public bool IsActive()
        {
            return _factionMemberModel.IsActive;
        }

        public void SetActive(bool isActive)
        {
            if (_factionMemberModel.IsActive == isActive)
            {
                return;
            }

            _factionMemberModel.IsActive = isActive;
            ProcessChanges();
        }
    }
}
