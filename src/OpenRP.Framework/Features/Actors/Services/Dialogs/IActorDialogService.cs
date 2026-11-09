using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Actors.Services.Dialogs
{
    public interface IActorDialogService
    {
        /// <summary>
        /// Opens the admin menu for creating and changing actors.
        /// </summary>
        void OpenActorEditor(Player player);
    }
}
