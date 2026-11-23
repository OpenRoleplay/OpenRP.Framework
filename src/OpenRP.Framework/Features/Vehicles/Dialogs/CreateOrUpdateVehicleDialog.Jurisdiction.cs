using OpenRP.Framework.Features.Jurisdictions.Components;

namespace OpenRP.Framework.Features.Vehicles.Dialogs
{
    /// <summary>
    /// The "Registered in" row: the jurisdiction the vehicle is registered in, whose country's tag starts its plate.
    /// </summary>
    public partial class CreateOrUpdateVehicleDialog
    {
        private void OpenRegisteredIn()
        {
            _jurisdictionDialogService.OpenJurisdictionPicker(_player, "Registered in", "Nowhere (SA plate)", SetRegisteredIn, ShowEditor);
        }

        /// <summary>
        /// Only the copy changes. The plate follows when the editor is saved.
        /// </summary>
        private void SetRegisteredIn(Jurisdiction? jurisdiction)
        {
            Data.JurisdictionId = jurisdiction?.GetId();
            ShowEditor();
        }

        private string GetRegisteredInLabel()
        {
            ulong? jurisdictionId = Data.JurisdictionId;
            Jurisdiction? jurisdiction = jurisdictionId.HasValue ? _jurisdictionService.GetJurisdiction(jurisdictionId.Value) : null;
            string plateTag = _jurisdictionService.GetPlateTag(jurisdictionId);
            return jurisdiction == null ? $"Nowhere ({plateTag} plate)" : $"{jurisdiction.GetName()} ({plateTag} plate)";
        }
    }
}
