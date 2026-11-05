using OpenRP.Framework.Features.Inventories.Services;
using OpenRP.Framework.Features.Items.Services;
using OpenRP.Framework.Shared.BaseManager.Helpers;
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
            inventoryManager.LoadAll();
            inventoryItemManager.LoadAll();
        }

        private const int SyncIntervalInMilliseconds = 60000;

        [Timer(SyncIntervalInMilliseconds)]
        public void ProcessChanges(IInventoryManager inventoryManager, IInventoryItemManager inventoryItemManager)
        {
            inventoryManager.ProcessChanges();
            inventoryItemManager.ProcessChanges();
        }

        /// <summary>
        /// Saves what changed since the last sync, so a restart doesn't lose up to a minute of inventory changes.
        /// Inventories go first because items belong to them.
        /// </summary>
        [Event]
        public void OnGameModeExit(IInventoryManager inventoryManager, IInventoryItemManager inventoryItemManager)
        {
            inventoryManager.SaveNow();
            inventoryItemManager.SaveNow();
        }
    }
}
