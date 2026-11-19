using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Fishing.Components;
using OpenRP.Framework.Features.Fishing.Entities;
using OpenRP.Framework.Features.Fishing.Helpers;
using OpenRP.Framework.Features.Inventories.Components;
using OpenRP.Framework.Features.Inventories.Services;
using OpenRP.Framework.Features.Items.Components;
using OpenRP.Framework.Features.Items.Entities;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Features.Vehicles.Components;
using OpenRP.Framework.Features.Vehicles.Helpers;
using OpenRP.Framework.Features.Vehicles.Services;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System.Linq;

namespace OpenRP.Framework.Features.Fishing.Services
{
    public class FishingNetService : IFishingNetService
    {
        private const string NoReeferError = "You must be on or next to a Reefer to do this!";
        private const string NoAccessError = "You must own this Reefer or hold its key to do this!";
        private const string NoNetError = "This Reefer has no fishing net fitted.";

        private readonly IEntityManager _entityManager;
        private readonly IVehicleInfoService _vehicleInfoService;
        private readonly IVehicleAccessService _vehicleAccessService;
        private readonly IInventoryManager _inventoryManager;
        private readonly IInventoryItemManager _inventoryItemManager;
        private readonly IFishingNetManager _fishingNetManager;

        public FishingNetService(IEntityManager entityManager, IVehicleInfoService vehicleInfoService, IVehicleAccessService vehicleAccessService,
            IInventoryManager inventoryManager, IInventoryItemManager inventoryItemManager, IFishingNetManager fishingNetManager)
        {
            _entityManager = entityManager;
            _vehicleInfoService = vehicleInfoService;
            _vehicleAccessService = vehicleAccessService;
            _inventoryManager = inventoryManager;
            _inventoryItemManager = inventoryItemManager;
            _fishingNetManager = fishingNetManager;
        }

        public void FitNet(Player player)
        {
            Vehicle? reefer = FindReeferAtPlayer(player);
            string? error = GetFitNetError(player, reefer);
            if (error != null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, error);
                return;
            }

            ulong vehicleId = reefer!.GetComponent<VehicleDataComponent>()!.VehicleData.Id;
            player.GetPlayerCurrentlyPlayingAsCharacter().GetInventory()!.TakeItem(GetNetItem()!, 1);
            Inventory storage = _inventoryManager.CreateInventory($"Fishing Net of Vehicle {vehicleId}", FishingSettings.NetStorageMaxWeightInGrams);
            _fishingNetManager.CreateNet(vehicleId, storage.GetId());
            // Saved now, like the net itself: a crash must not leave the player with both the item and a fitted net.
            _inventoryItemManager.SaveNow();
            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, "The net is fitted. Use /trawl while driving to lower it, and /netstorage to see the catch.");
        }

        public void RemoveNet(Player player)
        {
            Vehicle? reefer = FindReeferAtPlayer(player);
            string? error = GetRemoveNetError(player, reefer);
            if (error != null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, error);
                return;
            }

            VehicleFishingNet net = GetNet(reefer!)!;
            DeleteNetAndStorage(net);
            player.GetPlayerCurrentlyPlayingAsCharacter().GetInventory()!.AddItem(GetNetItem()!, 1);
            // Saved now, like the removed net: a crash must not lose the net item.
            _inventoryItemManager.SaveNow();
            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, "You took the net off the Reefer.");
        }

        public void RemoveNetOfDeletedVehicle(ulong vehicleId)
        {
            VehicleFishingNet? net = _fishingNetManager.FindNetOfVehicle(vehicleId);
            if (net != null)
            {
                DeleteNetAndStorage(net);
            }
        }

        public Vehicle? FindReeferAtPlayer(Player player)
        {
            Vehicle? vehicle = BoatHelper.GetVehicleInOrOn(player);
            if (vehicle != null)
            {
                return vehicle.Model == VehicleModelType.Reefer ? vehicle : null;
            }

            return NearbyVehicleFinder.FindNearbyVehicles(_entityManager, _vehicleInfoService, player)
                .FirstOrDefault(nearbyVehicle => nearbyVehicle.Model == VehicleModelType.Reefer);
        }

        public VehicleFishingNet? GetNet(Vehicle vehicle)
        {
            VehicleDataComponent? vehicleData = vehicle.GetComponent<VehicleDataComponent>();
            return vehicleData == null ? null : _fishingNetManager.FindNetOfVehicle(vehicleData.VehicleData.Id);
        }

        public Inventory? GetStorage(VehicleFishingNet net)
        {
            return _inventoryManager.LoadInventory(net.GetStorageInventoryId());
        }

        public bool CanUseNetOf(Player player, Vehicle vehicle)
        {
            VehicleDataComponent? vehicleData = vehicle.GetComponent<VehicleDataComponent>();
            if (vehicleData == null || !player.IsPlayerPlayingAsCharacter())
            {
                return false;
            }

            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();
            return _vehicleAccessService.CanControl(character, vehicleData, _vehicleAccessService.IsAdmin(character));
        }

        public string? GetNetStorageError(Player player, Vehicle? reefer)
        {
            if (reefer == null)
                return NoReeferError;
            if (GetNet(reefer) == null)
                return NoNetError;
            if (!CanUseNetOf(player, reefer))
                return NoAccessError;
            return null;
        }

        public bool IsPlayerAtVehicle(Player player, Vehicle vehicle)
        {
            if (!vehicle.IsComponentAlive)
            {
                return false;
            }

            return BoatHelper.GetVehicleInOrOn(player)?.Entity == vehicle.Entity
                || NearbyVehicleFinder.FindNearbyVehicles(_entityManager, _vehicleInfoService, player).Any(nearbyVehicle => nearbyVehicle.Entity == vehicle.Entity);
        }

        public string? TakeFishFromNet(Player player, VehicleFishingNet net, InventoryItem fish)
        {
            Inventory? inventory = player.GetPlayerCurrentlyPlayingAsCharacter().GetInventory();
            if (inventory == null || !IsFishInInventory(fish, GetStorage(net)))
            {
                return "That fish is no longer in the net.";
            }

            string fishName = fish.GetName();
            return fish.Transfer(inventory, 1) ? null : $"The {fishName} is too heavy for your inventory, so it stays in the net.";
        }

        public string? PutFishInNet(Player player, VehicleFishingNet net, InventoryItem fish)
        {
            Inventory? storage = GetStorage(net);
            if (storage == null || !IsFishInInventory(fish, player.GetPlayerCurrentlyPlayingAsCharacter().GetInventory()))
            {
                return "You no longer have that fish.";
            }
            if (!fish.GetItem().IsItemFish())
            {
                return "The net storage only holds fish.";
            }

            string fishName = fish.GetName();
            return fish.Transfer(storage, 1) ? null : $"The {fishName} doesn't fit in the net storage.";
        }

        private string? GetFitNetError(Player player, Vehicle? reefer)
        {
            if (!player.IsPlayerPlayingAsCharacter())
                return "You must be playing a character to do this!";
            if (reefer == null)
                return NoReeferError;
            if (reefer.GetComponent<VehicleDataComponent>() == null)
                return "Only a saved Reefer can have a net. Temporary vehicles can't.";
            if (!CanUseNetOf(player, reefer))
                return NoAccessError;
            if (GetNet(reefer) != null)
                return "This Reefer already has a net fitted.";

            Inventory? inventory = player.GetPlayerCurrentlyPlayingAsCharacter().GetInventory();
            Item? netItem = GetNetItem();
            if (inventory == null || netItem == null || !inventory.HasItem(netItem))
                return "You need a Fishing Net to do this!";
            return null;
        }

        private string? GetRemoveNetError(Player player, Vehicle? reefer)
        {
            string? accessError = player.IsPlayerPlayingAsCharacter() ? GetNetStorageError(player, reefer) : "You must be playing a character to do this!";
            if (accessError != null)
                return accessError;
            if (IsBeingTrawled(reefer!))
                return "Raise the net with /trawl first!";

            Inventory? storage = GetStorage(GetNet(reefer!)!);
            if (storage != null && storage.GetInventoryItems().Count > 0)
                return "Empty the net storage first! Use /netstorage to see what's in it.";

            Inventory? inventory = player.GetPlayerCurrentlyPlayingAsCharacter().GetInventory();
            Item? netItem = GetNetItem();
            if (inventory == null || netItem == null || !inventory.DoesItemFit(netItem, 1))
                return "The net doesn't fit in your inventory.";
            return null;
        }

        private bool IsBeingTrawled(Vehicle reefer)
        {
            return _entityManager.GetComponents<PlayerTrawling>().Any(trawling => trawling.GetVehicleEntityId() == reefer.Entity);
        }

        /// <summary>
        /// Item rows go first: deleted fish that aren't saved yet would still point at the storage.
        /// </summary>
        private void DeleteNetAndStorage(VehicleFishingNet net)
        {
            Inventory? storage = GetStorage(net);
            storage?.GetInventoryItems().ForEach(fish => fish.ProcessDeletion());
            _inventoryItemManager.SaveNow();
            _fishingNetManager.DeleteNet(net);
            storage?.ProcessDeletion();
            _inventoryManager.SaveNow();
        }

        private Item? GetNetItem()
        {
            return _entityManager.GetComponent<Item>(ItemEntities.GetItemId(FishingSettings.FishingNetItemId));
        }

        private static bool IsFishInInventory(InventoryItem fish, Inventory? inventory)
        {
            return inventory != null && fish.IsComponentAlive && !fish.IsDeleted() && fish.GetInventoryIn().GetId() == inventory.GetId();
        }
    }
}
