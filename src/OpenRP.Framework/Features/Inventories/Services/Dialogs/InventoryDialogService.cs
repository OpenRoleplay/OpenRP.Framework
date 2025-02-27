using OpenRP.Framework.Database.Services;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Inventories.Components;
using OpenRP.Framework.Features.Inventories.Enums;
using OpenRP.Framework.Features.Inventories.Helpers;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Chat.Services;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using SampSharp.ColAndreas.Entities.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Inventories.Services.Dialogs
{
    public class InventoryDialogService
    {
        private IDialogService _dialogService;
        public InventoryDialogService(IDialogService dialogService) 
        {
            _dialogService = dialogService;
        }

        public void OpenInventory(Player player, Inventory inventory, InventoryArguments inventoryArguments = InventoryArguments.None)
        {
            InventoryOpened openedInventory = player.AddComponent<InventoryOpened>(inventory);

            List<InventoryItem> inventoryItems = inventory.GetInventoryItems();
            Character character = player.GetPlayerCurrentlyPlayingAsCharacter();

            // Headers
            List<string> inventoryHeaders = new List<string>();

            inventoryHeaders.Add("Item");
            inventoryHeaders.Add("Amount");

            if(!inventoryArguments.HasFlag(InventoryArguments.HideTotalWeight))
            {
                inventoryHeaders.Add("Total Weight");
            }

            if (!inventoryArguments.HasFlag(InventoryArguments.HideExtraInformation))
            {
                inventoryHeaders.Add("Extra Information");
            }

            // Add parent inventory as prefix
            StringBuilder inventoryNameBuilder = new StringBuilder();
            Inventory parentInventory = inventory.GetParentInventory();
            if (parentInventory != null)
            {
                inventoryNameBuilder.Append(parentInventory.GetInventoryDialogName(false));
                inventoryNameBuilder.Append(ChatColor.CornflowerBlue + " -> " + ChatColor.White);
            }
            inventoryNameBuilder.Append(inventory.GetInventoryDialogName());

            // Build dialog
            BetterTablistDialog tablistDialog = new BetterTablistDialog("Details", "Cancel", inventoryHeaders.Count);
            tablistDialog.SetTitle(TitleType.Children, inventoryNameBuilder.ToString());
            tablistDialog.AddHeaders(inventoryHeaders.ToArray());

            foreach (InventoryItem inventoryItem in inventoryItems)
            {
                // Gather Extra Information
                List<string> extraInformation = new List<string>();
                ItemAdditionalData itemAdditionalData = inventoryItem.GetAdditionalData();

                if (inventoryItem.GetItem().IsItemSkin() && itemAdditionalData.GetBoolean("WEARING") != null & itemAdditionalData.GetBoolean("WEARING") == true)
                {
                    extraInformation.Add("Wearing");
                }

                if (inventoryItem.GetItem().IsItemAttachment() && itemAdditionalData.GetBoolean("WEARING") != null & itemAdditionalData.GetBoolean("WEARING") == true)
                {
                    extraInformation.Add("Wearing");
                }

                if (inventoryItem.HasMaxUses())
                {
                    extraInformation.Add($"{inventoryItem.GetRemainingUses()}/{inventoryItem.GetItem().GetMaxUses()} Durability");
                }

                // Fill Columns
                List<string> rowColumns = new List<string>();

                rowColumns.Add(inventoryItem.GetName());
                rowColumns.Add(inventoryItem.GetAmount().ToString());

                if (!inventoryArguments.HasFlag(InventoryArguments.HideTotalWeight))
                {
                    rowColumns.Add(String.Format("{0}g", inventoryItem.GetTotalWeight()));
                }

                if (!inventoryArguments.HasFlag(InventoryArguments.HideExtraInformation))
                {
                    rowColumns.Add(String.Join(ChatColor.CornflowerBlue + " | " + ChatColor.White, extraInformation));
                }

                // Add Row To Tablist Dialog & Item To
                int createdIndex = tablistDialog.AddRow(rowColumns.ToArray());
                openedInventory.OpenedInventoryItems.Add(createdIndex, inventoryItem);
            }

            void InventoryItemsDialogHandler(TablistDialogResponse r)
            {
                if (r.Response == DialogResponse.LeftButton)
                {
                    if(openedInventory.OpenedInventoryItems.TryGetValue(r.ItemIndex, out InventoryItem selectedInventoryItem))
                    {
                        openedInventory.OpenedInventoryItem = selectedInventoryItem;
                    }

                    // Open Item Options Dialog here
                }
                else
                {
                    player.DestroyComponents<InventoryOpened>();
                }
            }

            _dialogService.Show(player, tablistDialog, InventoryItemsDialogHandler);
        }
    }
}
