using OpenRP.Framework.Database.Models;

namespace OpenRP.Framework.Features.Vehicles.Dialogs
{
    /// <summary>
    /// The "Model ID" row. The admin types a GTA model id; the copy stores the id of its model row.
    /// </summary>
    public partial class CreateOrUpdateVehicleDialog
    {
        private const int FirstGtaModelId = 400;
        private const int LastGtaModelId = 611;

        private void AskForModelId()
        {
            OpenInputDialog("Edit Model ID", GetCurrentGtaModelIdText(), SetModelFromInput, ShowEditor,
                $"Enter a GTA model id from {FirstGtaModelId} to {LastGtaModelId}.");
        }

        private void SetModelFromInput(string input)
        {
            if (!int.TryParse(input, out int gtaModelId) || gtaModelId < FirstGtaModelId || gtaModelId > LastGtaModelId)
            {
                OpenMessageDialog("Invalid Model ID", $"Enter a GTA model id from {FirstGtaModelId} to {LastGtaModelId}.", AskForModelId, ShowEditor);
                return;
            }

            VehicleModelModel? vehicleModel = _vehicleManager.FindVehicleModelByGtaId(gtaModelId);
            if (vehicleModel == null)
            {
                OpenMessageDialog("Unknown Model ID", $"There's no vehicle model with the id {gtaModelId} in the database.", AskForModelId, ShowEditor);
                return;
            }

            // A fixed model replaces a random spawn type, so the row only ever stores one of the two.
            Data.ModelId = vehicleModel.Id;
            Data.ModelSpawnTypeId = null;
            ShowEditor();
        }

        /// <summary>
        /// The GTA model id of the copy's model, which is what the admin types, not the id of the model row.
        /// </summary>
        private string GetCurrentGtaModelIdText()
        {
            if (Data.ModelId == null)
            {
                return string.Empty;
            }

            VehicleModelModel? vehicleModel = _vehicleManager.FindVehicleModelById(Data.ModelId.Value);
            return vehicleModel == null ? string.Empty : vehicleModel.ModelId.ToString();
        }
    }
}
