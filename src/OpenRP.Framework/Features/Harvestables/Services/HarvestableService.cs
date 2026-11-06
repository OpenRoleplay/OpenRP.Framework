using Microsoft.Extensions.Logging;
using OpenRP.Framework.Features.Harvestables.Components;
using OpenRP.Framework.Features.Harvestables.Entities;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;
using OpenRP.Framework.Shared.ServerEvents.Services;
using OpenRP.Streamer;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Harvestables.Services
{
    public class HarvestableService : IHarvestableService
    {
        private const float LabelHeightAbovePlant = 0.25f;
        private const float LabelDrawDistance = 1.5f;

        private readonly IEntityManager _entityManager;
        private readonly IStreamerService _streamerService;
        private readonly IServerEventAggregator _serverEventAggregator;
        private readonly List<IHarvestableModel> _harvestableModels;
        private readonly ILogger<HarvestableService> _logger;

        public HarvestableService(IEntityManager entityManager, IStreamerService streamerService, IServerEventAggregator serverEventAggregator, IEnumerable<IHarvestableModel> harvestableModels, ILogger<HarvestableService> logger)
        {
            _entityManager = entityManager;
            _streamerService = streamerService;
            _serverEventAggregator = serverEventAggregator;
            _harvestableModels = harvestableModels.ToList();
            _logger = logger;
            LogDuplicateModels();
        }

        public bool IsHarvestableModel(int modelId)
        {
            return GetModelByObjectModelId(modelId) != null;
        }

        public IEnumerable<string> GetResourceNames()
        {
            return _harvestableModels.Select(model => model.ResourceName);
        }

        public bool AddHarvestable(EntityId entityId, int modelId, Vector3 position)
        {
            IHarvestableModel? harvestableModel = GetModelByObjectModelId(modelId);
            if (harvestableModel == null)
            {
                return false;
            }

            Vector3 labelPosition = new Vector3(position.XY, position.Z + LabelHeightAbovePlant);
            HarvestablePlant plant = harvestableModel.AddPlant(entityId, labelPosition);
            _streamerService.CreateDynamicTextLabel(plant.GetLabelText(), Color.White, labelPosition, LabelDrawDistance, testLos: true, parent: entityId);
            return true;
        }

        public bool CreateHarvestable(int modelId, Vector3 position, Vector3 rotation)
        {
            if (!IsHarvestableModel(modelId))
            {
                return false;
            }

            EntityId entityId = HarvestableEntities.GenerateHarvestablePlantId();
            _entityManager.Create(entityId);
            _streamerService.CreateDynamicObject(modelId, position, rotation, parent: entityId);
            return AddHarvestable(entityId, modelId, position);
        }

        public void BeginHarvest(Player player, string resourceName)
        {
            IHarvestableModel? harvestableModel = GetModelByResourceName(resourceName);
            HarvestablePlant? plant = harvestableModel == null ? null : GetNearbyPlant(harvestableModel, player);
            string? error = GetBeginHarvestError(player, resourceName, harvestableModel, plant);
            if (error != null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, error);
                return;
            }

            player.AddComponent<PlayerHarvesting>(plant!, harvestableModel!);
            player.ApplyAnimation(harvestableModel!.HarvestAnimationLibrary, harvestableModel.HarvestAnimationName, 4.1f, true, false, false, true, 0);
            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"You are now harvesting {harvestableModel.ResourceName}.");
        }

        public void EndFinishedHarvests()
        {
            foreach (PlayerHarvesting harvesting in _entityManager.GetComponents<PlayerHarvesting>().Where(harvesting => harvesting.IsFinished()).ToList())
            {
                EndHarvest(harvesting);
            }
        }

        private static string? GetBeginHarvestError(Player player, string resourceName, IHarvestableModel? harvestableModel, HarvestablePlant? plant)
        {
            if (harvestableModel == null)
                return $"No harvestable resource found for '{resourceName}'.";
            if (player.GetComponent<PlayerHarvesting>() != null)
                return "You are already harvesting something!";
            if (plant == null)
                return $"You are not near any harvestable {harvestableModel.ResourceName}!";
            if (!plant.HasHarvestsLeft())
                return "This plant has nothing left to harvest.";
            return null;
        }

        private void EndHarvest(PlayerHarvesting harvesting)
        {
            Player player = harvesting.GetComponent<Player>();
            HarvestablePlant plant = harvesting.GetPlant();
            string resourceName = harvesting.GetHarvestableModel().ResourceName;
            harvesting.Destroy();
            player.ClearAnimations();

            if (!_entityManager.Exists(plant.Entity) || !plant.HasHarvestsLeft())
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "There was nothing left to harvest.");
                return;
            }

            plant.RemoveOneHarvest();
            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"You have harvested 1x {resourceName}.");
            PublishResourceHarvested(player, resourceName);
        }

        private async void PublishResourceHarvested(Player player, string resourceName)
        {
            try
            {
                await _serverEventAggregator.PublishAsync(new OnResourceHarvestedEventArgs { Player = player, ResourceName = resourceName });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish that {ResourceName} was harvested.", resourceName);
            }
        }

        private IHarvestableModel? GetModelByObjectModelId(int modelId)
        {
            return _harvestableModels.FirstOrDefault(model => model.ResourceObjectModelIds.Contains(modelId));
        }

        private IHarvestableModel? GetModelByResourceName(string resourceName)
        {
            return _harvestableModels.FirstOrDefault(model => model.ResourceName.Equals(resourceName, StringComparison.OrdinalIgnoreCase));
        }

        private static HarvestablePlant? GetNearbyPlant(IHarvestableModel harvestableModel, Player player)
        {
            return harvestableModel.GetPlants().FirstOrDefault(plant => plant.IsPlayerNearby(player));
        }

        /// <summary>
        /// Two plant types with the same name or object model would shadow each other, so that's reported at startup.
        /// </summary>
        private void LogDuplicateModels()
        {
            foreach (string resourceName in _harvestableModels.GroupBy(model => model.ResourceName.ToLowerInvariant()).Where(group => group.Count() > 1).Select(group => group.Key))
            {
                _logger.LogError("Several harvestable models use the resource name '{ResourceName}'.", resourceName);
            }

            foreach (int modelId in _harvestableModels.SelectMany(model => model.ResourceObjectModelIds).GroupBy(id => id).Where(group => group.Count() > 1).Select(group => group.Key))
            {
                _logger.LogError("Several harvestable models use object model {ModelId}.", modelId);
            }
        }
    }
}
