using SampSharp.Entities;
using System;

namespace OpenRP.Framework.Features.Properties.Entities
{
    public static class PropertyEntities
    {
        [EntityType]
        public static readonly Guid PropertyType = new Guid("DAD676FC-DE30-4B52-BC9B-36AE61C060CB");

        [EntityType]
        public static readonly Guid PropertyDoorType = new Guid("77AE379F-111C-4959-A57D-5A881E96F7B1");

        public static EntityId GetPropertyId(int propertyId)
        {
            return new EntityId(PropertyType, propertyId);
        }

        public static EntityId GetPropertyDoorId(int propertyDoorId)
        {
            return new EntityId(PropertyDoorType, propertyDoorId);
        }
    }
}
