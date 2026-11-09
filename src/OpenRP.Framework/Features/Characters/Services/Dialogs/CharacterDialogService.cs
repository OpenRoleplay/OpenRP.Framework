using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using SampSharp.Entities.SAMP;
using OpenRP.Framework.Shared.Dialogs.Extensions;

namespace OpenRP.Framework.Features.Characters.Services.Dialogs
{
    /// <summary>
    /// All dialogs of the Characters feature. The flows live in separate files:
    /// Selection, Creation, Preferences and Traits.
    /// </summary>
    public partial class CharacterDialogService : ICharacterDialogService
    {
        private readonly IDialogService _dialogService;
        private readonly ICharacterService _characterService;

        public CharacterDialogService(IDialogService dialogService, ICharacterService characterService)
        {
            _dialogService = dialogService;
            _characterService = characterService;
        }

        /// <summary>
        /// Shows a message with a single button and runs onClose when the player closes it.
        /// </summary>
        private void ShowMessage(Player player, string button, string content, Action onClose, params string[] title)
        {
            BetterMessageDialog messageDialog = new BetterMessageDialog(button);
            messageDialog.SetTitle(TitleType.Children, title);
            messageDialog.SetContent(content);

            _dialogService.ShowIgnoringDisconnect(player, messageDialog, (MessageDialogResponse response) => onClose());
        }
    }
}
