using OpenRP.Framework.Features.Factions.Components;
using SampSharp.Entities.SAMP;
using System;

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

        /// <summary>
        /// Lets the player pick any faction, or none (null) through the first row when noneLabel isn't null. Used by
        /// admin editors in other features.
        /// </summary>
        void OpenFactionPicker(Player player, string title, string? noneLabel, Action<Faction?> onPicked, Action onBack);
    }
}
