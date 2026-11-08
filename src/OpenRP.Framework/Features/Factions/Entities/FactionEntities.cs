using OpenRP.Framework.Shared.BaseManager.Entities;
using SampSharp.Entities;
using System;

namespace OpenRP.Framework.Features.Factions.Entities
{
    public static class FactionEntities
    {
        public static readonly Guid FactionType = new Guid("ACDFB33C-E5B5-40F6-90FE-A83DA1CBE7FD");

        public static readonly Guid FactionRankType = new Guid("4D0A94AC-0653-481F-BFB9-EB3273630330");

        public static readonly Guid FactionMemberType = new Guid("0D374107-16A1-4160-A37F-7028481E2EF7");

        public static readonly Guid FactionRankPermissionType = new Guid("E75B6CE7-7E98-46E8-AA68-9A926E2D0248");

        public static readonly Guid FactionPermissionType = new Guid("E1C09953-186E-475A-BBC3-308B22195DD4");

        public static EntityId GetFactionId(int factionId)
        {
            return RowEntityIds.Get(FactionType, factionId);
        }

        public static EntityId GetFactionRankId(int factionRankId)
        {
            return RowEntityIds.Get(FactionRankType, factionRankId);
        }

        public static EntityId GetFactionMemberId(int factionMemberId)
        {
            return RowEntityIds.Get(FactionMemberType, factionMemberId);
        }

        public static EntityId GetFactionRankPermissionId(int factionRankPermissionId)
        {
            return RowEntityIds.Get(FactionRankPermissionType, factionRankPermissionId);
        }

        public static EntityId GetFactionPermissionId(int factionPermissionId)
        {
            return RowEntityIds.Get(FactionPermissionType, factionPermissionId);
        }
    }
}
