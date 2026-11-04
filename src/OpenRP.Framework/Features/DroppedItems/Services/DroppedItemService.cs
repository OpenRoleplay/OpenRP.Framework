using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.DroppedItems.Components;
using OpenRP.Framework.Features.DroppedItems.Entities;
using OpenRP.Framework.Features.Inventories.Components;
using OpenRP.Framework.Features.Inventories.Entities;
using OpenRP.Framework.Features.Inventories.Helpers;
using OpenRP.Framework.Features.Items.Components;
using OpenRP.Framework.Features.Items.Entities;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;
using OpenRP.Framework.Shared.ServerEvents.Services;
using OpenRP.Streamer;
using SampSharp.ColAndreas.Entities.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.DroppedItems.Services
{
    public class DroppedItemService : IDroppedItemService
    {
        // Dropped items are inventory items in this inventory (see InventoryModelDefaultData).
        private const ulong WorldInventoryId = 1;
        private const int DefaultDropModelId = 1575;
        private const float LabelHeightAboveItem = 0.25f;
        private const float LabelDrawDistance = 1.5f;
        private const float GroundRayCastEndZ = -100f;

        private readonly BaseDataContext _dataContext;
        private readonly IEntityManager _entityManager;
        private readonly IStreamerService _streamerService;
        private readonly IColAndreasService _colAndreasService;
        private readonly IServerEventAggregator _serverEventAggregator;
        private readonly ILogger<DroppedItemService> _logger;

        public DroppedItemService(BaseDataContext dataContext, IEntityManager entityManager, IStreamerService streamerService, IColAndreasService colAndreasService, IServerEventAggregator serverEventAggregator, ILogger<DroppedItemService> logger)
        {
            _dataContext = dataContext;
            _entityManager = entityManager;
            _streamerService = streamerService;
            _colAndreasService = colAndreasService;
            _serverEventAggregator = serverEventAggregator;
            _logger = logger;
        }

        public void LoadDroppedItems()
        {
            List<DroppedInventoryItemModel> droppedItemModels = _dataContext.DroppedInventoryItems
                .Include(droppedItem => droppedItem.InventoryItem)
                    .ThenInclude(inventoryItem => inventoryItem.Item)
                .AsNoTracking()
                .ToList();

            foreach (DroppedInventoryItemModel droppedItemModel in droppedItemModels)
            {
                CreateDroppedItemEntity(droppedItemModel, droppedItemModel.InventoryItem.Item);
            }
            _logger.LogInformation("Loaded {Count} dropped items.", droppedItemModels.Count);
        }

        public async Task<bool> DropItem(Player player, InventoryItem inventoryItem, uint amount)
        {
            try
            {
                Item item = inventoryItem.GetItem();
                if (amount == 0 || !inventoryItem.HasAmount(amount) || !item.IsDroppable())
                {
                    return false;
                }

                (Vector3 position, Vector3 rotation) = GetDropPosition(player);
                DroppedInventoryItemModel droppedItemModel = CreateWorldItem(inventoryItem, amount, position, rotation);
                AddWorldItemComponent(droppedItemModel.InventoryItem);
                RemoveFromInventory(inventoryItem, amount);
                CreateDroppedItemEntity(droppedItemModel, item.GetRawItemModel());

                player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"You have dropped {amount}x {item.GetName()}.");
                await PublishCashUpdateIfCurrency(player, item);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to drop an item.");
                return false;
            }
        }

        public async Task PickupNearbyItem(Player player)
        {
            try
            {
                DroppedItem? droppedItem = GetNearbyDroppedItem(player);
                if (droppedItem == null)
                {
                    player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You are not near any dropped items!");
                    return;
                }

                Item? item = GetItem(droppedItem);
                if (item == null || !AddToCharacterInventory(player, item, droppedItem))
                {
                    player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You don't have enough space in your inventory to pick this up!");
                    return;
                }

                uint amount = droppedItem.GetAmount();
                DeleteWorldItem(droppedItem.GetInventoryItemId());
                droppedItem.DestroyEntity();

                player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"You have picked up {amount}x {item.GetName()}.");
                await PublishCashUpdateIfCurrency(player, item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to pick up an item.");
            }
        }

        private DroppedItem? GetNearbyDroppedItem(Player player)
        {
            return _entityManager.GetComponents<DroppedItem>().FirstOrDefault(droppedItem => droppedItem.IsPlayerNearby(player));
        }

        private Item? GetItem(DroppedItem droppedItem)
        {
            return _entityManager.GetComponent<Item>(ItemEntities.GetItemId((int)droppedItem.GetItemId()));
        }

        private static bool AddToCharacterInventory(Player player, Item item, DroppedItem droppedItem)
        {
            Inventory? inventory = player.GetPlayerCurrentlyPlayingAsCharacter().GetInventory();
            return inventory != null && inventory.AddItem(item, droppedItem.GetAmount(), droppedItem.GetAdditionalData());
        }

        /// <summary>
        /// The spot on the ground below the player, tilted to the slope when the ground is found.
        /// </summary>
        private (Vector3 Position, Vector3 Rotation) GetDropPosition(Player player)
        {
            Vector3 start = player.Position;
            Vector3 end = new Vector3(start.XY, GroundRayCastEndZ);
            if (_colAndreasService.RayCastLineAngle(start, end, out Vector3 hitPosition, out Vector3 hitRotation) != 0)
            {
                return (hitPosition, hitRotation);
            }
            return (_colAndreasService.FindZ_For2DCoord(start), Vector3.Zero);
        }

        /// <summary>
        /// Saves the dropped part as a new item in the world inventory, together with where it lies.
        /// </summary>
        private DroppedInventoryItemModel CreateWorldItem(InventoryItem inventoryItem, uint amount, Vector3 position, Vector3 rotation)
        {
            InventoryItemModel source = inventoryItem.GetRawInventoryItemModel();
            DroppedInventoryItemModel droppedItemModel = new DroppedInventoryItemModel
            {
                PosX = position.X,
                PosY = position.Y,
                PosZ = position.Z,
                RotX = rotation.X,
                RotY = rotation.Y,
                RotZ = rotation.Z,
                InventoryItem = new InventoryItemModel
                {
                    ItemId = source.ItemId,
                    Amount = amount,
                    UsesRemaining = source.UsesRemaining,
                    KeepOnDeath = source.KeepOnDeath,
                    CanDrop = source.CanDrop,
                    CanDestroy = source.CanDestroy,
                    Weight = source.Weight,
                    InventoryId = WorldInventoryId,
                    AdditionalData = GetAdditionalDataWithoutPersonalData(source.AdditionalData),
                },
            };

            _dataContext.DroppedInventoryItems.Add(droppedItemModel);
            _dataContext.SaveChanges();
            DetachFromDataContext(droppedItemModel);
            return droppedItemModel;
        }

        private static string? GetAdditionalDataWithoutPersonalData(string? additionalData)
        {
            string cleaned = ItemAdditionalData.RemovePersonalizedData(ItemAdditionalData.Parse(additionalData)).ToString();
            return cleaned.Length == 0 ? null : cleaned;
        }

        private void DetachFromDataContext(DroppedInventoryItemModel droppedItemModel)
        {
            _dataContext.Entry(droppedItemModel).State = EntityState.Detached;
            _dataContext.Entry(droppedItemModel.InventoryItem).State = EntityState.Detached;

            // The world inventory item component keeps this model; it must not drag the dropped row along when saved.
            droppedItemModel.InventoryItem.DroppedInventoryItem = null;
        }

        /// <summary>
        /// Loads the new world item right away, so the inventory item manager sees it as already loaded.
        /// </summary>
        private void AddWorldItemComponent(InventoryItemModel worldItemModel)
        {
            EntityId worldInventoryEntityId = InventoryEntities.GetInventoryId((int)WorldInventoryId);
            EntityId worldItemEntityId = InventoryEntities.GetInventoryItemId((int)worldItemModel.Id);
            if (!_entityManager.Exists(worldInventoryEntityId) || _entityManager.Exists(worldItemEntityId))
            {
                return;
            }

            _entityManager.Create(worldItemEntityId, worldInventoryEntityId);
            _entityManager.AddComponent<InventoryItem>(worldItemEntityId, worldItemModel);
        }

        private static void RemoveFromInventory(InventoryItem inventoryItem, uint amount)
        {
            if (inventoryItem.GetAmount() <= amount)
            {
                inventoryItem.ProcessDeletion();
                return;
            }
            inventoryItem.Subtract(amount);
        }

        /// <summary>
        /// Removes the world item from the database (its dropped row is deleted with it) and from memory.
        /// </summary>
        private void DeleteWorldItem(ulong worldItemId)
        {
            _dataContext.InventoryItems.Where(inventoryItem => inventoryItem.Id == worldItemId).ExecuteDelete();

            EntityId worldItemEntityId = InventoryEntities.GetInventoryItemId((int)worldItemId);
            if (_entityManager.Exists(worldItemEntityId))
            {
                _entityManager.Destroy(worldItemEntityId);
            }
        }

        private void CreateDroppedItemEntity(DroppedInventoryItemModel droppedItemModel, ItemModel itemModel)
        {
            EntityId droppedItemEntityId = DroppedItemEntities.GetDroppedItemId((int)droppedItemModel.Id);
            _entityManager.Create(droppedItemEntityId);
            DroppedItem droppedItem = _entityManager.AddComponent<DroppedItem>(droppedItemEntityId, droppedItemModel);

            Vector3 position = droppedItem.GetPosition();
            _streamerService.CreateDynamicObject(itemModel.DropModelId ?? DefaultDropModelId, position, droppedItem.GetRotation(), parent: droppedItemEntityId);
            _streamerService.CreateDynamicTextLabel(
                $"{droppedItem.GetAmount()}x {itemModel.Name}\n/pickup to pickup",
                Color.WhiteSmoke,
                new Vector3(position.XY, position.Z + LabelHeightAboveItem),
                LabelDrawDistance,
                testLos: true,
                parent: droppedItemEntityId);
        }

        /// <summary>
        /// Money on the ground no longer counts, so the HUD money has to be refreshed.
        /// </summary>
        private async Task PublishCashUpdateIfCurrency(Player player, Item item)
        {
            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();
            ulong? defaultCurrencyId = character.GetPreferences()?.GetDefaultCurrencyId();
            if (!item.IsItemCurrency() || defaultCurrencyId == null)
            {
                return;
            }

            await _serverEventAggregator.PublishAsync(new OnPlayerCashUpdateEventArgs
            {
                Player = player,
                CurrencyId = defaultCurrencyId.Value,
            });
        }
    }
}
