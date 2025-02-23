using Microsoft.EntityFrameworkCore.Metadata;
using OpenRP.Framework.Features.Items.Services;
using OpenRP.Framework.Features.WorldWeather.Services;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Items.Systems
{
    public class ItemSystem : ISystem
    {
        [Event]
        public void OnGameModeInit(IItemManager itemManager)
        {
            itemManager.LoadItems();
        }

        [Timer(60000)]
        public void ProcessChanges(IItemManager itemManager)
        {
            itemManager.ProcessChanges();
        }
    }
}
