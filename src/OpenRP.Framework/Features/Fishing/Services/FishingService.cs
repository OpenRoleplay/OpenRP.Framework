using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.DroppedItems.Services;
using OpenRP.Framework.Features.Fishing.Components;
using OpenRP.Framework.Features.Fishing.Entities;
using OpenRP.Framework.Features.Fishing.Enums;
using OpenRP.Framework.Features.Fishing.Helpers;
using OpenRP.Framework.Features.Inventories.Components;
using OpenRP.Framework.Features.Items.Components;
using OpenRP.Framework.Features.Items.Entities;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Framework.Shared.Chat.Services;
using SampSharp.ColAndreas.Entities;
using SampSharp.ColAndreas.Entities.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Linq;
using System.Numerics;

namespace OpenRP.Framework.Features.Fishing.Services
{
    public class FishingService : IFishingService
    {
        private readonly IEntityManager _entityManager;
        private readonly IColAndreasService _colAndreasService;
        private readonly IChatService _chatService;
        private readonly IFishService _fishService;
        private readonly IFishHotspotService _fishHotspotService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<FishingService> _logger;

        public FishingService(IEntityManager entityManager, IColAndreasService colAndreasService, IChatService chatService, IFishService fishService,
            IFishHotspotService fishHotspotService, IServiceScopeFactory scopeFactory, ILogger<FishingService> logger)
        {
            _entityManager = entityManager;
            _colAndreasService = colAndreasService;
            _chatService = chatService;
            _fishService = fishService;
            _fishHotspotService = fishHotspotService;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public void BeginFishing(Player player)
        {
            string? error = GetBeginFishingError(player);
            if (error != null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, error);
                return;
            }

            Inventory inventory = player.GetPlayerCurrentlyPlayingAsCharacter().GetInventory()!;
            UseRodAndBait(player, inventory);
            player.AddComponent<PlayerFishing>(CreateCast(player));
            ShowCast(player);
        }

        public void UpdateFishers()
        {
            foreach (PlayerFishing fishing in _entityManager.GetComponents<PlayerFishing>().ToList())
            {
                UpdateFisher(fishing.GetComponent<Player>()!, fishing);
            }
        }

        public void LandBite(Player player)
        {
            PlayerFishing? fishing = player.GetComponent<PlayerFishing>();
            if (fishing == null || !fishing.IsBiting())
            {
                return;
            }

            bool isOnBoat = fishing.GetCast().BoatEntityId != null;
            fishing.Destroy();

            FishLootModel? loot = _fishService.GetRandomRodCatch(_fishService.GetFishLootTypeZone(player), _fishService.GetFishingLevel(player), isOnBoat);
            if (loot == null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "Your line comes up empty.");
                return;
            }
            GiveCatch(player, loot);
        }

        public void EndFishingAfterDamage(Player player)
        {
            CancelFishing(player, "You got hurt and dropped your line.");
        }

        public void EndFishingAfterDeath(Player player)
        {
            CancelFishing(player, "You died, and your line snapped.");
        }

        public void EndFishing(Player player)
        {
            player.GetComponent<PlayerFishing>()?.Destroy();
        }

        private string? GetBeginFishingError(Player player)
        {
            if (!player.IsPlayerPlayingAsCharacter())
                return "You must be playing a character to fish!";
            if (player.GetComponent<PlayerFishing>() != null)
                return "You already have a line in the water!";
            if (player.GetComponent<PlayerTrawling>() != null)
                return "You can't use a rod while you're trawling!";

            Inventory? inventory = player.GetPlayerCurrentlyPlayingAsCharacter().GetInventory();
            if (inventory == null || GetUsableRod(inventory) == null)
                return "You need a Fishing Rod to fish!";
            if (GetBait(inventory) == null)
                return "You need Fishing Bait to fish!";
            if (!IsAtFishingSpot(player))
                return "You must be facing water, or be on a boat, to fish!";
            return null;
        }

        private static InventoryItem? GetUsableRod(Inventory inventory)
        {
            return inventory.GetInventoryItems().FirstOrDefault(inventoryItem =>
                inventoryItem.GetItem().GetId() == FishingSettings.FishingRodItemId
                && (!inventoryItem.HasMaxUses() || inventoryItem.HasRemainingUses()));
        }

        private Item? GetBait(Inventory inventory)
        {
            Item? bait = _entityManager.GetComponent<Item>(ItemEntities.GetItemId(FishingSettings.FishingBaitItemId));
            return bait != null && inventory.HasItem(bait) ? bait : null;
        }

        private bool IsAtFishingSpot(Player player)
        {
            Vehicle? boat = BoatHelper.GetBoatInOrOn(player);
            if (boat != null)
            {
                return IsAboveWater(boat.Position);
            }
            return !player.InAnyVehicle && _colAndreasService.IsPlayerFacingWater(player, FishingSettings.FacingWaterDistance, FishingSettings.FacingWaterRayLength);
        }

        private bool IsAboveWater(Vector3 position)
        {
            Vector3 start = new Vector3(position.X, position.Y, position.Z + FishingSettings.VehicleWaterRayStartHeight);
            Vector3 end = new Vector3(position.X, position.Y, position.Z - FishingSettings.BoatWaterRayLength);
            return _colAndreasService.RayCastLineModel(start, end, out _) == ColAndreasConstants.WaterObject;
        }

        private void UseRodAndBait(Player player, Inventory inventory)
        {
            inventory.TakeItem(GetBait(inventory)!, 1);

            InventoryItem rod = GetUsableRod(inventory)!;
            bool isLastUse = rod.GetRemainingUses() == 1;
            rod.Use();
            if (isLastUse)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, "Your Fishing Rod is worn out. This is its last cast.");
            }
        }

        private FishingCast CreateCast(Player player)
        {
            Vehicle? boat = BoatHelper.GetBoatInOrOn(player);
            return new FishingCast(player.Position, boat?.Entity, DateTime.UtcNow.AddSeconds(GetBiteWaitInSeconds(player)));
        }

        /// <summary>
        /// A random wait, which a nearby hotspot cuts in half.
        /// </summary>
        private double GetBiteWaitInSeconds(Player player)
        {
            double waitInSeconds = Random.Shared.Next(FishingSettings.MinBiteWaitInSeconds, FishingSettings.MaxBiteWaitInSeconds + 1);
            return _fishHotspotService.IsNearHotspot(player.Position) ? waitInSeconds * FishingSettings.HotspotBiteWaitMultiplier : waitInSeconds;
        }

        private void ShowCast(Player player)
        {
            player.SetAttachedObject(FishingSettings.RodAttachedObjectSlot, FishingSettings.RodObjectModelId, Bone.RightHand,
                new Vector3(0.079f, 0.037f, 0.007f), new Vector3(181.5f, 186.4f, 348.2f), Vector3.One, default, default);

            // A seated player can't play this animation.
            if (!player.InAnyVehicle)
            {
                player.ApplyAnimation(FishingSettings.FishingAnimationLibrary, FishingSettings.FishingAnimationName, 4.1f, false, false, false, true, TimeSpan.Zero, PlayerAnimationSyncType.Sync);
            }

            _chatService.SendPlayerChatMessage(player, PlayerChatMessageType.ME, "casts a fishing line into the water.");
            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, "Line in the water. Stay put and press Y as soon as a fish bites.");
        }

        private void UpdateFisher(Player player, PlayerFishing fishing)
        {
            if (HasLeftFishingSpot(player, fishing.GetCast()))
            {
                CancelFishing(player, "You moved away and pulled your line out of the water.");
                return;
            }

            if (fishing.IsBiteOver())
            {
                fishing.Destroy();
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "Too slow! The fish got away.");
                return;
            }

            if (fishing.IsBiteDue())
            {
                fishing.BeginBite();
                player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, "A fish is biting! Press Y now!");
                player.GameText("~y~Press Y!", TimeSpan.FromSeconds(FishingSettings.BiteWindowInSeconds), GameTextStyle.Style3);
            }
        }

        private static void CancelFishing(Player player, string reason)
        {
            PlayerFishing? fishing = player.GetComponent<PlayerFishing>();
            if (fishing == null)
            {
                return;
            }

            fishing.Destroy();
            player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, reason);
        }

        private static bool HasLeftFishingSpot(Player player, FishingCast cast)
        {
            // Standing on a deck isn't checked: the game briefly reports no vehicle underfoot while a boat rocks.
            Vehicle? seatedVehicle = player.InAnyVehicle ? player.Vehicle : null;
            if (seatedVehicle != null && seatedVehicle.Entity != cast.BoatEntityId)
            {
                return true;
            }

            float maxDistance = cast.BoatEntityId != null ? FishingSettings.MaxMoveDistanceOnBoat : FishingSettings.MaxMoveDistanceOnLand;
            return Vector3.Distance(player.Position, cast.Position) > maxDistance;
        }

        private void GiveCatch(Player player, FishLootModel loot)
        {
            Item? item = _fishService.GetLootItem(loot);
            if (item == null)
            {
                _logger.LogError("Fish loot {LootName} points to item {ItemId}, which doesn't exist.", loot.Name, loot.ItemId);
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "Your line comes up empty.");
                return;
            }

            uint weightInGrams = _fishService.GetRandomWeightInGrams(loot);
            bool isInInventory = AddCatchToInventory(player, loot, item, weightInGrams);
            if (!isInInventory)
            {
                DropCatchAtFeet(player, loot, item, weightInGrams);
            }

            _chatService.SendPlayerChatMessage(player, PlayerChatMessageType.ME, $"reels in the line and pulls up: {loot.Name}.");
            SendCatchMessage(player, loot, weightInGrams, isInInventory);
            _fishService.AddFishingExperience(player, _fishService.GetExperienceForCatch(loot));
            _ = _fishService.PublishFishCaught(player, loot, weightInGrams, FishCatchMethod.Rod);
        }

        /// <summary>
        /// Only fish keep a weight of their own. Junk stays the plain item, because /hotwire looks for an unchanged Rusty Paperclip.
        /// </summary>
        private static bool AddCatchToInventory(Player player, FishLootModel loot, Item item, uint weightInGrams)
        {
            Inventory? inventory = player.GetPlayerCurrentlyPlayingAsCharacter().GetInventory();
            if (inventory == null)
            {
                return false;
            }
            return loot.LootKind == FishLootKind.Fish ? inventory.AddItem(item, 1, weightInGrams) : inventory.AddItem(item, 1);
        }

        private void DropCatchAtFeet(Player player, FishLootModel loot, Item item, uint weightInGrams)
        {
            // The dropped item service holds a database context, so it only lives as long as this scope.
            using IServiceScope scope = _scopeFactory.CreateScope();
            IDroppedItemService droppedItemService = scope.ServiceProvider.GetRequiredService<IDroppedItemService>();
            droppedItemService.DropNewItem(player, item, loot.LootKind == FishLootKind.Fish ? weightInGrams : null);
        }

        private static void SendCatchMessage(Player player, FishLootModel loot, uint weightInGrams, bool isInInventory)
        {
            string weightText = loot.LootKind == FishLootKind.Fish ? $" ({FishWeightText.GetWeightText(weightInGrams)})" : string.Empty;
            string whereText = isInInventory ? string.Empty : " It didn't fit in your inventory, so it landed at your feet.";
            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"You caught: {loot.Name}{weightText}.{whereText}");
        }
    }
}
