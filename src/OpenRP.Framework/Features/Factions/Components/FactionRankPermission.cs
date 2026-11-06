using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Shared.BaseManager.Entities;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.Factions.Components
{
    /// <summary>
    /// A permission given to everyone with a faction rank.
    /// </summary>
    public class FactionRankPermission : Component, IBaseDataComponent, IChangeable, IDeletable
    {
        private readonly FactionRankPermissionModel _factionRankPermissionModel;
        private bool _hasChanges;
        private bool _isDeleted;

        public FactionRankPermission(FactionRankPermissionModel factionRankPermissionModel)
        {
            _factionRankPermissionModel = factionRankPermissionModel;
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

        public FactionRankPermissionModel GetRawFactionRankPermissionModel()
        {
            return _factionRankPermissionModel;
        }

        public ulong GetId()
        {
            return _factionRankPermissionModel.Id;
        }

        public ulong GetRankId()
        {
            return _factionRankPermissionModel.FactionRankId;
        }

        public ulong GetPermissionId()
        {
            return _factionRankPermissionModel.PermissionId;
        }
    }
}
