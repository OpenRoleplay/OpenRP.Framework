using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Characters.Services.Dialogs
{
    public interface ICharacterDialogService
    {
        /// <summary>
        /// Lets the player pick one of their characters or create a new one. Quitting kicks the player.
        /// </summary>
        void OpenCharacterSelection(Player player, Action onGoBack);
        void OpenCharacterPreferences(Player player);
        void OpenCharacterTraits(Player player);
    }
}
