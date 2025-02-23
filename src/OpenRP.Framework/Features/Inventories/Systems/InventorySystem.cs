﻿using OpenRP.Framework.Features.Inventories.Services;
using OpenRP.Framework.Features.Items.Services;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Inventories.Systems
{
    public class InventorySystem : ISystem
    {
        [Event]
        public void OnGameModeInit(IInventoryManager inventoryManager, IInventoryItemManager inventoryItemManager)
        {
            inventoryManager.LoadInventories();
            inventoryItemManager.LoadInventoryItems();
        }

        [Timer(60000)]
        public void ProcessChanges(IInventoryManager inventoryManager, IInventoryItemManager inventoryItemManager)
        {
            inventoryManager.ProcessChanges();
            inventoryItemManager.ProcessChanges();
        }
    }
}
