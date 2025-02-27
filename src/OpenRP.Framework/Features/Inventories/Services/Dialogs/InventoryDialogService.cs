using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Database.Services;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Inventories.Components;
using OpenRP.Framework.Features.Inventories.Enums;
using OpenRP.Framework.Features.Inventories.Helpers;
using OpenRP.Framework.Features.Items.Components;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Framework.Shared.Chat.Services;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Helpers;
using OpenRP.Framework.Shared.Extensions;
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
    public class InventoryDialogService : IInventoryDialogService
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
            Inventory? parentInventory = inventory.GetParentInventory();
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
                        openedInventory.SelectedInventoryItem = selectedInventoryItem;
                    }

                    OpenSelectedItemDialog(player, openedInventory);
                }
                else
                {
                    player.DestroyComponents<InventoryOpened>();
                }
            }

            _dialogService.Show(player, tablistDialog, InventoryItemsDialogHandler);
        }

        public void OpenSelectedItemDialog(Player player, InventoryOpened inventoryOpened)
        {
            Item item = inventoryOpened.SelectedInventoryItem.GetItem();
            BetterListDialog listDialog = new BetterListDialog("Select", "Cancel");
            listDialog.SetTitle(TitleType.Children, inventoryOpened.SelectedInventoryItem.GetItem().GetName());

            // Build a dictionary mapping action names to actions (delegates)
            Dictionary<string, Action> actions = BuildSelectedItemActions(player, inventoryOpened);

            // Get sorted keys and add them to the dialog
            List<string> actionKeys = actions.Keys.OrderBy(s => s).ToList();
            actionKeys.ForEach(a => listDialog.AddRow(a));

            void InventoryItemSelectedItemActionsDialogHandler(ListDialogResponse response)
            {
                if (response.Response == DialogResponse.LeftButton)
                {
                    string selectedAction = actionKeys[response.ItemIndex];
                    if (actions.TryGetValue(selectedAction, out Action action))
                    {
                        action.Invoke();
                    }
                }
                player.DestroyComponents<InventoryOpened>();
            }

            _dialogService.Show(player, listDialog, InventoryItemSelectedItemActionsDialogHandler);
        }

        private Dictionary<string, Action> BuildSelectedItemActions(Player player, InventoryOpened inventoryOpened)
        {
            var actions = new Dictionary<string, Action>();
            if (inventoryOpened.SelectedInventoryItem != null)
            {
                Item item = inventoryOpened.SelectedInventoryItem.GetItem();
                Character character = player.GetPlayerCurrentlyPlayingAsCharacter();

                if (item.IsItemWallet())
                {
                    actions["Open"] = () =>
                    {
                        // Assuming the wallet's inventory is stored in the item itself.
                        Inventory itemInventory = inventoryOpened.SelectedInventoryItem.GetItemInventory();
                        OpenInventory(player, itemInventory);
                    };

                    actions["Set As Default"] = () =>
                    {
                        if (item.IsItemWallet())
                        {
                            //characterService.SetCharacterDefaultWallet(character, openInventoryComponent.selectedInventoryItem);
                        }
                    };
                }
                else if (item.IsItemSkin() || item.IsItemAttachment())
                {
                    actions["Wear"] = () =>
                    {
                        /*bool success = characterService.SetCharacterWearingInventorySkin(character, openInventoryComponent.selectedInventoryItem);
                        if (success)
                        {
                            chatService.SendPlayerChatMessage(player, PlayerChatMessageType.ME, "changed their clothes.");
                            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, "You have successfully changed your clothes.");
                        }
                        else
                        {
                            player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "An unknown problem occurred and we could not change your clothes.");
                        }*/
                    };
                }
                else
                {
                    actions["Use"] = () =>
                    {
                        inventoryOpened.OpenedInventory.UseItem(player, inventoryOpened.SelectedInventoryItem);
                    };
                }

                if (item.IsItemAttachment())
                {
                    actions["Edit"] = () =>
                    {
                        // Implement edit logic as needed.
                    };
                }

                actions["Description"] = () =>
                {
                    string description = String.IsNullOrEmpty(item.GetDescription()) ? "This item does not have a description." : item.GetDescription();

                    BetterMessageDialog messageDialog = new BetterMessageDialog("Close");
                    messageDialog.SetTitle(TitleType.Children, inventoryOpened.OpenedInventory.GetName(), inventoryOpened.SelectedInventoryItem.GetName(), "Description");
                    messageDialog.SetContent(description);
                    _dialogService.Show(player, messageDialog);
                };

                if (item.IsDestroyable())
                {
                    actions["Destroy"] = () =>
                    {
                        // Implement destroy logic here.
                    };
                }

                if (item.IsDroppable())
                {
                    actions["Drop"] = () =>
                    {
                        BetterInputDialog dropAmountDialog = new BetterInputDialog("Drop", "Go Back");
                        dropAmountDialog.SetTitle(TitleType.Children, item.GetName(), "Drop");
                        dropAmountDialog.SetContent($"You can enter the amount of the item you want to drop here.\r\n\r\n{ChatColor.CornflowerBlue}Note:{ChatColor.White} If you want to drop all items, just type in the maximum amount ({ChatColor.Highlight}{inventoryOpened.SelectedInventoryItem.GetAmount()}{ChatColor.White}) you have.");

                        void DropAmountDialog(InputDialogResponse r)
                        {
                            if (r.Response == DialogResponse.RightButtonOrCancel)
                            {
                                _dialogService.Show(player, dropAmountDialog, DropAmountDialog);
                                return;
                            }

                            if (int.TryParse(r.InputText, out int amount) && amount > 0)
                            {
                                bool success = false;//TODO: port over droppedItemService.DropItem(player, openInventoryComponent.selectedInventoryItem, amount);
                                if (success)
                                {
                                    return;
                                }
                            }

                            BetterMessageDialog invalidAmountDialog = new BetterMessageDialog("Ok");
                            invalidAmountDialog.SetTitle(TitleType.Children, item.GetName(), "Drop", "Invalid Drop Amount");
                            invalidAmountDialog.SetContent($"{ChatColor.Highlight}The drop item amount you provided is invalid!{ChatColor.White} Please ensure you enter a positive number that doesn't exceed the amount of items in your inventory.");

                            void DropAmountInvalid(MessageDialogResponse r)
                            {
                                _dialogService.Show(player, dropAmountDialog, DropAmountDialog);
                            }

                            _dialogService.Show(player, invalidAmountDialog, DropAmountInvalid);
                        }

                        _dialogService.Show(player, dropAmountDialog, DropAmountDialog);
                    };
                }
            }

            return actions;
        }
    }
}
