using OpenRP.Framework.Features.Radios.Components;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Radios.Services.Dialogs
{
    public interface IRadioDialogService
    {
        /// <summary>
        /// Opens /radio: power, frequency and earpiece, after asking which radio when the player can use several.
        /// </summary>
        void OpenRadio(Player player);

        /// <summary>
        /// Opens the menu of this radio, for example after "Use" in the inventory, when the player can still use it.
        /// </summary>
        void OpenRadio(Player player, Radio radio);

        /// <summary>
        /// Opens /editfrequencies: the protected and encrypted frequencies and their factions.
        /// </summary>
        void OpenFrequencyEditor(Player player);
    }
}
