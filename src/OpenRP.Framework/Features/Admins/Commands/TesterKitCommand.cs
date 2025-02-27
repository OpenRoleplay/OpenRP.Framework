using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Database.Services;
using OpenRP.Framework.Features.Characters.Services;
using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Inventories.Services;
using OpenRP.Framework.Features.Items.Components;
using OpenRP.Framework.Shared.Chat.Enums;
using SampSharp.Entities.SAMP;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Inventories.Components;
using OpenRP.Framework.Shared.Chat.Extensions;

namespace OpenRP.Framework.Features.Admins.Commands
{
    public class TesterKitCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Tester", "Admin" },
            Description = "Ensures you have the hotwire tools: a Rusty Paperclip, a Wornout Screwdriver and Electrical Tape.")]
        public void Testerkit(Player player, IEntityManager entityManager, IInventoryService inventoryService, ITempCharacterService characterService, IDataMemoryService dataMemory)
        {
            // Retrieve the player's current character and inventory.
            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();
            Inventory characterInventory = character.GetInventory();

            // Get item components
            List<Item> items = entityManager.GetComponents<Item>().ToList();

            // Define the required items by their IDs.
            // Rusty Paperclip (ID 33), Wornout Screwdriver (ID 34), Electrical Tape (ID 35).
            Item? itemRustyPaperclip = items.FirstOrDefault(i => i.GetId() == 33);
            Item? itemWornoutScrewdriver = items.FirstOrDefault(i => i.GetId() == 34);
            Item? itemElectricalTape = items.FirstOrDefault(i => i.GetId() == 35);

            // Check if the player has a Rusty Paperclip; if not, give one.
            if (!characterInventory.HasItem(itemRustyPaperclip))
            {
                characterInventory.AddItem(itemRustyPaperclip, 1);
                player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, "You have received a Rusty Paperclip.");
            }
            else
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, "You already have a Rusty Paperclip.");
            }

            // Check if the player has a Wornout Screwdriver; if not, give one.
            if (!characterInventory.HasItem(itemWornoutScrewdriver))
            {
                characterInventory.AddItem(itemWornoutScrewdriver, 1);
                player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, "You have received a Wornout Screwdriver.");
            }
            else
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, "You already have a Wornout Screwdriver.");
            }

            // Check if the player has Electrical Tape; if not, give one.
            if (!characterInventory.HasItem(itemElectricalTape))
            {
                characterInventory.AddItem(itemElectricalTape, 1);
                player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, "You have received Electrical Tape.");
            }
            else
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, "You already have Electrical Tape.");
            }
        }
    }
}
