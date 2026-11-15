using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Inventories.Components;
using OpenRP.Framework.Features.Inventories.Helpers;
using OpenRP.Framework.Features.Items.Components;
using OpenRP.Framework.Features.Items.Entities;
using OpenRP.Framework.Features.Players.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System.Linq;

namespace OpenRP.Framework.Features.Vehicles.Services
{
    public class VehicleKeyService : IVehicleKeyService
    {
        // The vehicle key is item model 32 in the item table.
        private const int VehicleKeyItemId = 32;
        private const string NoInventoryError = "You're not playing a character with an inventory.";

        private readonly IEntityManager _entityManager;

        public VehicleKeyService(IEntityManager entityManager)
        {
            _entityManager = entityManager;
        }

        public string? GiveVehicleKey(Player player, ulong vehicleId)
        {
            Character? character = player.IsPlayerPlayingAsCharacter() ? player.GetPlayerCurrentlyPlayingAsCharacter() : null;
            Inventory? inventory = character?.GetInventory();
            if (character == null || inventory == null)
            {
                return NoInventoryError;
            }
            if (HasVehicleKey(character, vehicleId))
            {
                return "You already have a key for this vehicle.";
            }

            Item? keyItem = _entityManager.GetComponent<Item>(ItemEntities.GetItemId(VehicleKeyItemId));
            if (keyItem == null)
            {
                return "The vehicle key item doesn't exist.";
            }
            if (!inventory.DoesItemFit(keyItem, 1))
            {
                return "The key doesn't fit in your inventory.";
            }

            inventory.AddItem(keyItem, 1, CreateKeyAdditionalData(vehicleId));
            return null;
        }

        public bool HasVehicleKey(Character character, ulong vehicleId)
        {
            Inventory? inventory = character.GetInventory();
            if (inventory == null)
            {
                return false;
            }

            return inventory.GetInventoryItems().Any(inventoryItem => inventoryItem.GetVehicleId() == vehicleId);
        }

        public void RemoveVehicleKeys(ulong vehicleId)
        {
            // A list, because processing a key removes its component while the components are still being read.
            foreach (InventoryItem inventoryItem in _entityManager.GetComponents<InventoryItem>().ToList())
            {
                if (!inventoryItem.IsDeleted() && inventoryItem.GetVehicleId() == vehicleId)
                {
                    inventoryItem.ProcessDeletion();
                }
            }
        }

        private static ItemAdditionalData CreateKeyAdditionalData(ulong vehicleId)
        {
            ItemAdditionalData additionalData = ItemAdditionalData.Parse(string.Empty);
            additionalData.SetString(InventoryItem.VehicleIdKey, vehicleId.ToString());
            return additionalData;
        }
    }
}
