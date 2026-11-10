using OpenRP.Framework.Database.Models;
using System;
using OpenRP.Framework.Shared.BaseManager.Entities;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.Phones.Components
{
    /// <summary>
    /// A text in a phone's memory, received or sent.
    /// </summary>
    public class PhoneMessage : Component, IBaseDataComponent, IChangeable, IDeletable
    {
        private readonly PhoneMessageModel _model;
        private bool _hasChanges;
        private bool _isDeleted;

        public PhoneMessage(PhoneMessageModel model)
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

        public PhoneMessageModel GetRawPhoneMessageModel()
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

        /// <summary>
        /// The number the text came from, or went to.
        /// </summary>
        public uint GetOtherNumber()
        {
            return _model.OtherNumber;
        }

        public string GetText()
        {
            return _model.Text;
        }

        public bool IsIncoming()
        {
            return _model.IsIncoming;
        }

        public bool IsRead()
        {
            return _model.IsRead;
        }

        public void SetRead()
        {
            _model.IsRead = true;
            ProcessChanges();
        }

        public DateTime GetSentAt()
        {
            return _model.SentAt;
        }
    }
}
