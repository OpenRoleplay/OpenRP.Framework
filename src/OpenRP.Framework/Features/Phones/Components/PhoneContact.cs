using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Shared.BaseManager.Entities;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.Phones.Components
{
    /// <summary>
    /// A name in a phone's phone book.
    /// </summary>
    public class PhoneContact : Component, IBaseDataComponent, IChangeable, IDeletable
    {
        private readonly PhoneContactModel _model;
        private bool _hasChanges;
        private bool _isDeleted;

        public PhoneContact(PhoneContactModel model)
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

        public PhoneContactModel GetRawPhoneContactModel()
        {
            return _model;
        }

        public ulong GetId()
        {
            return _model.Id;
        }

        public ulong GetPhoneId()
        {
            return _model.PhoneId;
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

        public uint GetNumber()
        {
            return _model.Number;
        }
    }
}
