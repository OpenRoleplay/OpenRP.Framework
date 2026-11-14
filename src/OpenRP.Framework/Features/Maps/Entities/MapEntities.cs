using OpenRP.Framework.Shared.BaseManager.Entities;
using SampSharp.Entities;
using System;

namespace OpenRP.Framework.Features.Maps.Entities
{
    public static class MapEntities
    {
        // One type key per table; generate a fresh Guid for a new table and never reuse one.
        public static readonly Guid MapType = new Guid("DF03BCC2-2D1F-4B14-89F8-F7162E8E11F0");

        public static readonly Guid MapObjectType = new Guid("55F70D50-A4C6-47F9-B0AD-CFD15822439C");

        public static readonly Guid MapObjectMaterialEntityType = new Guid("6815B5ED-CB08-46D4-B075-60F2AC6B4D7A");

        public static readonly Guid MapRemovedBuildingType = new Guid("1A822070-B3DD-4C66-98B0-19D0CF36B713");

        public static EntityId GetMapId(int mapId)
        {
            return RowEntityIds.Get(MapType, mapId);
        }

        public static EntityId GetMapObjectId(int mapObjectId)
        {
            return RowEntityIds.Get(MapObjectType, mapObjectId);
        }
    }
}
