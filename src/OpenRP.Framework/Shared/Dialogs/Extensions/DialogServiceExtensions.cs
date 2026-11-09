using SampSharp.Entities.SAMP;
using System;

namespace OpenRP.Framework.Shared.Dialogs.Extensions
{
    public static class DialogServiceExtensions
    {
        /// <summary>
        /// Shows the dialog, and calls the handler when the player answers it, but not when it closes because the
        /// player disconnected. Use it instead of Show: a handler that kicks or messages a player who already left
        /// crashes the server.
        /// </summary>
        public static void ShowIgnoringDisconnect<TResponse>(this IDialogService dialogService, Player player, IDialog<TResponse> dialog, Action<TResponse> handler)
            where TResponse : struct
        {
            DisconnectAwareDialog<TResponse> disconnectAwareDialog = new DisconnectAwareDialog<TResponse>(dialog);
            dialogService.Show(player, disconnectAwareDialog, response =>
            {
                if (disconnectAwareDialog.IsClosedByDisconnect)
                {
                    return;
                }
                handler(response);
            });
        }
    }
}
