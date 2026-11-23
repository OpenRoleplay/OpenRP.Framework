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

        /// <summary>
        /// Opens the government menu of the player's faction: the settings of the jurisdictions it governs, or a picker when
        /// there are several. Only the leaders of the governing faction get this far.
        /// </summary>
        void OpenGovernment(Player player);

        /// <summary>
        /// Opens the settings of a jurisdiction for an admin, who may change any of them. onBack returns to the caller.
        /// </summary>
        void OpenJurisdictionSettings(Player player, Jurisdiction jurisdiction, Action onBack);
    }
}
