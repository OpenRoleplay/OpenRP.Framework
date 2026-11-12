using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Shared.BaseManager.Entities;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.Actors.Components
{
    /// <summary>
    /// One permission of one actor.
    /// </summary>
    public class ActorPermission : Component, IBaseDataComponent, IChangeable, IDeletable
    {
        private readonly ActorPermissionModel _model;
        private bool _hasChanges;
        private bool _isDeleted;

        public ActorPermission(ActorPermissionModel model)
        {
            _model = model;
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

        public ActorPermissionModel GetRawActorPermissionModel()
        {
            return _model;
        }

        public ulong GetId()
        {
            return _model.Id;
        }

        public ulong GetActorId()
        {
            return _model.ActorId;
        }

        public string GetPermission()
        {
            return _model.Permission;
        }
    }
}
