using OpenRP.Framework.Database;
using OpenRP.Framework.Features.Jurisdictions.Components;
using OpenRP.Framework.Features.Jurisdictions.Services;
using OpenRP.Framework.Features.Jurisdictions.Services.Dialogs;
using OpenRP.Framework.Features.Radios.Services;
using OpenRP.Framework.Features.Vehicles.Components;
using OpenRP.Framework.Features.Vehicles.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Vehicles.Dialogs
{
    /// <summary>
    /// The "Registered in" row: the jurisdiction the vehicle is registered in, whose country's tag starts its plate.
    /// </summary>
    public partial class CreateOrUpdateVehicleDialog
    {
        private static void OpenRegisteredIn(Player player, IDialogService dialogService, IEntityManager entityManager, IVehicleManager vehicleManager,
            IJurisdictionService jurisdictionService, IJurisdictionDialogService jurisdictionDialogService, IRadioService radioService, IRadioEditorService radioEditorService, BaseDataContext context, CreateOrUpdateVehicleDataComponent component)
        {
            void ReopenEditor()
            {
                Open(player, dialogService, entityManager, vehicleManager, jurisdictionService, jurisdictionDialogService, radioService, radioEditorService, context, component);
            }

            void SetRegisteredIn(Jurisdiction? jurisdiction)
            {
                component.CreateOrUpdateVehicleData.JurisdictionId = jurisdiction?.GetId();
                if (component.EditedVehicle != null)
                {
                    vehicleManager.UpdateVehicle(component.EditedVehicle, component.CreateOrUpdateVehicleData);
                }
                ReopenEditor();
            }

            jurisdictionDialogService.OpenJurisdictionPicker(player, "Registered in", "Nowhere (SA plate)", SetRegisteredIn, ReopenEditor);
        }

        private static string GetRegisteredInLabel(IJurisdictionService jurisdictionService, CreateOrUpdateVehicleDataComponent component)
        {
            ulong? jurisdictionId = component.CreateOrUpdateVehicleData.JurisdictionId;
            Jurisdiction? jurisdiction = jurisdictionId.HasValue ? jurisdictionService.GetJurisdiction(jurisdictionId.Value) : null;
            string plateTag = jurisdictionService.GetPlateTag(jurisdictionId);
            return jurisdiction == null ? $"Nowhere ({plateTag} plate)" : $"{jurisdiction.GetName()} ({plateTag} plate)";
        }
    }
}
