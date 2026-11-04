using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Inventories.Helpers;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.DroppedItems.Components
{
    /// <summary>
    /// An item lying on the ground. Its object and text label are child entities, so they go away with it.
    /// </summary>
    public class DroppedItem : Component
    {
        private const float PickupRange = 1.5f;

        private readonly DroppedInventoryItemModel _droppedItemModel;

        public DroppedItem(DroppedInventoryItemModel droppedItemModel)
        {
            _droppedItemModel = droppedItemModel;
        }

        public ulong GetId()
        {
            return _droppedItemModel.Id;
        }

        /// <summary>
        /// The world inventory item that holds this dropped item.
        /// </summary>
        public ulong GetInventoryItemId()
        {
            return _droppedItemModel.InventoryItemId;
        }

        public ulong GetItemId()
        {
            return _droppedItemModel.InventoryItem.ItemId;
        }

        public uint GetAmount()
        {
            return _droppedItemModel.InventoryItem.Amount;
        }

        public ItemAdditionalData GetAdditionalData()
        {
            return ItemAdditionalData.Parse(_droppedItemModel.InventoryItem.AdditionalData);
        }

        public Vector3 GetPosition()
        {
            return new Vector3(_droppedItemModel.PosX, _droppedItemModel.PosY, _droppedItemModel.PosZ);
        }

        public Vector3 GetRotation()
        {
            return new Vector3(_droppedItemModel.RotX ?? 0, _droppedItemModel.RotY ?? 0, _droppedItemModel.RotZ ?? 0);
        }

        public bool IsPlayerNearby(Player player)
        {
            return player.IsInRangeOfPoint(PickupRange, GetPosition());
        }
    }
}
