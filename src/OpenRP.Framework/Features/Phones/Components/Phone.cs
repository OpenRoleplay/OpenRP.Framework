using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Shared.BaseManager.Entities;
using OpenRP.Framework.Shared.Batteries.Entities;
using SampSharp.Entities;
using System;

namespace OpenRP.Framework.Features.Phones.Components
{
    /// <summary>
    /// A mobile phone with its own number. Its item in an inventory points to it.
    /// </summary>
    public class Phone : Component, IBaseDataComponent, IChangeable, IDeletable
    {
        private readonly PhoneModel _model;
        // Runtime only: a restart ends a recharge.
        private BatteryCharge? _charge;
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

        public double GetBatteryLevel()
        {
            return _model.BatteryLevel;
        }

        public DateTime? GetBatteryUpdatedAt()
        {
            return _model.BatteryUpdatedAt;
        }

        /// <summary>
        /// Sets the battery level and when it was worked out.
        /// </summary>
        public void SetBattery(double level, DateTime updatedAt)
        {
            _model.BatteryLevel = level;
            _model.BatteryUpdatedAt = updatedAt;
            ProcessChanges();
        }

        public bool IsCharging()
        {
            return _charge != null;
        }

        /// <summary>
        /// The recharge in progress, or null.
        /// </summary>
        public BatteryCharge? GetCharge()
        {
            return _charge;
        }

        public void BeginCharging(BatteryCharge charge)
        {
            _charge = charge;
        }

        public void EndCharging()
        {
            _charge = null;
        }
    }
}
