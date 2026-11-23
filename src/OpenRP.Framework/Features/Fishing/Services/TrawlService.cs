using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Fishing.Components;
using OpenRP.Framework.Features.Fishing.Entities;
using OpenRP.Framework.Features.Fishing.Enums;
using OpenRP.Framework.Features.Inventories.Components;
using OpenRP.Framework.Features.Items.Components;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using SampSharp.ColAndreas.Entities;
using SampSharp.ColAndreas.Entities.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace OpenRP.Framework.Features.Fishing.Services
{
    public class TrawlService : ITrawlService
    {
        private readonly IEntityManager _entityManager;
        private readonly IColAndreasService _colAndreasService;
        private readonly IFishService _fishService;
        private readonly IFishHotspotService _fishHotspotService;
        private readonly IFishingNetService _fishingNetService;
        private readonly ILogger<TrawlService> _logger;

        public TrawlService(IEntityManager entityManager, IColAndreasService colAndreasService, IFishService fishService,
            IFishHotspotService fishHotspotService, IFishingNetService fishingNetService, ILogger<TrawlService> logger)
        {
            _entityManager = entityManager;
            _colAndreasService = colAndreasService;
            _fishService = fishService;
            _fishHotspotService = fishHotspotService;
            _fishingNetService = fishingNetService;
            _logger = logger;
        }

        public void ToggleTrawl(Player player)
        {
            PlayerTrawling? trawling = player.GetComponent<PlayerTrawling>();
            if (trawling != null)
            {
                CancelTrawl(player, trawling, "You raised the net early, so there is no catch.");
                return;
            }

            string? error = GetBeginTrawlError(player);
            if (error != null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, error);
                return;
            }

            player.AddComponent<PlayerTrawling>(player.Vehicle!);
            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO,
                $"The net is down. Sail between {FishingSettings.TrawlMinSpeedInKmh:0} and {FishingSettings.TrawlMaxSpeedInKmh:0} km/h for {FishingSettings.TrawlDurationInSeconds / 60f:0} minutes. Use /trawl to raise it early.");
        }

        public void UpdateTrawlers()
        {
            foreach (PlayerTrawling trawling in _entityManager.GetComponents<PlayerTrawling>().ToList())
            {
                UpdateTrawler(trawling.GetComponent<Player>()!, trawling);
            }
        }

        private string? GetBeginTrawlError(Player player)
        {
            if (!player.IsPlayerPlayingAsCharacter())
                return "You must be playing a character to do this!";
            if (player.GetComponent<PlayerFishing>() != null)
                return "Reel in your line first!";

            Vehicle? vehicle = player.InAnyVehicle ? player.Vehicle : null;
            if (vehicle == null || vehicle.Model != VehicleModelType.Reefer || player.VehicleSeat != 0)
                return "You must be driving a Reefer to trawl!";
            if (_fishingNetService.GetNet(vehicle) == null)
                return "This Reefer has no fishing net fitted. Use /fitnet first.";
            if (!_fishingNetService.CanUseNetOf(player, vehicle))
                return "You must own this Reefer or hold its key to use its net!";
            if (!vehicle.Engine)
                return "The engine must be running to trawl!";
            if (!IsOnWater(vehicle))
                return "The Reefer must be on the water to trawl!";
            return null;
        }

        private bool IsOnWater(Vehicle vehicle)
        {
            Vector3 position = vehicle.Position;
            Vector3 start = new Vector3(position.X, position.Y, position.Z + FishingSettings.VehicleWaterRayStartHeight);
            Vector3 end = new Vector3(position.X, position.Y, position.Z - FishingSettings.VehicleWaterRayLength);
            return _colAndreasService.RayCastLineModel(start, end, out _) == ColAndreasConstants.WaterObject;
        }

        private void UpdateTrawler(Player player, PlayerTrawling trawling)
        {
            Vehicle? vehicle = GetDrivenTrawler(player, trawling);
            if (vehicle == null)
            {
                CancelTrawl(player, trawling, "You left the driver's seat, so the net came up empty.");
                return;
            }

            if (!IsAtTrawlingSpeed(vehicle))
            {
                SendSpeedWarning(player, trawling);
                return;
            }

            AddTrawlProgress(player, trawling);
            if (trawling.IsFinished())
            {
                EndTrawl(player, trawling, vehicle);
            }
        }

        private static Vehicle? GetDrivenTrawler(Player player, PlayerTrawling trawling)
        {
            bool isDriving = player.InAnyVehicle && player.VehicleSeat == 0 && player.Vehicle?.Entity == trawling.GetVehicleEntityId();
            return isDriving ? player.Vehicle : null;
        }

        private static bool IsAtTrawlingSpeed(Vehicle vehicle)
        {
            float speedInKmh = vehicle.Velocity.Length() * FishingSettings.VelocityToKmh;
            return speedInKmh >= FishingSettings.TrawlMinSpeedInKmh && speedInKmh <= FishingSettings.TrawlMaxSpeedInKmh;
        }

        private static void SendSpeedWarning(Player player, PlayerTrawling trawling)
        {
            if (!trawling.IsWarningDue())
            {
                return;
            }

            trawling.SetWarningSent();
            player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR,
                $"The net isn't catching anything. Keep your speed between {FishingSettings.TrawlMinSpeedInKmh:0} and {FishingSettings.TrawlMaxSpeedInKmh:0} km/h.");
        }

        private void AddTrawlProgress(Player player, PlayerTrawling trawling)
        {
            float secondsBefore = trawling.GetCountedSeconds();
            float seconds = FishingSettings.TrawlUpdateIntervalInMilliseconds / 1000f;
            if (_fishHotspotService.IsNearHotspot(player.Position))
            {
                seconds *= FishingSettings.HotspotTrawlProgressMultiplier;
            }
            trawling.AddCountedSeconds(seconds);
            SendProgressMessageWhenDue(player, secondsBefore, trawling.GetCountedSeconds());
        }

        private static void SendProgressMessageWhenDue(Player player, float secondsBefore, float secondsNow)
        {
            int interval = FishingSettings.TrawlProgressMessageIntervalInSeconds;
            if ((int)(secondsBefore / interval) == (int)(secondsNow / interval) || secondsNow >= FishingSettings.TrawlDurationInSeconds)
            {
                return;
            }

            float minutesLeft = (FishingSettings.TrawlDurationInSeconds - secondsNow) / 60f;
            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"The net is filling up. About {Math.Ceiling(minutesLeft):0} more minutes of sailing.");
        }

        private static void CancelTrawl(Player player, PlayerTrawling trawling, string reason)
        {
            trawling.Destroy();
            player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, reason);
        }

        private void EndTrawl(Player player, PlayerTrawling trawling, Vehicle vehicle)
        {
            trawling.Destroy();

            VehicleFishingNet? net = _fishingNetService.GetNet(vehicle);
            Inventory? storage = net == null ? null : _fishingNetService.GetStorage(net);
            if (storage == null)
            {
                _logger.LogError("The net storage of vehicle entity {VehicleEntity} couldn't be loaded after a trawl.", vehicle.Entity);
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "The net came up, but its storage is gone.");
                return;
            }

            IReadOnlyList<FishLootModel> catches = _fishService.GetRandomNetCatch(_fishService.GetFishLootTypeZone(player), _fishService.GetFishingLevel(player));
            int storedFish = StoreCatches(player, storage, catches);
            SendTrawlResult(player, storedFish, catches.Count - storedFish);
        }

        /// <summary>
        /// Puts each fish in the storage and rewards the driver for it. What doesn't fit is released. Returns how many were kept.
        /// </summary>
        private int StoreCatches(Player player, Inventory storage, IReadOnlyList<FishLootModel> catches)
        {
            int storedFish = 0;
            uint experience = 0;
            foreach (FishLootModel loot in catches)
            {
                Item? item = _fishService.GetLootItem(loot);
                uint weightInGrams = _fishService.GetRandomWeightInGrams(loot);
                if (item == null || !storage.AddItem(item, 1, weightInGrams))
                {
                    continue;
                }

                storedFish++;
                experience += _fishService.GetExperienceForCatch(loot);
                _ = _fishService.PublishFishCaught(player, loot, weightInGrams, FishCatchMethod.Net);
            }

            if (experience > 0)
            {
                _fishService.AddFishingExperience(player, experience);
            }
            return storedFish;
        }

        private static void SendTrawlResult(Player player, int storedFish, int releasedFish)
        {
            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"The net comes up with {storedFish} fish. See them with /netstorage.");
            if (releasedFish > 0)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"The net storage is full, so {releasedFish} fish were released.");
            }
        }
    }
}
