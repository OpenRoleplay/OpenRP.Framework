using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Vehicles.Dialogs
{
    /// <summary>
    /// The "Give key" row: gives the admin a key to this vehicle. A vehicle that isn't saved yet gets its key when it's
    /// saved, because a key needs the vehicle's id.
    /// </summary>
    public partial class CreateOrUpdateVehicleDialog
    {
        private void GiveKey()
        {
            ulong vehicleId = Data.Id;
            if (vehicleId == 0)
            {
                _component.GiveKeyOnSave = !_component.GiveKeyOnSave;
                _player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, _component.GiveKeyOnSave ? "A key will be given to you when you save." : "No key will be given to you.");
            }
            else
            {
                string? error = _vehicleKeyService.GiveVehicleKey(_player, vehicleId);
                _player.SendPlayerInfoMessage(error == null ? PlayerInfoMessageType.INFO : PlayerInfoMessageType.ERROR, error ?? $"You received a key for vehicle #{vehicleId}.");
            }

            ShowEditor();
        }

        /// <summary>
        /// Gives the key chosen for a new vehicle, now that saving gave it an id.
        /// </summary>
        private void GiveKeyAfterSave()
        {
            ulong vehicleId = Data.Id;
            if (!_component.GiveKeyOnSave || vehicleId == 0)
            {
                return;
            }

            _component.GiveKeyOnSave = false;
            string? error = _vehicleKeyService.GiveVehicleKey(_player, vehicleId);
            if (error != null)
            {
                _player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, error);
            }
        }

        private string GetGiveKeyLabel()
        {
            if (Data.Id == 0)
            {
                return $"Give key on save: {(_component.GiveKeyOnSave ? "Yes" : "No")}";
            }
            return HasKeyForVehicle(Data.Id) ? "You have a key" : "Give yourself a key";
        }

        private bool HasKeyForVehicle(ulong vehicleId)
        {
            return _player.IsPlayerPlayingAsCharacter() && _vehicleKeyService.HasVehicleKey(_player.GetPlayerCurrentlyPlayingAsCharacter(), vehicleId);
        }
    }
}
