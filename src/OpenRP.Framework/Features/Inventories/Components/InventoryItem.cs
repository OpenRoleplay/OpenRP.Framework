using OpenRP.Framework.Database.Models;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Inventories.Components
{
    public class InventoryItem : Component
    {
        private readonly InventoryItemModel _inventoryItemModel;
        private bool _scheduledForSaving;
        public InventoryItem(InventoryItemModel inventoryItemModel)
        {
            _inventoryItemModel = inventoryItemModel;
        }

        public bool IsScheduledForSaving()
        {
            return _scheduledForSaving;
        }

        public void ScheduleForSaving(bool save = true)
        {
            _scheduledForSaving = save;
        }

        public ulong GetId()
        {
            return _inventoryItemModel.Id;
        }
    }
}
