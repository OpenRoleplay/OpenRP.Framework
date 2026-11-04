using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Database.Services;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Characters.Services;
using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Inventories.Services;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Services;
using SampSharp.ColAndreas.Entities.Services;
using SampSharp.Entities.SAMP;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRP.Framework.Features.Inventories.Services.Dialogs;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Features.Inventories.Components;
using OpenRP.Framework.Shared.Chat.Extensions;

namespace OpenRP.Framework.Features.Inventories.Commands
{
    public class InventoryCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Open your character's inventory. Use this command to view, manage, and interact with the items you possess.")]
        public void Inventory(Player player, IInventoryDialogService inventoryDialogService)
        {
            if (!player.IsPlayerPlayingAsCharacter())
            {
                return;
            }

            Inventory? inventory = player.GetPlayerCurrentlyPlayingAsCharacter().GetInventory();
            if (inventory == null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "Your inventory is still being loaded. Please try again in a minute.");
                return;
            }

            inventoryDialogService.OpenInventory(player, inventory);
        }

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Open your character's inventory. Use this command to view, manage, and interact with the items you possess.")]
        public void Inv(Player player, IInventoryDialogService inventoryDialogService)
        {
            Inventory(player, inventoryDialogService);
        }
    }
}
