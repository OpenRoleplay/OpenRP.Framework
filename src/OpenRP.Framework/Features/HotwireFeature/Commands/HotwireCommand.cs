using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Database.Services;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Characters.Services;
using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.HotwireFeature.Components;
using OpenRP.Framework.Features.Inventories.Services;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Services;
using SampSharp.Entities.SAMP;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Features.Items.Components;
using OpenRP.Framework.Features.Inventories.Components;

namespace OpenRP.Framework.Features.HotwireFeature.Commands
{
    public class HotwireCommand : ISystem
    {
        private readonly Random _random = new Random();

        [ServerCommand(PermissionGroups = new string[] { "Default" },
            Description = "Attempt to hotwire a vehicle. This allows you to start a car without a key, provided you have the necessary tools.",
            CommandGroups = new string[] { "Vehicles", "General" })]
        public void Hotwire(Player player,
                            IEntityManager entityManager,
                            IInventoryService inventoryService,
                            ITempCharacterService characterService,
                            IChatService chatService)
        {
            // Must be in a vehicle and in the driver's seat.
            if (!player.InAnyVehicle)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You must be in a vehicle to hotwire it!");
                return;
            }
            if (player.VehicleSeat != 0)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You must be in the driver's seat to hotwire the vehicle!");
                return;
            }

            // Retrieve the vehicle component from the entity manager.
            Vehicle vehicle = entityManager.GetComponent<Vehicle>(player.Vehicle);
            if (vehicle == null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "This vehicle cannot be hotwired.");
                return;
            }
            if (vehicle.Engine)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "The engine is already running!");
                return;
            }

            PlayerHotwiring isHotwiring = player.GetComponent<PlayerHotwiring>();
            if (isHotwiring != null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You are already hotwiring a vehicle!");
                return;
            }

            // Get the player's character and inventory.
            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();
            Inventory inventory = character.GetInventory();

            // Get item components
            List<Item> items = entityManager.GetComponents<Item>().ToList();

            // Define required items:
            // Rusty Paperclip (ID 33) and Wornout Screwdriver (ID 34) are required.
            // Electrical Tape (ID 35) is optional.
            Item? itemRustyPaperclip = items.FirstOrDefault(i => i.GetId() == 33);
            Item? itemWornoutScrewdriver = items.FirstOrDefault(i => i.GetId() == 34);
            Item? itemElectricalTape = items.FirstOrDefault(i => i.GetId() == 35);

            InventoryItem? invPaperclip = inventory.FindItem(itemRustyPaperclip);
            bool hasScrewdriver = inventory.HasItem(itemWornoutScrewdriver);

            // Verify that the player has both required items.
            if (invPaperclip == null || !hasScrewdriver)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You need a Rusty Paperclip and a Wornout Screwdriver to hotwire the vehicle!");
                return;
            }

            // Consume the Rusty Paperclip.
            if (invPaperclip != null)
            {
                inventory.UseItem(player, invPaperclip);

                // If available, consume Electrical Tape for a safe hotwire.
                bool usedElectricalTape = false;
                InventoryItem? reqTape = inventory.FindItem(itemElectricalTape);
                if (reqTape != null)
                {
                    inventory.UseItem(player, reqTape);
                    usedElectricalTape = true;
                }

                // Attach the hotwiring components.
                // Add the HotwiringPlayer component to the player.
                PlayerHotwiring hotwiringPlayer = player.AddComponent<PlayerHotwiring>();
                hotwiringPlayer.ProgressStep = 0;

                // Disable player control during the process.
                player.ToggleControllable(false);

                // Add the HotwiredVehicle component to the vehicle.
                HotwiredVehicle hotwiredVehicle = vehicle.AddComponent<HotwiredVehicle>();
                hotwiredVehicle.SafelyHotwired = usedElectricalTape;
                hotwiredVehicle.ProgressStep = 0;

                // Send the initial /me action.
                chatService.SendPlayerChatMessage(player, PlayerChatMessageType.ME, "starts fiddling with the wires under the dashboard...");
            }
        }
    }
}
