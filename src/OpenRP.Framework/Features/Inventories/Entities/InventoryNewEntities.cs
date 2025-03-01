using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Inventories.Entities
{
    public static class InventoryNewEntities
    {
        private static int NewInventoryItemId = int.MaxValue;
        private static int NewInventoryId = int.MaxValue;

        public static void ResetNewInventoryItemId()
        {
            NewInventoryItemId = int.MaxValue;
        }

        public static void ResetNewInventoryId()
        {
            NewInventoryId = int.MaxValue;
        }

        public static int GenerateNewInventoryItemId()
        {
            // Atomically decrement the counter to ensure thread-safety.
            return Interlocked.Decrement(ref NewInventoryItemId);
        }

        public static int GenerateNewInventoryId()
        {
            // Atomically decrement the counter to ensure thread-safety.
            return Interlocked.Decrement(ref NewInventoryId);
        }
    }
}
