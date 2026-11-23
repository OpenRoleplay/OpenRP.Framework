using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Shared.BaseManager.Entities;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.Jurisdictions.Components
{
    /// <summary>
    /// A setting of a jurisdiction: its own value, and for a country, whether it binds its sub-jurisdictions to it.
    /// </summary>
    public class JurisdictionSetting : Component, IBaseDataComponent, IChangeable, IDeletable
    {
        private readonly JurisdictionSettingModel _settingModel;
        private bool _hasChanges;
        private bool _isDeleted;

        public JurisdictionSetting(JurisdictionSettingModel settingModel)
        {
            _settingModel = settingModel;
        }

        public bool HasChanges()
        {
            return _hasChanges;
        }

        public void ProcessChanges(bool processChanges = true)
        {
            _hasChanges = processChanges;
        }

        public bool IsDeleted()
        {
            return _isDeleted;
        }

        public void ProcessDeletion(bool processDeletion = true)
        {
            _isDeleted = processDeletion;
        }

        public JurisdictionSettingModel GetRawJurisdictionSettingModel()
        {
            return _settingModel;
        }

        public ulong GetId()
        {
            return _settingModel.Id;
        }

        public ulong GetJurisdictionId()
        {
            return _settingModel.JurisdictionId;
        }

        public string GetKey()
        {
            return _settingModel.Key;
        }

        /// <summary>
        /// The value the jurisdiction has of its own, or null when it inherits one.
        /// </summary>
        public string? GetStoredValue()
        {
            return _settingModel.Value;
        }

        public bool HasStoredValue()
        {
            return _settingModel.Value != null;
        }

        public void SetStoredValue(string? storedValue)
        {
            _settingModel.Value = storedValue;
            ProcessChanges();
        }

        public bool GetBindsSubJurisdictions()
        {
            return _settingModel.BindsSubJurisdictions;
        }

        public void SetBindsSubJurisdictions(bool bindsSubJurisdictions)
        {
            _settingModel.BindsSubJurisdictions = bindsSubJurisdictions;
            ProcessChanges();
        }
    }
}
