using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Database.Services;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.DroppedItems.Services;
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
        private IChatService _chatService;
        private IDroppedItemService _droppedItemService;
        public InventoryDialogService(IDialogService dialogService, IChatService chatService, IDroppedItemService droppedItemService)
        {
            _dialogService = dialogService;
            _chatService = chatService;
            _droppedItemService = droppedItemService;
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
                        Inventory? itemInventory = inventoryOpened.SelectedInventoryItem.GetItemInventory();
                        if (itemInventory == null)
                        {
                            player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "This wallet can't be opened right now.");
                            return;
                        }
                        OpenInventory(player, itemInventory);
                    };

                    actions["Set As Default"] = () =>
                    {
                        SetWalletAsDefault(player, inventoryOpened.OpenedInventory, inventoryOpened.SelectedInventoryItem);
                    };
                }
                else if (item.IsItemSkin())
                {
                    actions["Wear"] = () =>
                    {
                        WearSkin(player, character, inventoryOpened.OpenedInventory, inventoryOpened.SelectedInventoryItem);
                    };
                }
                else if (item.IsItemAttachment())
                {
                    actions["Wear"] = () =>
                    {
                        // Wearing attachments isn't implemented yet.
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
                    _dialogService.Show(player, messageDialog, _ => { });
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
                    InventoryItem selectedInventoryItem = inventoryOpened.SelectedInventoryItem;
                    actions["Drop"] = () =>
                    {
                        BetterInputDialog dropAmountDialog = new BetterInputDialog("Drop", "Go Back");
                        dropAmountDialog.SetTitle(TitleType.Children, item.GetName(), "Drop");
                        dropAmountDialog.SetContent($"You can enter the amount of the item you want to drop here.\r\n\r\n{ChatColor.CornflowerBlue}Note:{ChatColor.White} If you want to drop all items, just type in the maximum amount ({ChatColor.Highlight}{selectedInventoryItem.GetAmount()}{ChatColor.White}) you have.");

                        // async void because dialog callbacks can't be awaited; DroppedItemService catches and logs its own errors.
                        async void DropAmountDialog(InputDialogResponse r)
                        {
                            if (r.Response == DialogResponse.RightButtonOrCancel)
                            {
                                return;
                            }

                            if (uint.TryParse(r.InputText, out uint amount) && await _droppedItemService.DropItem(player, selectedInventoryItem, amount))
                            {
                                return;
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

        private static void SetWalletAsDefault(Player player, Inventory inventory, InventoryItem? wallet)
        {
            if (wallet == null)
            {
                return;
            }

            inventory.SetDefaultWallet(wallet);
            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, $"Your {wallet.GetName()} is now your default wallet.");
        }

        private void WearSkin(Player player, Character character, Inventory inventory, InventoryItem? skin)
        {
            int? skinModelId = skin?.GetSkinModelId();
            if (skin == null || skinModelId == null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "An unknown problem occurred and we could not change your clothes.");
                return;
            }

            inventory.SetWornSkin(skin);
            character.SetSkin(skinModelId.Value);
            player.Skin = skinModelId.Value;

            _chatService.SendPlayerChatMessage(player, PlayerChatMessageType.ME, "changed their clothes.");
            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, "You have successfully changed your clothes.");
        }
    }
}
