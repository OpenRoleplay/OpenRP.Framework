using OpenRP.Framework.Features.Jurisdictions.Components;
using SampSharp.Entities.SAMP;
using System;

namespace OpenRP.Framework.Features.Jurisdictions.Services.Dialogs
{
    public interface IJurisdictionDialogService
    {
        /// <summary>
        /// Opens the admin menu for creating and managing jurisdictions and their tax registrations.
        /// </summary>
        void OpenJurisdictionEditor(Player player);

        /// <summary>
        /// Lets the player pick a jurisdiction from the tree, or none (null) through the first row, which shows
        /// noneLabel. Other features' editors use it, for example for a property's jurisdiction.
        /// </summary>
        void OpenJurisdictionPicker(Player player, string title, string noneLabel, Action<Jurisdiction?> onPicked, Action onBack);
    }
}
