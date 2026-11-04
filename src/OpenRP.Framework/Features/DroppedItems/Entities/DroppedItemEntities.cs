using SampSharp.Entities;
using System;

namespace OpenRP.Framework.Features.DroppedItems.Entities
{
    public static class DroppedItemEntities
    {
        // Moved from the gamemode. It is the same entity type, so it keeps its Guid.
        [EntityType]
        public static readonly Guid DroppedItemType = new Guid("CDFF3D53-E1C7-4E1F-BD04-C5F41C34C32C");

        public static EntityId GetDroppedItemId(int droppedItemId)
        {
            return new EntityId(DroppedItemType, droppedItemId);
        }
    }
}
