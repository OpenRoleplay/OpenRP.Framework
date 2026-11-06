using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Commands.Helpers;
using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Features.Factions.Entities;
using OpenRP.Framework.Features.Permissions.Entities;
using OpenRP.Framework.Features.Permissions.Services;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;
using OpenRP.Framework.Shared.ServerEvents.Services;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Factions.Services
{
    public class FactionPermissionService : IFactionPermissionService, ICharacterPermissionSource
    {
        private const string DefaultPermissionGroupName = "Faction";

        private readonly IEntityManager _entityManager;
        private readonly IFactionService _factionService;
        private readonly IFactionRankService _factionRankService;
        private readonly IFactionPermissionManager _factionPermissionManager;
        private readonly IFactionRankPermissionManager _factionRankPermissionManager;
        private readonly IServerEventAggregator _serverEventAggregator;
        private readonly ILogger<FactionPermissionService> _logger;

        private readonly Dictionary<string, ulong> _permissionIdsByName = new Dictionary<string, ulong>();
        private readonly Dictionary<ulong, string> _permissionNamesById = new Dictionary<ulong, string>();
        private readonly Dictionary<string, string> _permissionDescriptionsByName = new Dictionary<string, string>();
        private readonly HashSet<ulong> _defaultPermissionIds = new HashSet<ulong>();

        public FactionPermissionService(IEntityManager entityManager, IFactionService factionService, IFactionRankService factionRankService, IFactionPermissionManager factionPermissionManager, IFactionRankPermissionManager factionRankPermissionManager, IServerEventAggregator serverEventAggregator, ILogger<FactionPermissionService> logger)
        {
            _entityManager = entityManager;
            _factionService = factionService;
            _factionRankService = factionRankService;
            _factionPermissionManager = factionPermissionManager;
            _factionRankPermissionManager = factionRankPermissionManager;
            _serverEventAggregator = serverEventAggregator;
            _logger = logger;
        }

        public void Initialize(IPermissionService permissionService)
        {
            foreach ((string name, string description) in GetBuiltInDefaultPermissions())
            {
                PermissionModel permission = permissionService.CreatePermissionIfNotExists(name, description);
                permissionService.AddPermissionToGroupIfMissing(permission.Id, DefaultPermissionGroupName);
            }

            foreach (PermissionModel permission in permissionService.GetAllPermissions())
            {
                _permissionIdsByName[permission.Name] = permission.Id;
                _permissionNamesById[permission.Id] = permission.Name;
                if (!string.IsNullOrWhiteSpace(permission.Description))
                {
                    _permissionDescriptionsByName[permission.Name] = permission.Description;
                }
            }

            _defaultPermissionIds.UnionWith(permissionService.GetPermissionGroupPermissionIds(DefaultPermissionGroupName));
        }

        /// <summary>
        /// The faction.* permissions and the commands that opt into the "Faction" group. They're created and linked here
        /// too, because the permission system links command permissions in its own startup handler, which may run after
        /// this one; otherwise a new faction command would only become a default after a second restart.
        /// </summary>
        private static IEnumerable<(string Name, string Description)> GetBuiltInDefaultPermissions()
        {
            IEnumerable<(string Name, string Description)> factionPermissions = FactionPermissions.GetAll().Select(entry => (entry.Key, entry.Value));
            IEnumerable<(string Name, string Description)> factionCommands = ServerCommandCache.CachedServerCommands
                .Where(command => command.Attribute.PermissionGroups.Contains(DefaultPermissionGroupName))
                .Select(command => command.Name.ToLower())
                .Distinct()
                .Select(commandName => ($"cmd.{commandName}", $"This permission grants the ability to use the /{commandName} command."));
            return factionPermissions.Concat(factionCommands);
        }

        public bool IsKnownPermission(string permission)
        {
            return _permissionIdsByName.ContainsKey(permission);
        }

        public bool IsDefaultPermission(string permission)
        {
            return _permissionIdsByName.TryGetValue(permission, out ulong permissionId) && _defaultPermissionIds.Contains(permissionId);
        }

        public string GetPermissionDescription(string permission)
        {
            return _permissionDescriptionsByName.TryGetValue(permission, out string? description) ? description : permission;
        }

        public List<string> GetFactionPermissions(Faction faction)
        {
            return GetNames(GetFactionPermissionIds(faction));
        }

        public bool FactionHasPermission(Faction faction, string permission)
        {
            return _permissionIdsByName.TryGetValue(permission, out ulong permissionId) && GetFactionPermissionIds(faction).Contains(permissionId);
        }

        public List<string> GetRankPermissions(FactionRank rank)
        {
            return GetNames(GetRankPermissionIds(rank));
        }

        public bool RankHasPermission(FactionRank rank, string permission)
        {
            return _permissionIdsByName.TryGetValue(permission, out ulong permissionId) && GetRankPermissionIds(rank).Contains(permissionId);
        }

        public bool IsGrantedToFaction(Faction faction, string permission)
        {
            return _permissionIdsByName.TryGetValue(permission, out ulong permissionId) && GetFactionGrantIds(faction).Contains(permissionId);
        }

        public bool IsGrantedToRank(FactionRank rank, string permission)
        {
            return _permissionIdsByName.TryGetValue(permission, out ulong permissionId) && GetRankGrantIds(rank).Contains(permissionId);
        }

        public List<string> GetInactiveFactionGrants(Faction faction)
        {
            HashSet<ulong> inactiveGrantIds = GetFactionGrantIds(faction);
            inactiveGrantIds.ExceptWith(GetFactionPermissionIds(faction));
            return GetNames(inactiveGrantIds);
        }

        public List<string> GetInactiveRankGrants(FactionRank rank)
        {
            HashSet<ulong> inactiveGrantIds = GetRankGrantIds(rank);
            inactiveGrantIds.ExceptWith(GetRankPermissionIds(rank));
            return GetNames(inactiveGrantIds);
        }

        public void SetFactionGrant(Faction faction, string permission, bool granted)
        {
            if (!TryGetPermissionId(permission, out ulong permissionId))
            {
                return;
            }

            // A grant removed and given again before the next sync still has its row, so that row is reused.
            FactionPermission? factionPermission = _entityManager.GetComponents<FactionPermission>()
                .FirstOrDefault(grant => grant.GetFactionId() == faction.GetId() && grant.GetPermissionId() == permissionId);
            if (factionPermission != null)
                factionPermission.ProcessDeletion(!granted);
            else if (granted)
                _factionPermissionManager.CreateAndLoad(new FactionPermissionModel { FactionId = faction.GetId(), PermissionId = permissionId });

            NotifyPermissionsChanged(faction);
        }

        public void SetRankGrant(FactionRank rank, string permission, bool granted)
        {
            if (!TryGetPermissionId(permission, out ulong permissionId))
            {
                return;
            }

            FactionRankPermission? rankPermission = _entityManager.GetComponents<FactionRankPermission>()
                .FirstOrDefault(grant => grant.GetRankId() == rank.GetId() && grant.GetPermissionId() == permissionId);
            if (rankPermission != null)
                rankPermission.ProcessDeletion(!granted);
            else if (granted)
                _factionRankPermissionManager.CreateAndLoad(new FactionRankPermissionModel { FactionRankId = rank.GetId(), PermissionId = permissionId });

            Faction? faction = _factionService.GetFaction(rank.GetFactionId());
            if (faction != null)
            {
                NotifyPermissionsChanged(faction);
            }
        }

        public void NotifyPermissionsChanged(Faction faction, params ulong[] alsoCharacterIds)
        {
            HashSet<ulong> factionIds = GetFactionAndSubFactionIds(faction);
            List<ulong> characterIds = _entityManager.GetComponents<FactionMember>()
                .Where(member => factionIds.Contains(member.GetFactionId()))
                .Select(member => member.GetCharacterId())
                .Concat(alsoCharacterIds)
                .Distinct()
                .ToList();

            PublishPermissionsChanged(faction.GetId(), characterIds);
        }

        /// <summary>
        /// Every permission all of the character's ranks have. Feeds the character's command permissions.
        /// </summary>
        public IEnumerable<string> GetPermissions(ulong characterId)
        {
            return _entityManager.GetComponents<FactionMember>()
                .Where(member => member.GetCharacterId() == characterId && !member.IsDeleted())
                .Select(member => _factionRankService.GetRank(member.GetRankId()))
                .Where(rank => rank != null)
                .SelectMany(rank => GetRankPermissions(rank!))
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// The defaults, plus the faction's grants. A sub-faction's grants only count while its parent has them.
        /// </summary>
        private HashSet<ulong> GetFactionPermissionIds(Faction faction)
        {
            return GetFactionPermissionIds(faction, new HashSet<ulong>());
        }

        private HashSet<ulong> GetFactionPermissionIds(Faction faction, HashSet<ulong> visitedFactionIds)
        {
            visitedFactionIds.Add(faction.GetId());
            HashSet<ulong> grantIds = GetFactionGrantIds(faction);

            Faction? parent = _factionService.GetParentFaction(faction);
            if (parent != null && !visitedFactionIds.Contains(parent.GetId()))
            {
                grantIds.IntersectWith(GetFactionPermissionIds(parent, visitedFactionIds));
            }
            else if (parent != null)
            {
                // A loop in the data (A above B above A) would otherwise never end; the faction is treated as top-level.
                _logger.LogWarning("Faction {FactionId} is part of a loop of parent factions; check ParentFactionId in the database.", faction.GetId());
            }

            grantIds.UnionWith(_defaultPermissionIds);
            return grantIds;
        }

        /// <summary>
        /// The top rank has everything its faction has; other ranks have their grants, as far as the faction has them.
        /// </summary>
        private HashSet<ulong> GetRankPermissionIds(FactionRank rank)
        {
            Faction? faction = _factionService.GetFaction(rank.GetFactionId());
            if (faction == null)
            {
                return new HashSet<ulong>();
            }

            HashSet<ulong> factionPermissionIds = GetFactionPermissionIds(faction);
            if (rank.IsTopRank())
            {
                return factionPermissionIds;
            }

            HashSet<ulong> rankPermissionIds = GetRankGrantIds(rank);
            rankPermissionIds.IntersectWith(factionPermissionIds);
            return rankPermissionIds;
        }

        private HashSet<ulong> GetFactionGrantIds(Faction faction)
        {
            return _entityManager.GetComponents<FactionPermission>()
                .Where(grant => grant.GetFactionId() == faction.GetId() && !grant.IsDeleted())
                .Select(grant => grant.GetPermissionId())
                .ToHashSet();
        }

        private HashSet<ulong> GetRankGrantIds(FactionRank rank)
        {
            return _entityManager.GetComponents<FactionRankPermission>()
                .Where(grant => grant.GetRankId() == rank.GetId() && !grant.IsDeleted())
                .Select(grant => grant.GetPermissionId())
                .ToHashSet();
        }

        private HashSet<ulong> GetFactionAndSubFactionIds(Faction faction)
        {
            HashSet<ulong> factionIds = new HashSet<ulong> { faction.GetId() };
            Queue<Faction> factionsToVisit = new Queue<Faction>(new[] { faction });
            while (factionsToVisit.Count > 0)
            {
                foreach (Faction subFaction in _factionService.GetChildFactions(factionsToVisit.Dequeue()).Where(subFaction => factionIds.Add(subFaction.GetId())))
                {
                    factionsToVisit.Enqueue(subFaction);
                }
            }
            return factionIds;
        }

        private List<string> GetNames(IEnumerable<ulong> permissionIds)
        {
            return permissionIds
                .Where(_permissionNamesById.ContainsKey)
                .Select(permissionId => _permissionNamesById[permissionId])
                .OrderBy(name => name)
                .ToList();
        }

        private bool TryGetPermissionId(string permission, out ulong permissionId)
        {
            if (_permissionIdsByName.TryGetValue(permission, out permissionId))
            {
                return true;
            }

            _logger.LogError("Permission {Permission} isn't known; it may have been added after the server started.", permission);
            return false;
        }

        private async void PublishPermissionsChanged(ulong factionId, List<ulong> characterIds)
        {
            try
            {
                await _serverEventAggregator.PublishAsync(new OnFactionPermissionsChangedEventArgs { FactionId = factionId, CharacterIds = characterIds });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish that the permissions of faction {FactionId} changed.", factionId);
            }
        }
    }
}
