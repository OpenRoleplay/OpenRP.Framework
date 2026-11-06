using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Shared.BaseManager.Entities;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.Factions.Components
{
    /// <summary>
    /// A permission granted to a faction. It only counts while the faction's parent faction has the permission too.
    /// </summary>
    public class FactionPermission : Component, IBaseDataComponent, IChangeable, IDeletable
    {
        private readonly FactionPermissionModel _factionPermissionModel;
        private bool _hasChanges;
        private bool _isDeleted;

        public FactionPermission(FactionPermissionModel factionPermissionModel)
        {
            _factionPermissionModel = factionPermissionModel;
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

        public FactionPermissionModel GetRawFactionPermissionModel()
        {
            return _factionPermissionModel;
        }

        public ulong GetId()
        {
            return _factionPermissionModel.Id;
        }

        public ulong GetFactionId()
        {
            return _factionPermissionModel.FactionId;
        }

        public ulong GetPermissionId()
        {
            return _factionPermissionModel.PermissionId;
        }
    }
}
