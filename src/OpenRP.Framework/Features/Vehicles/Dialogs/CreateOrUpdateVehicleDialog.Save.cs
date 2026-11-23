using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Vehicles.Components;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using SampSharp.Entities.SAMP;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Vehicles.Dialogs
{
    /// <summary>
    /// The "Save" row: checks the copy, saves it, then changes the live vehicle or spawns the new one.
    /// </summary>
    public partial class CreateOrUpdateVehicleDialog
    {
        private const int MaxColor = 255;

        private void SaveVehicleData()
        {
            if (!RefreshEditedVehicle())
            {
                _player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "This vehicle was deleted or despawned while you edited it, so nothing was saved.");
                return;
            }

            string? error = GetValidationError(Data) ?? GetLockChangeWaitError();
            if (error != null)
            {
                _player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, error);
                ShowEditor();
                return;
            }

            bool isNewVehicle = Data.Id == 0;
            VehicleModel? originalData = _component.OriginalVehicleData;
            VehicleModel? liveData = isNewVehicle ? null : GetLiveVehicleData();
            if (originalData != null && liveData != null)
            {
                KeepLiveValuesForUneditedSettings(originalData, liveData);
            }

            if (!_vehicleManager.SaveVehicle(Data, _context))
            {
                _player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "The vehicle couldn't be saved.");
                ShowEditor();
                return;
            }

            if (isNewVehicle)
            {
                SpawnSavedVehicle();
            }
            else if (originalData != null)
            {
                ApplySavedChanges(originalData, liveData);
            }

            InstallRadioAfterSave();
            GiveKeyAfterSave();
            SendSavedMessage(isNewVehicle);
        }

        /// <summary>
        /// Another admin can replace (a model change) or delete the vehicle while this editor is open, so the save looks
        /// the live vehicle up again. False when the editor had a live vehicle and it's gone.
        /// </summary>
        private bool RefreshEditedVehicle()
        {
            if (Data.Id == 0 || _component.EditedVehicle == null)
            {
                return true;
            }

            _component.EditedVehicle = _vehicleManager.FindLoadedVehicle(Data.Id);
            return _component.EditedVehicle != null;
        }

        /// <summary>
        /// A lock change that is still being saved would be overwritten by this copy's door state, so the save waits for it.
        /// </summary>
        private string? GetLockChangeWaitError()
        {
            Vehicle? editedVehicle = _component.EditedVehicle;
            return editedVehicle != null && _vehicleManager.IsLockChangeInProgress(editedVehicle) ? "Wait a moment." : null;
        }

        /// <summary>
        /// A snapshot of the live vehicle's data as it is now, or null when there's no live vehicle.
        /// </summary>
        private VehicleModel? GetLiveVehicleData()
        {
            VehicleDataComponent? vehicleDataComponent = _component.EditedVehicle?.GetComponent<VehicleDataComponent>();
            return vehicleDataComponent?.VehicleData.Clone();
        }

        /// <summary>
        /// Every setting the admin didn't change in the editor takes its live value, so a Save never reverts what happened
        /// while the editor was open. The copy then differs from the original only in what the admin changed.
        /// </summary>
        private void KeepLiveValuesForUneditedSettings(VehicleModel originalData, VehicleModel liveData)
        {
            KeepLivePlacementAndState(originalData, liveData);
            KeepLiveDoorsAndWindows(originalData, liveData);
            KeepLiveHealthAndRegistration(originalData, liveData);
        }

        private void KeepLivePlacementAndState(VehicleModel originalData, VehicleModel liveData)
        {
            Data.X = KeepLiveIfUnedited(Data.X, originalData.X, liveData.X);
            Data.Y = KeepLiveIfUnedited(Data.Y, originalData.Y, liveData.Y);
            Data.Z = KeepLiveIfUnedited(Data.Z, originalData.Z, liveData.Z);
            Data.Rotation = KeepLiveIfUnedited(Data.Rotation, originalData.Rotation, liveData.Rotation);
            Data.IsEngineOn = KeepLiveIfUnedited(Data.IsEngineOn, originalData.IsEngineOn, liveData.IsEngineOn);
            Data.AreLightsOn = KeepLiveIfUnedited(Data.AreLightsOn, originalData.AreLightsOn, liveData.AreLightsOn);
            Data.IsAlarmOn = KeepLiveIfUnedited(Data.IsAlarmOn, originalData.IsAlarmOn, liveData.IsAlarmOn);
            Data.AreDoorsLocked = KeepLiveIfUnedited(Data.AreDoorsLocked, originalData.AreDoorsLocked, liveData.AreDoorsLocked);
        }

        private void KeepLiveDoorsAndWindows(VehicleModel originalData, VehicleModel liveData)
        {
            Data.IsBonnetOpen = KeepLiveIfUnedited(Data.IsBonnetOpen, originalData.IsBonnetOpen, liveData.IsBonnetOpen);
            Data.IsBootOpen = KeepLiveIfUnedited(Data.IsBootOpen, originalData.IsBootOpen, liveData.IsBootOpen);
            Data.IsDriverDoorOpen = KeepLiveIfUnedited(Data.IsDriverDoorOpen, originalData.IsDriverDoorOpen, liveData.IsDriverDoorOpen);
            Data.IsPassengerDoorOpen = KeepLiveIfUnedited(Data.IsPassengerDoorOpen, originalData.IsPassengerDoorOpen, liveData.IsPassengerDoorOpen);
            Data.IsBackLeftDoorOpen = KeepLiveIfUnedited(Data.IsBackLeftDoorOpen, originalData.IsBackLeftDoorOpen, liveData.IsBackLeftDoorOpen);
            Data.IsBackRightDoorOpen = KeepLiveIfUnedited(Data.IsBackRightDoorOpen, originalData.IsBackRightDoorOpen, liveData.IsBackRightDoorOpen);
            Data.IsDriverWindowClosed = KeepLiveIfUnedited(Data.IsDriverWindowClosed, originalData.IsDriverWindowClosed, liveData.IsDriverWindowClosed);
            Data.IsPassengerWindowClosed = KeepLiveIfUnedited(Data.IsPassengerWindowClosed, originalData.IsPassengerWindowClosed, liveData.IsPassengerWindowClosed);
            Data.IsBackLeftWindowClosed = KeepLiveIfUnedited(Data.IsBackLeftWindowClosed, originalData.IsBackLeftWindowClosed, liveData.IsBackLeftWindowClosed);
            Data.IsBackRightWindowClosed = KeepLiveIfUnedited(Data.IsBackRightWindowClosed, originalData.IsBackRightWindowClosed, liveData.IsBackRightWindowClosed);
        }

        private void KeepLiveHealthAndRegistration(VehicleModel originalData, VehicleModel liveData)
        {
            Data.CurrentHealth = KeepLiveIfUnedited(Data.CurrentHealth, originalData.CurrentHealth, liveData.CurrentHealth);
            Data.JurisdictionId = KeepLiveIfUnedited(Data.JurisdictionId, originalData.JurisdictionId, liveData.JurisdictionId);
        }

        /// <summary>
        /// The live value when the copy still has the original value, otherwise the admin's edited value.
        /// </summary>
        private static T KeepLiveIfUnedited<T>(T editedValue, T originalValue, T liveValue)
        {
            return EqualityComparer<T>.Default.Equals(editedValue, originalValue) ? liveValue : editedValue;
        }

        /// <summary>
        /// The first save of a vehicle spawns it, with the admin in the driver seat so they can drive it away.
        /// </summary>
        private void SpawnSavedVehicle()
        {
            Vehicle vehicle = _vehicleManager.LoadVehicle(Data);
            _player.PutInVehicle(vehicle);
        }

        /// <summary>
        /// Applies what the admin changed to the live vehicle. A new model needs a new vehicle, so the old one is replaced.
        /// The live data is what the vehicle has now, so only the settings the admin changed differ from it.
        /// </summary>
        private void ApplySavedChanges(VehicleModel originalData, VehicleModel? liveData)
        {
            Vehicle? editedVehicle = _component.EditedVehicle;
            if (editedVehicle == null || liveData == null)
            {
                return;
            }

            if (IsModelChanged(originalData, Data))
            {
                _vehicleManager.ReplaceVehicle(editedVehicle, Data);
            }
            else
            {
                _vehicleManager.UpdateChangedSettings(editedVehicle, liveData, Data);
            }
        }

        private static bool IsModelChanged(VehicleModel before, VehicleModel after)
        {
            return before.ModelId != after.ModelId || before.ModelSpawnTypeId != after.ModelSpawnTypeId;
        }

        private void SendSavedMessage(bool isNewVehicle)
        {
            string location = $"at {Data.X:F1}, {Data.Y:F1}, {Data.Z:F1} in virtual world {Data.VirtualWorld}, interior {Data.Interior}";
            string message = isNewVehicle
                ? $"Vehicle #{Data.Id} is saved and spawned {location}. You're in the driver seat."
                : $"Vehicle #{Data.Id} is saved and updated. It's {location}.";
            _player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, message);
        }

        /// <summary>
        /// The first problem found, named for the admin, or null when the copy can be saved.
        /// </summary>
        private static string? GetValidationError(VehicleModel vehicleData)
        {
            if (vehicleData.ModelId == null && vehicleData.ModelSpawnTypeId == null)
            {
                return $"Choose a model first: set the Model ID to a GTA model id from {FirstGtaModelId} to {LastGtaModelId}.";
            }
            if (!IsValidColor(vehicleData.Color1))
            {
                return $"Color 1 must be from 0 to {MaxColor}.";
            }
            if (!IsValidColor(vehicleData.Color2))
            {
                return $"Color 2 must be from 0 to {MaxColor}.";
            }
            if (!float.IsFinite(vehicleData.MaxHealth) || vehicleData.MaxHealth <= 0)
            {
                return "Max health must be a number above 0.";
            }
            if (!float.IsFinite(vehicleData.CurrentHealth) || vehicleData.CurrentHealth <= 0)
            {
                return "Current health must be a number above 0.";
            }
            if (vehicleData.CurrentHealth > vehicleData.MaxHealth)
            {
                return $"Current health can't be more than the max health ({vehicleData.MaxHealth}).";
            }
            if (vehicleData.VirtualWorld < 0 || vehicleData.Interior < 0)
            {
                return "The virtual world and the interior must be 0 or more.";
            }
            return null;
        }

        private static bool IsValidColor(int color)
        {
            return color >= 0 && color <= MaxColor;
        }
    }
}
