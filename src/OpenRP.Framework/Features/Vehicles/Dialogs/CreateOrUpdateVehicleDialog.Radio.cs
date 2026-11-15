using OpenRP.Framework.Features.Radios.Components;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Vehicles.Dialogs
{
    /// <summary>
    /// The "Radio" row: whether a radio is installed in the vehicle. A vehicle that isn't saved yet gets its radio when
    /// it's saved, because a radio needs the vehicle's id.
    /// </summary>
    public partial class CreateOrUpdateVehicleDialog
    {
        private void ToggleRadio()
        {
            ulong vehicleId = Data.Id;
            if (vehicleId == 0)
            {
                _component.InstallRadioOnSave = !_component.InstallRadioOnSave;
                _player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, _component.InstallRadioOnSave ? "A radio will be installed when you save." : "No radio will be installed.");
            }
            else if (_radioService.GetVehicleRadio(vehicleId) == null)
            {
                string? error = _radioEditorService.InstallVehicleRadio(vehicleId);
                _player.SendPlayerInfoMessage(error == null ? PlayerInfoMessageType.INFO : PlayerInfoMessageType.ERROR, error ?? "A radio is installed in this vehicle, tuned to 1000.");
            }
            else
            {
                _radioEditorService.RemoveVehicleRadio(vehicleId);
                _player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, "The radio is removed from this vehicle.");
            }

            ShowEditor();
        }

        /// <summary>
        /// Installs the radio chosen for a new vehicle, now that saving gave it an id.
        /// </summary>
        private void InstallRadioAfterSave()
        {
            ulong vehicleId = Data.Id;
            if (!_component.InstallRadioOnSave || vehicleId == 0)
            {
                return;
            }

            _component.InstallRadioOnSave = false;
            string? error = _radioEditorService.InstallVehicleRadio(vehicleId);
            if (error != null)
            {
                _player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, error);
            }
        }

        private string GetRadioLabel()
        {
            ulong vehicleId = Data.Id;
            if (vehicleId == 0)
            {
                return _component.InstallRadioOnSave ? "Installed when saved" : "None";
            }

            Radio? radio = _radioService.GetVehicleRadio(vehicleId);
            if (radio == null)
            {
                return "None";
            }
            return radio.IsOn() ? $"Installed (on, {radio.GetFrequency()})" : "Installed (off)";
        }
    }
}
