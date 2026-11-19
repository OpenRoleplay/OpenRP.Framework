using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Items.Services;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Admins.Commands
{
    public class GiveItemCommand : ISystem
    {
        private const int DefaultAmount = 1;

        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Give a player an amount of an item, by its item id.",
            CommandGroups = new string[] { "Admin" })]
        public void GiveItem(Player player, IItemGiveService itemGiveService, Player receiver, int item_id, int amount)
        {
            itemGiveService.GiveItem(player, receiver, item_id, amount);
        }

        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Give a player one of an item, by its item id.",
            CommandGroups = new string[] { "Admin" })]
        public void GiveItem(Player player, IItemGiveService itemGiveService, Player receiver, int item_id)
        {
            itemGiveService.GiveItem(player, receiver, item_id, DefaultAmount);
        }

        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Give a player an amount of an item, by its item id.",
            CommandGroups = new string[] { "Admin" })]
        public void GiveItem(Player player)
        {
            player.SendPlayerInfoMessage(PlayerInfoMessageType.SYNTAX, "/giveitem [playerid] [item id] [amount]");
        }
    }
}
