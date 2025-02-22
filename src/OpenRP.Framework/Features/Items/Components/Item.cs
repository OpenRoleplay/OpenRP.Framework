using OpenRP.Framework.Database.Models;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Items.Components
{
    public class Item : Component
    {
        private ItemModel _itemModel;
        public Item(ItemModel itemModel) 
        { 
            _itemModel = itemModel;
        }
    }
}
