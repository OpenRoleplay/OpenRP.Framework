using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Phones.Enums;
using System;
using OpenRP.Framework.Shared.BaseManager.Entities;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.Phones.Components
{
    /// <summary>
    /// A call in a phone's call log: received, dialed or missed.
    /// </summary>
    public class PhoneCallLog : Component, IBaseDataComponent, IChangeable, IDeletable
    {
        private readonly PhoneCallLogModel _model;
        private bool _hasChanges;
        private bool _isDeleted;

        public PhoneCallLog(PhoneCallLogModel model)
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

        public PhoneCallLogModel GetRawPhoneCallLogModel()
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

        public uint GetOtherNumber()
        {
            return _model.OtherNumber;
        }

        public PhoneCallLogType GetCallType()
        {
            return _model.Type;
        }

        public DateTime GetStartedAt()
        {
            return _model.StartedAt;
        }

        public int GetDurationSeconds()
        {
            return _model.DurationSeconds;
        }

        /// <summary>
        /// A missed call stays new until the call log is opened.
        /// </summary>
        public bool IsSeen()
        {
            return _model.IsSeen;
        }

        public void SetSeen()
        {
            _model.IsSeen = true;
            ProcessChanges();
        }
    }
}
