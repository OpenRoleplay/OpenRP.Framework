﻿using OpenRP.Framework.Features.Inventories.Components;
using OpenRP.Framework.Features.Inventories.Enums;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Inventories.Services.Dialogs
{
    public interface IInventoryDialogService
    {
        void OpenInventory(Player player, Inventory inventory, InventoryArguments inventoryArguments = InventoryArguments.None);
    }
}
