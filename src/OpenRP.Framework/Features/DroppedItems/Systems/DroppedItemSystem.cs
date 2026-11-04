using OpenRP.Framework.Features.DroppedItems.Services;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.DroppedItems.Systems
{
    public class DroppedItemSystem : ISystem
    {
        [Event]
        public void OnGameModeInit(IDroppedItemService droppedItemService)
        {
            droppedItemService.LoadDroppedItems();
        }
    }
}
