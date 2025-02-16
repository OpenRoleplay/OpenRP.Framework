using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Fishing.Enums;
using OpenRP.Framework.Shared.Chat.Enums;
using SampSharp.ColAndreas.Entities.Services;
using SampSharp.Entities.SAMP;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRP.Framework.Features.Fishing.Services;
using OpenRP.Framework.Shared.Chat.Extensions;

namespace OpenRP.Framework.Features.Fishing.Commands
{
    public class FishCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Command to start fishing.",
            CommandGroups = new string[] { "Jobs", "Fishing" })]
        public void Fish(Player player, IColAndreasService colAndreasService, IFishService fishService)
        {
            bool facingWater = colAndreasService.IsPlayerFacingWater(player);
            if (!facingWater)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You must be near water in order to fish!");
                return;
            }

            FishLootType fishLootType = fishService.GetFishLootTypeZone(player);

            FishLootModel fishModel = fishService.GetRandomFishLoot(fishLootType);

            if (fishModel == null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "Ouch! You were unlucky and caught nothing!");
            }
            else
            {
                string fishArea = "Saltwater";
                if (fishLootType == FishLootType.Freshwater)
                {
                    fishArea = "Freshwater";
                }
                else if (fishLootType == FishLootType.Murkywater)
                {
                    fishArea = "Murkywater";
                }

                Random random = new Random();
                player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"If the fishing system was fully implemented, you would be fishing in {fishArea} and caught a {fishModel.Name} weighing {random.Next(fishModel.MinWeightInGrams, fishModel.MaxWeightInGrams)} grams.");
            }
        }
    }
}
