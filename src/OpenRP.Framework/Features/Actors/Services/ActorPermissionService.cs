using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Actors.Components;
using OpenRP.Framework.Features.Actors.Entities;
using SampSharp.Entities;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Actors.Services
{
    public class ActorPermissionService : IActorPermissionService
    {
        private readonly IEntityManager _entityManager;
        private readonly List<ActorPermissionDefinition> _definitions;

        public ActorPermissionService(IEntityManager entityManager, IEnumerable<ActorPermissionDefinition> definitions)
        {
            _entityManager = entityManager;
            _definitions = definitions.ToList();
        }

        public IReadOnlyList<ActorPermissionDefinition> GetDefinitions()
        {
            return _definitions;
        }

        public bool HasPermission(ServerActor actor, string permission)
        {
            return GetPermissionRow(actor, permission) != null;
        }

        public void SetPermission(ServerActor actor, string permission, bool isGranted)
        {
            ActorPermission? row = GetPermissionRow(actor, permission);
            if (isGranted && row == null)
            {
                // Unsaved until the next sync; the manager then gives it its id.
                _entityManager.AddComponent<ActorPermission>(EntityId.NewEntityId(), new ActorPermissionModel { ActorId = actor.GetId(), Permission = permission });
            }
            else if (!isGranted && row != null)
            {
                row.ProcessDeletion();
            }
        }

        private ActorPermission? GetPermissionRow(ServerActor actor, string permission)
        {
            return _entityManager.GetComponents<ActorPermission>()
                .FirstOrDefault(row => !row.IsDeleted() && row.GetActorId() == actor.GetId() && row.GetPermission() == permission);
        }
    }
}
