﻿using OpenRP.Framework.Features.Harvestables.Entities;
using OpenRP.Framework.Shared.Chat.Enums;
using SampSharp.Entities.SAMP;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Streamer;
using SampSharp.ColAndreas.Entities.Services;
using OpenRP.Framework.Features.Harvestables.Components;

namespace OpenRP.Framework.Features.Harvestables.Services
{
    public class IndianHempHarvestableModel : IHarvestableModel
    {
        private readonly IEntityManager _entityManager;
        private readonly IColAndreasService _colAndreasService;
        private readonly IStreamerService _streamerService;
        public IndianHempHarvestableModel(IEntityManager entityManager, IColAndreasService colAndreasService, IStreamerService streamerService)
        {
            _entityManager = entityManager;
            _streamerService = streamerService;
            _colAndreasService = colAndreasService;
        }

        public string ResourceName 
            => "indian hemp";

        public int[] ResourceObjectModelIds
            => new int[] { 19473 };

        public void CreateHarvestable(int modelId, Vector3 position, Vector3 rotation)
        {
            EntityId harvestableId = HarvestableEntities.GenerateIndianHempId();

            _entityManager.Create(harvestableId);

            DynamicObject harvestableObject = _streamerService.CreateDynamicObject(modelId, position, rotation, parent: harvestableId);
            DynamicTextLabel harvestableTextLabel = _streamerService.CreateDynamicTextLabel(String.Empty, Color.White, new Vector3(position.XY, position.Z + 0.25f), 1.5f, parent: harvestableId);
        }

        public void BeginHarvest(Player player)
        {
            foreach (IndianHempPlant indianHempPlant in _entityManager.GetComponents<IndianHempPlant>())
            {
                if (indianHempPlant.IsPlayerNearby(player))
                {
                    //PlayerHarvesting.StartHarvesting(player, this, indianHempPlant, TimeSpan.FromSeconds(10));
                    return;
                }
            }
            player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You are not near any harvestable Indian Hemp plants!");
        }

        public void EndHarvest(Player player)
        {
        }
    }
}
