using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Shared.Dialogs
{
    /// <summary>
    /// Shows another dialog unchanged, and remembers whether it was closed because the player disconnected. SampSharp
    /// still calls the handler then, but the player can no longer be kicked, messaged or shown another dialog.
    /// </summary>
    internal sealed class DisconnectAwareDialog<TResponse> : IDialog<TResponse> where TResponse : struct
    {
        private readonly IDialog<TResponse> _dialog;

        public DisconnectAwareDialog(IDialog<TResponse> dialog)
        {
            _dialog = dialog;
        }

        public bool IsClosedByDisconnect { get; private set; }

        public DialogStyle Style => _dialog.Style;
        public string Caption => _dialog.Caption;
        public string Content => _dialog.Content;
        public string Button1 => _dialog.Button1;
        public string Button2 => _dialog.Button2;

        public TResponse Translate(DialogResult dialogResult)
        {
            IsClosedByDisconnect = dialogResult.Response == DialogResponse.Disconnected;
            return _dialog.Translate(dialogResult);
        }
    }
}
