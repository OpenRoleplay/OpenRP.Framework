using OpenRP.Framework.Database.Models;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs
{
    public class OnPlayerInventoryItemUsedEventArgs : ServerEventArgs
    {
        public Player? Player { get; set; }
        public InventoryItemModel? InventoryItem { get; set; }
    }
}
