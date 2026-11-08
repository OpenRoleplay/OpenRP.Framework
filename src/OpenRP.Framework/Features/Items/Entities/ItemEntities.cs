using OpenRP.Framework.Shared.BaseManager.Entities;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Items.Entities
{
    public static class ItemEntities
    {
        public static readonly Guid ItemType = new Guid("45413CA1-F39D-47EB-B067-E93435C114E6");

        public static EntityId GetItemId(int itemId)
        {
            return RowEntityIds.Get(ItemType, itemId);
        }
    }
}
