using OpenRP.Framework.Features.Inventories.Components;
using OpenRP.Framework.Features.Items.Components;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System.Linq;

namespace OpenRP.Framework.Features.Items.Services
{
    public class ItemGiveService : IItemGiveService
    {
        private readonly IEntityManager _entityManager;

        public ItemGiveService(IEntityManager entityManager)
        {
            _entityManager = entityManager;
        }

        public void GiveItem(Player admin, Player receiver, int itemId, int amount)
        {
            Item? item = FindItem(itemId);
            string? error = GetGiveItemError(receiver, item, itemId, amount);
            if (error != null)
            {
                admin.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, error);
                return;
            }

            AddToInventory(receiver.GetPlayerCurrentlyPlayingAsCharacter().GetInventory()!, item!, (uint)amount);
            SendItemGivenMessages(admin, receiver, item!, amount);
        }

        private Item? FindItem(int itemId)
        {
            return itemId < 1 ? null : _entityManager.GetComponents<Item>().FirstOrDefault(item => item.GetId() == (ulong)itemId);
        }

        private static string? GetGiveItemError(Player receiver, Item? item, int itemId, int amount)
        {
            if (amount < 1)
                return "The amount must be at least 1.";
            if (item == null)
                return $"There is no item with id {itemId}.";
            if (!receiver.IsPlayerPlayingAsCharacter())
                return "That player is not playing as a character.";

            Inventory? inventory = receiver.GetPlayerCurrentlyPlayingAsCharacter().GetInventory();
            if (inventory == null)
                return "That player's inventory is not loaded yet.";
            if (!inventory.DoesItemFit(item, (uint)amount))
                return $"{amount}x {item.GetName()} doesn't fit in that player's inventory.";
            return null;
        }

        /// <summary>
        /// Items that don't stack each get their own row, so every rod wears out by itself.
        /// </summary>
        private static void AddToInventory(Inventory inventory, Item item, uint amount)
        {
            if (item.IsStackable())
            {
                inventory.AddItem(item, amount);
                return;
            }

            for (uint i = 0; i < amount; i++)
            {
                inventory.AddItem(item, 1);
            }
        }

        private static void SendItemGivenMessages(Player admin, Player receiver, Item item, int amount)
        {
            string characterName = receiver.GetPlayerCurrentlyPlayingAsCharacter().GetName();
            admin.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"You gave {amount}x {item.GetName()} to {characterName}.");
            if (receiver.Entity != admin.Entity)
            {
                receiver.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"An admin gave you {amount}x {item.GetName()}.");
            }
        }
    }
}
