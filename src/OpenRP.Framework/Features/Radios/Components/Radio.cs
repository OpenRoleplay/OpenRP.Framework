using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Shared.BaseManager.Entities;
using OpenRP.Framework.Shared.Batteries.Entities;
using SampSharp.Entities;
using System;

namespace OpenRP.Framework.Features.Radios.Components
{
    /// <summary>
    /// A radio, tuned to one frequency. A handheld's item points to it; a vehicle radio belongs to its vehicle.
    /// </summary>
    public class Radio : Component, IBaseDataComponent, IChangeable, IDeletable
    {
        private readonly RadioModel _model;
        // Runtime only: a restart ends a recharge.
        private BatteryCharge? _charge;
        private bool _hasChanges;
        private bool _isDeleted;

        public Radio(RadioModel model)
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

        public RadioModel GetRawRadioModel()
        {
            return _model;
        }

        public ulong GetId()
        {
            return _model.Id;
        }

        public uint GetFrequency()
        {
            return _model.Frequency;
        }

        public void SetFrequency(uint frequency)
        {
            _model.Frequency = frequency;
            ProcessChanges();
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
        /// Whether the holder chose the earpiece. It only keeps the radio quiet while they carry an earpiece item.
        /// </summary>
        public bool UsesEarpiece()
        {
            return _model.UsesEarpiece;
        }

        public void SetUsesEarpiece(bool usesEarpiece)
        {
            _model.UsesEarpiece = usesEarpiece;
            ProcessChanges();
        }

        /// <summary>
        /// The vehicle the radio is installed in, or null for a handheld.
        /// </summary>
        public ulong? GetVehicleId()
        {
            return _model.VehicleId;
        }

        public bool IsVehicleRadio()
        {
            return _model.VehicleId.HasValue;
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
