using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Factions.Services.Dialogs
{
    public interface IFactionDialogService
    {
        /// <summary>
        /// Opens /faction: the faction menu, after asking which faction when the player can open several.
        /// </summary>
        void OpenFactions(Player player);

        /// <summary>
        /// Asks the player to accept or decline their pending faction invitation.
        /// </summary>
        void OpenInvitation(Player player);
    }
}
