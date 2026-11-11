using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Features.Radios.Components;
using OpenRP.Framework.Features.Radios.Enums;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Radios.Services
{
    /// <summary>
    /// Changes radios and frequencies. Methods return why something isn't possible, or null when it worked; changes are
    /// saved with the next radio sync.
    /// </summary>
    public interface IRadioEditorService
    {
        /// <summary>
        /// Puts a new handheld radio, tuned to 1000, in the inventory of the character the player plays.
        /// </summary>
        (Radio? Radio, string? Error) GiveRadio(Player player);

        /// <summary>
        /// Puts a radio earpiece in the inventory of the character the player plays.
        /// </summary>
        string? GiveEarpiece(Player player);

        void SetPower(Radio radio, bool isOn);

        /// <summary>
        /// Tunes the radio to the frequency in the text.
        /// </summary>
        string? SetFrequency(Radio radio, string frequencyText);

        void SetUsesEarpiece(Radio radio, bool usesEarpiece);

        (RadioFrequency? Frequency, string? Error) CreateFrequency(string numberText, string name, RadioFrequencyMode mode);

        /// <summary>
        /// Creates the frequency unless one with that number exists. Returns the frequency, or null when it couldn't be
        /// created.
        /// </summary>
        RadioFrequency? CreateFrequencyIfNotExists(uint number, string name, RadioFrequencyMode mode);

        string? RenameFrequency(RadioFrequency frequency, string name);

        void SetFrequencyMode(RadioFrequency frequency, RadioFrequencyMode mode);

        /// <summary>
        /// Deletes the frequency and its factions. Its number goes back to the default rules.
        /// </summary>
        void DeleteFrequency(RadioFrequency frequency);

        string? AddFrequencyFaction(RadioFrequency frequency, Faction faction);

        void RemoveFrequencyFaction(RadioFrequencyFaction frequencyFaction);

        /// <summary>
        /// Installs a radio, tuned to 1000, in the saved vehicle with this id.
        /// </summary>
        string? InstallVehicleRadio(ulong vehicleId);

        void RemoveVehicleRadio(ulong vehicleId);
    }
}
