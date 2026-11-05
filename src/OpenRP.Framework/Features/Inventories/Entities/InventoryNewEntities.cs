using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Inventories.Entities
{
    public static class InventoryNewEntities
    {
        // Unsaved inventory items get entity ids counting down from the top, far away from database ids. The counter
        // is never reset: an id may still be in use by an item whose save hasn't happened yet.
        private static int NewInventoryItemId = int.MaxValue;

        public static int GenerateNewInventoryItemId()
        {
            return Interlocked.Decrement(ref NewInventoryItemId);
        }
    }
}
