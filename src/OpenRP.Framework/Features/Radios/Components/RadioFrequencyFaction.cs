using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Shared.BaseManager.Entities;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.Radios.Components
{
    /// <summary>
    /// A faction allowed on a protected or encrypted frequency.
    /// </summary>
    public class RadioFrequencyFaction : Component, IBaseDataComponent, IChangeable, IDeletable
    {
        private readonly RadioFrequencyFactionModel _model;
        private bool _hasChanges;
        private bool _isDeleted;

        public RadioFrequencyFaction(RadioFrequencyFactionModel model)
        {
            _model = model;
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

        public RadioFrequencyFactionModel GetRawRadioFrequencyFactionModel()
        {
            return _model;
        }

        public ulong GetId()
        {
            return _model.Id;
        }

        public ulong GetRadioFrequencyId()
        {
            return _model.RadioFrequencyId;
        }

        public ulong GetFactionId()
        {
            return _model.FactionId;
        }
    }
}
