using OpenRP.Framework.Features.Fishing.Components;
using OpenRP.Framework.Features.Fishing.Entities;
using OpenRP.Framework.Features.Fishing.Helpers;
using OpenRP.Framework.Features.Inventories.Components;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Extensions;
using SampSharp.Entities.SAMP;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Fishing.Services.Dialogs
{
    public class FishingNetDialogService : IFishingNetDialogService
    {
        private const string DialogTitle = "Net Storage";
        private const string LeftTheBoatError = "You moved too far away from the Reefer.";

        private readonly IDialogService _dialogService;
        private readonly IFishingNetService _fishingNetService;

        public FishingNetDialogService(IDialogService dialogService, IFishingNetService fishingNetService)
        {
            _dialogService = dialogService;
            _fishingNetService = fishingNetService;
        }

        public void OpenNetStorage(Player player)
        {
            if (!player.IsPlayerPlayingAsCharacter())
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You must be playing a character to do this!");
                return;
            }

            Vehicle? reefer = _fishingNetService.FindReeferAtPlayer(player);
            string? error = _fishingNetService.GetNetStorageError(player, reefer);
            if (error != null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, error);
                return;
            }

            ShowNetStorage(player, reefer!, _fishingNetService.GetNet(reefer!)!);
        }

        private void ShowNetStorage(Player player, Vehicle reefer, VehicleFishingNet net)
        {
            if (!CanStillUseNet(player, reefer, net))
            {
                return;
            }

            Inventory? storage = _fishingNetService.GetStorage(net);
            if (storage == null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "The net storage can't be opened right now.");
                return;
            }

            BetterTablistDialog dialog = new BetterTablistDialog("Select", "Close", 2);
            dialog.SetTitle(TitleType.Children, DialogTitle, storage.GetInventoryDialogName());
            dialog.AddHeaders("Fish", "Weight");

            int putFishRow = dialog.AddRow("Put a fish in the net", string.Empty);
            Dictionary<int, InventoryItem> fishByRow = AddFishRows(dialog, storage.GetInventoryItems());

            _dialogService.ShowIgnoringDisconnect(player, dialog, response =>
            {
                if (response.Response != DialogResponse.LeftButton)
                {
                    return;
                }
                if (response.ItemIndex == putFishRow)
                {
                    ShowPutFish(player, reefer, net);
                }
                else if (fishByRow.TryGetValue(response.ItemIndex, out InventoryItem? fish))
                {
                    TakeFish(player, reefer, net, fish);
                }
            });
        }

        private void ShowPutFish(Player player, Vehicle reefer, VehicleFishingNet net)
        {
            if (!CanStillUseNet(player, reefer, net))
            {
                return;
            }

            List<InventoryItem> ownFish = GetFishInInventory(player);
            if (ownFish.Count == 0)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You aren't carrying any fish.");
                ShowNetStorage(player, reefer, net);
                return;
            }

            BetterTablistDialog dialog = new BetterTablistDialog("Put In", "Back", 2);
            dialog.SetTitle(TitleType.Children, DialogTitle, "Put a fish in the net");
            dialog.AddHeaders("Fish", "Weight");
            Dictionary<int, InventoryItem> fishByRow = AddFishRows(dialog, ownFish);

            _dialogService.ShowIgnoringDisconnect(player, dialog, response =>
            {
                if (response.Response == DialogResponse.LeftButton && fishByRow.TryGetValue(response.ItemIndex, out InventoryItem? fish))
                {
                    PutFish(player, reefer, net, fish);
                    return;
                }
                ShowNetStorage(player, reefer, net);
            });
        }

        private static List<InventoryItem> GetFishInInventory(Player player)
        {
            Inventory? inventory = player.GetPlayerCurrentlyPlayingAsCharacter().GetInventory();
            if (inventory == null)
            {
                return new List<InventoryItem>();
            }
            return inventory.GetInventoryItems().Where(inventoryItem => inventoryItem.GetItem().IsItemFish()).ToList();
        }

        /// <summary>
        /// The list is capped, because a dialog can only hold so much text.
        /// </summary>
        private static Dictionary<int, InventoryItem> AddFishRows(BetterTablistDialog dialog, List<InventoryItem> fish)
        {
            Dictionary<int, InventoryItem> fishByRow = new Dictionary<int, InventoryItem>();
            foreach (InventoryItem inventoryItem in fish.Take(FishingSettings.MaxNetStorageDialogRows))
            {
                int row = dialog.AddRow(inventoryItem.GetName(), FishWeightText.GetWeightText(inventoryItem.GetWeight()));
                fishByRow.Add(row, inventoryItem);
            }
            return fishByRow;
        }

        private void TakeFish(Player player, Vehicle reefer, VehicleFishingNet net, InventoryItem fish)
        {
            if (!CanStillUseNet(player, reefer, net))
            {
                return;
            }

            SendMoveResult(player, _fishingNetService.TakeFishFromNet(player, net, fish), "You took the fish from the net.");
            ShowNetStorage(player, reefer, net);
        }

        private void PutFish(Player player, Vehicle reefer, VehicleFishingNet net, InventoryItem fish)
        {
            if (!CanStillUseNet(player, reefer, net))
            {
                return;
            }

            SendMoveResult(player, _fishingNetService.PutFishInNet(player, net, fish), "You put the fish in the net.");
            ShowNetStorage(player, reefer, net);
        }

        /// <summary>
        /// The player can walk off, or the net can be removed, while a dialog is open.
        /// </summary>
        private bool CanStillUseNet(Player player, Vehicle reefer, VehicleFishingNet net)
        {
            if (net.IsComponentAlive && !net.IsDeleted() && _fishingNetService.IsPlayerAtVehicle(player, reefer))
            {
                return true;
            }

            player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, LeftTheBoatError);
            return false;
        }

        private static void SendMoveResult(Player player, string? error, string successMessage)
        {
            if (error != null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, error);
                return;
            }
            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, successMessage);
        }
    }
}
