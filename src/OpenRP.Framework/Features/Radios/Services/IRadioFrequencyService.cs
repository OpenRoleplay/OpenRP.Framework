using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Features.Radios.Components;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Radios.Services
{
    /// <summary>
    /// The rules of the frequencies: who may listen and who may talk.
    /// </summary>
    public interface IRadioFrequencyService
    {
        /// <summary>
        /// The protected or encrypted frequency with this number, or null when the default rules apply.
        /// </summary>
        RadioFrequency? GetFrequency(uint number);

        /// <summary>
        /// Every protected and encrypted frequency, lowest number first.
        /// </summary>
        List<RadioFrequency> GetFrequencies();

        /// <summary>
        /// The factions allowed on the frequency, each with the factions it controls.
        /// </summary>
        List<RadioFrequencyFaction> GetFrequencyFactions(RadioFrequency frequency);

        /// <summary>
        /// Whether the character may hear the frequency. Pass null for a radio nobody holds.
        /// </summary>
        bool CanListen(uint number, Character? character);

        /// <summary>
        /// Why the character can't talk on the frequency, or null when they can.
        /// </summary>
        string? GetTalkError(uint number, Character character);

        /// <summary>
        /// The frequency for messages, for example "911 (911 Dispatch)" or "1500".
        /// </summary>
        string GetLabel(uint number);

        /// <summary>
        /// The faction an allowed-faction row points to, or null when it isn't loaded.
        /// </summary>
        Faction? GetFaction(RadioFrequencyFaction frequencyFaction);
    }
}
