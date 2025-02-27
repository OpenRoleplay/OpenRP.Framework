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

namespace OpenRP.Framework.Features.Inventories.Commands
{
    public class InventoryCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Open your character's inventory. Use this command to view, manage, and interact with the items you possess.")]
        public void Inventory(Player player, IInventoryDialogService inventoryDialogService)
        {
            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();

            inventoryDialogService.OpenInventory(player, character.GetInventory());

            /*if (player.IsPlayerPlayingAsCharacter())
            {
                if (player.GetComponent<BillTransactionBetweenPlayers>() != null)
                {
                    player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You cannot open your inventory while doing a transaction!");
                    return;
                }

                if (player.GetComponent<HotwiringPlayer>() != null)
                {
                    player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You cannot open your inventory while hotwiring a vehicle!");
                    return;
                }

                using (var context = new DataContext())
                {
                    Character character = player.GetPlayerCurrentlyPlayingAsCharacter();
                    if (character != null)
                    {
                        InventoryModel characterInventory = tempCharacterService.GetCharacterInventory(character);
                        InventoryDialog.Open(player, characterInventory, dialogService, entityManager, colAndreasService, characterService, droppedItemService, inventoryService, chatService, dataMemoryService);
                    }
                }
            }*/
        }

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Open your character's inventory. Use this command to view, manage, and interact with the items you possess.")]
        public void Inv(Player player, IInventoryDialogService inventoryDialogService)
        {
            Inventory(player, inventoryDialogService);
        }
    }
}
