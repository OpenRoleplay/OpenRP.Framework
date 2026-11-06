using SampSharp.Entities;
using System;

namespace OpenRP.Framework.Features.Factions.Entities
{
    public static class FactionEntities
    {
        [EntityType]
        public static readonly Guid FactionType = new Guid("ACDFB33C-E5B5-40F6-90FE-A83DA1CBE7FD");

        [EntityType]
        public static readonly Guid FactionRankType = new Guid("4D0A94AC-0653-481F-BFB9-EB3273630330");

        [EntityType]
        public static readonly Guid FactionMemberType = new Guid("0D374107-16A1-4160-A37F-7028481E2EF7");

        [EntityType]
        public static readonly Guid FactionRankPermissionType = new Guid("E75B6CE7-7E98-46E8-AA68-9A926E2D0248");

        [EntityType]
        public static readonly Guid FactionPermissionType = new Guid("E1C09953-186E-475A-BBC3-308B22195DD4");

        public static EntityId GetFactionId(int factionId)
        {
            return new EntityId(FactionType, factionId);
        }

        public static EntityId GetFactionRankId(int factionRankId)
        {
            return new EntityId(FactionRankType, factionRankId);
        }

        public static EntityId GetFactionMemberId(int factionMemberId)
        {
            return new EntityId(FactionMemberType, factionMemberId);
        }

        public static EntityId GetFactionRankPermissionId(int factionRankPermissionId)
        {
            return new EntityId(FactionRankPermissionType, factionRankPermissionId);
        }

        public static EntityId GetFactionPermissionId(int factionPermissionId)
        {
            return new EntityId(FactionPermissionType, factionPermissionId);
        }
    }
}
