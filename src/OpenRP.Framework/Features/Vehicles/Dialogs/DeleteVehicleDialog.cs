using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Dialogs.Extensions;
using SampSharp.Entities.SAMP;
using System;

namespace OpenRP.Framework.Features.Vehicles.Dialogs
{
    /// <summary>
    /// The confirmation before a saved vehicle is deleted for good.
    /// </summary>
    public static class DeleteVehicleDialog
    {
        public static void Open(Player player, IDialogService dialogService, string content, Action onConfirmed)
        {
            MessageDialog confirmDialog = new MessageDialog(ChatColor.CornflowerBlue + "Delete Vehicle", ChatColor.White + content, "Delete", "Cancel");

            dialogService.ShowIgnoringDisconnect(player, confirmDialog, response =>
            {
                if (response.Response == DialogResponse.LeftButton)
                {
                    onConfirmed();
                }
            });
        }
    }
}
