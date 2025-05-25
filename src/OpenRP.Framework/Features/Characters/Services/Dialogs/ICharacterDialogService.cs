using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Characters.Services.Dialogs
{
    public interface ICharacterDialogService
    {
        void OpenCharacterSelection(Player player, Action onGoBack);
    }
}