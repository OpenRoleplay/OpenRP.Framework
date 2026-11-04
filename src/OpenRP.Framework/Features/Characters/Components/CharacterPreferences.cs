using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Shared.BaseManager.Entities;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.Characters.Components
{
    /// <summary>
    /// The consent and gameplay preferences of the character a player is playing as.
    /// </summary>
    public class CharacterPreferences : Component, IChangeable
    {
        private readonly CharacterPreferencesModel _characterPreferencesModel;
        private bool _hasChanges;

        public CharacterPreferences(CharacterPreferencesModel characterPreferencesModel)
        {
            _characterPreferencesModel = characterPreferencesModel;
        }

        public bool HasChanges()
        {
            return _hasChanges;
        }

        public void ProcessChanges(bool processChanges = true)
        {
            _hasChanges = processChanges;
        }

        public CharacterPreferencesModel GetRawCharacterPreferencesModel()
        {
            return _characterPreferencesModel;
        }

        public bool IsHardcoreModeEnabled()
        {
            return _characterPreferencesModel.HardcoreMode;
        }

        /// <summary>
        /// Turns on Hardcore Mode, which consents to every interaction. It cannot be turned off again.
        /// </summary>
        public void EnableHardcoreMode()
        {
            _characterPreferencesModel.HardcoreMode = true;
            _characterPreferencesModel.AllowCharacterKill = true;
            _characterPreferencesModel.AllowRape = true;
            _characterPreferencesModel.AllowSlavery = true;
            _characterPreferencesModel.AllowWorldEvents = true;
            ProcessChanges();
        }

        /// <summary>
        /// Consent settings are locked while Hardcore Mode is on.
        /// </summary>
        public bool CanChangeConsent()
        {
            return !IsHardcoreModeEnabled();
        }

        public bool IsCharacterKillAllowed()
        {
            return _characterPreferencesModel.AllowCharacterKill;
        }

        public void SetCharacterKillAllowed(bool isAllowed)
        {
            _characterPreferencesModel.AllowCharacterKill = isAllowed;
            ProcessChanges();
        }

        public bool IsRapeAllowed()
        {
            return _characterPreferencesModel.AllowRape;
        }

        public void SetRapeAllowed(bool isAllowed)
        {
            _characterPreferencesModel.AllowRape = isAllowed;
            ProcessChanges();
        }

        public bool IsSlaveryAllowed()
        {
            return _characterPreferencesModel.AllowSlavery;
        }

        public void SetSlaveryAllowed(bool isAllowed)
        {
            _characterPreferencesModel.AllowSlavery = isAllowed;
            ProcessChanges();
        }

        public bool IsWorldEventsAllowed()
        {
            return _characterPreferencesModel.AllowWorldEvents;
        }

        public void SetWorldEventsAllowed(bool isAllowed)
        {
            _characterPreferencesModel.AllowWorldEvents = isAllowed;
            ProcessChanges();
        }

        public ulong GetDefaultCurrencyId()
        {
            return _characterPreferencesModel.DefaultCurrencyId;
        }

        /// <summary>
        /// The "Blessed by Misfortune" trait is the reward for playing in Hardcore Mode.
        /// </summary>
        public bool HasBlessedByMisfortuneTrait()
        {
            return IsHardcoreModeEnabled();
        }
    }
}
