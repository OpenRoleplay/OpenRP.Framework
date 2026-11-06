using OpenRP.Framework.Features.Factions.Services;
using OpenRP.Framework.Features.Permissions.Services;
using OpenRP.Framework.Shared.BaseManager.Helpers;
using SampSharp.Entities;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Factions.Systems
{
    public class FactionSystem : ISystem
    {
        private const int SyncIntervalInMilliseconds = 60000;

        // Parents first: faction permissions, ranks and members are child entities of their faction, rank permissions of their rank.
        private readonly List<IBaseManager> _managersInLoadOrder;

        public FactionSystem(IFactionManager factionManager, IFactionPermissionManager factionPermissionManager, IFactionRankManager factionRankManager, IFactionRankPermissionManager factionRankPermissionManager, IFactionMemberManager factionMemberManager)
        {
            _managersInLoadOrder = new List<IBaseManager> { factionManager, factionPermissionManager, factionRankManager, factionRankPermissionManager, factionMemberManager };
        }

        [Event]
        public void OnGameModeInit(IFactionPermissionService factionPermissionService, IPermissionService permissionService)
        {
            factionPermissionService.Initialize(permissionService);
            _managersInLoadOrder.ForEach(manager => manager.LoadAll());
        }

        [Timer(SyncIntervalInMilliseconds)]
        public void ProcessChanges()
        {
            _managersInLoadOrder.ForEach(manager => manager.ProcessChanges());
        }

        [Event]
        public void OnGameModeExit()
        {
            _managersInLoadOrder.ForEach(manager => manager.SaveNow());
        }
    }
}
