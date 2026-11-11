using OpenRP.Framework.Database;
using OpenRP.Framework.Features.Jurisdictions.Services;
using OpenRP.Framework.Features.Jurisdictions.Services.Dialogs;
using OpenRP.Framework.Features.Radios.Components;
using OpenRP.Framework.Features.Radios.Services;
using OpenRP.Framework.Features.Vehicles.Components;
using OpenRP.Framework.Features.Vehicles.Services;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Vehicles.Dialogs
{
    /// <summary>
    /// The "Radio" row: whether a radio is installed in the vehicle. A vehicle that isn't saved yet gets its radio when
    /// it's saved, because a radio needs the vehicle's id.
    /// </summary>
    public partial class CreateOrUpdateVehicleDialog
    {
        private static void ToggleRadio(Player player, IDialogService dialogService, IEntityManager entityManager, IVehicleManager vehicleManager, IJurisdictionService jurisdictionService,
            IJurisdictionDialogService jurisdictionDialogService, IRadioService radioService, IRadioEditorService radioEditorService, BaseDataContext context, CreateOrUpdateVehicleDataComponent component)
        {
            ulong vehicleId = component.CreateOrUpdateVehicleData.Id;
            if (vehicleId == 0)
            {
                component.InstallRadioOnSave = !component.InstallRadioOnSave;
                player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, component.InstallRadioOnSave ? "A radio will be installed when you save." : "No radio will be installed.");
            }
            else if (radioService.GetVehicleRadio(vehicleId) == null)
            {
                string? error = radioEditorService.InstallVehicleRadio(vehicleId);
                player.SendPlayerInfoMessage(error == null ? PlayerInfoMessageType.INFO : PlayerInfoMessageType.ERROR, error ?? "A radio is installed in this vehicle, tuned to 1000.");
            }
            else
            {
                radioEditorService.RemoveVehicleRadio(vehicleId);
                player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, "The radio is removed from this vehicle.");
            }

            Open(player, dialogService, entityManager, vehicleManager, jurisdictionService, jurisdictionDialogService, radioService, radioEditorService, context, component);
        }

        /// <summary>
        /// Installs the radio chosen for a new vehicle, now that saving gave it an id.
        /// </summary>
        private static void InstallRadioAfterSave(Player player, IRadioEditorService radioEditorService, CreateOrUpdateVehicleDataComponent component)
        {
            ulong vehicleId = component.CreateOrUpdateVehicleData.Id;
            if (!component.InstallRadioOnSave || vehicleId == 0)
            {
                return;
            }

            component.InstallRadioOnSave = false;
            string? error = radioEditorService.InstallVehicleRadio(vehicleId);
            if (error != null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, error);
            }
        }

        private static string GetRadioLabel(IRadioService radioService, CreateOrUpdateVehicleDataComponent component)
        {
            ulong vehicleId = component.CreateOrUpdateVehicleData.Id;
            if (vehicleId == 0)
            {
                return component.InstallRadioOnSave ? "Installed when saved" : "None";
            }

            Radio? radio = radioService.GetVehicleRadio(vehicleId);
            if (radio == null)
            {
                return "None";
            }
            return radio.IsOn() ? $"Installed (on, {radio.GetFrequency()})" : "Installed (off)";
        }
    }
}
