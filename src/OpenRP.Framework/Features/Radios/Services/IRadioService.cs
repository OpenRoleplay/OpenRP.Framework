using OpenRP.Framework.Features.Radios.Components;
using OpenRP.Framework.Features.Radios.Entities;
using SampSharp.Entities.SAMP;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Radios.Services
{
    /// <summary>
    /// Finds radios: the ones a player carries, the one in their vehicle, and every radio that's playing a frequency.
    /// </summary>
    public interface IRadioService
    {
        Radio? GetRadio(ulong radioId);

        /// <summary>
        /// The handheld radios directly in the inventory of the character the player plays.
        /// </summary>
        List<Radio> GetCarriedRadios(Player player);

        bool IsCarrying(Player player, Radio radio);

        /// <summary>
        /// Whether the character the player plays carries a radio earpiece.
        /// </summary>
        bool HasEarpiece(Player player);

        /// <summary>
        /// The radio installed in the vehicle with this id, or null.
        /// </summary>
        Radio? GetVehicleRadio(ulong vehicleId);

        /// <summary>
        /// The radio installed in the vehicle the player sits in, or null.
        /// </summary>
        Radio? GetRadioOfVehicleIn(Player player);

        /// <summary>
        /// The radios the player can use right now: the ones they carry, then the one in their vehicle.
        /// </summary>
        List<Radio> GetUsableRadios(Player player);

        /// <summary>
        /// The radio /r and /freq use: a carried radio that's on, else the vehicle's radio, else any carried radio.
        /// </summary>
        Radio? GetRadioToUse(Player player);

        /// <summary>
        /// Whether the player can still use the radio: they carry it, or sit in the vehicle it's installed in.
        /// </summary>
        bool CanUse(Player player, Radio radio);

        /// <summary>
        /// Every radio that's on and tuned to the frequency: carried, dropped or installed in a vehicle.
        /// </summary>
        List<RadioSpeaker> GetPlayingRadios(uint frequency);
    }
}
