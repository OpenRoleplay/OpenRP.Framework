using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Radios.Enums;
using OpenRP.Framework.Shared.BaseManager.Entities;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.Radios.Components
{
    /// <summary>
    /// A protected or encrypted frequency, with its own rules about who may listen and talk.
    /// </summary>
    public class RadioFrequency : Component, IBaseDataComponent, IChangeable, IDeletable
    {
        private readonly RadioFrequencyModel _model;
        private bool _hasChanges;
        private bool _isDeleted;

        public RadioFrequency(RadioFrequencyModel model)
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

        public RadioFrequencyModel GetRawRadioFrequencyModel()
        {
            return _model;
        }

        public ulong GetId()
        {
            return _model.Id;
        }

        public uint GetNumber()
        {
            return _model.Number;
        }

        public string GetName()
        {
            return _model.Name;
        }

        public void SetName(string name)
        {
            _model.Name = name;
            ProcessChanges();
        }

        public RadioFrequencyMode GetMode()
        {
            return _model.Mode;
        }

        public void SetMode(RadioFrequencyMode mode)
        {
            _model.Mode = mode;
            ProcessChanges();
        }

        public bool IsEncrypted()
        {
            return _model.Mode == RadioFrequencyMode.Encrypted;
        }
    }
}
