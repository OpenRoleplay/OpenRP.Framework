using SampSharp.Entities;
using System;
using System.Threading;

namespace OpenRP.Framework.Shared.BaseManager.Entities
{
    /// <summary>
    /// Entity ids for components whose row isn't saved yet.
    /// </summary>
    /// <remarks>
    /// A saved component's entity id is its row id (for example InventoryEntities.GetInventoryItemId(row id)), so an
    /// unsaved one can't use that entity type: any number it picked could belong to a loaded row. Unsaved components
    /// get this separate type instead, so the two can never clash. BaseManager moves them to their real entity id on
    /// their first save. The counter is never reset, because an unsaved component may still be using its id.
    /// </remarks>
    public static class UnsavedEntities
    {
        [EntityType]
        public static readonly Guid UnsavedType = new Guid("C7A41E9B-2D58-4F03-8B6E-91D4F2A7305C");

        private static int UnsavedId = 0;

        public static EntityId GenerateUnsavedId()
        {
            return new EntityId(UnsavedType, Interlocked.Increment(ref UnsavedId));
        }
    }
}
