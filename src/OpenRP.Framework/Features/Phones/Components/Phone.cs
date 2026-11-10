using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Shared.BaseManager.Entities;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.Phones.Components
{
    /// <summary>
    /// A mobile phone with its own number. Its item in an inventory points to it.
    /// </summary>
    public class Phone : Component, IBaseDataComponent, IChangeable, IDeletable
    {
        private readonly PhoneModel _model;
        private bool _hasChanges;
        private bool _isDeleted;

        public Phone(PhoneModel model)
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

        public PhoneModel GetRawPhoneModel()
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

        public bool IsOn()
        {
            return _model.IsOn;
        }

        public void SetOn(bool isOn)
        {
            _model.IsOn = isOn;
            ProcessChanges();
        }

        /// <summary>
        /// A silent phone doesn't ring or beep out loud; only its carrier notices.
        /// </summary>
        public bool IsSilent()
        {
            return _model.IsSilent;
        }

        public void SetSilent(bool isSilent)
        {
            _model.IsSilent = isSilent;
            ProcessChanges();
        }
    }
}
