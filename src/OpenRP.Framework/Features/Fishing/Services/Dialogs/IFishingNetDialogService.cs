using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Fishing.Services.Dialogs
{
    public interface IFishingNetDialogService
    {
        /// <summary>
        /// Shows the catch in the net of the Reefer the player is on or next to.
        /// </summary>
        void OpenNetStorage(Player player);
    }
}
