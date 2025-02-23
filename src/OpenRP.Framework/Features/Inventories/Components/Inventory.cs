using OpenRP.Framework.Database.Models;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Inventories.Components
{
    public class Inventory : Component
    {
        private InventoryModel _inventoryModel;
        private bool _scheduledForSaving;
        public Inventory(InventoryModel inventoryModel)
        {
            _inventoryModel = inventoryModel;
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
            return _inventoryModel.Id;
        }

        public uint? GetMaxWeight()
        {
            return _inventoryModel.MaxWeight;
        }
    }
}
