using OpenRP.Framework.Database.Models;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Vehicles.Components
{
    public class CreateOrUpdateVehicleDataComponent : Component
    {
        // The editor's copy. Nothing reaches the database or the live vehicle until it's saved.
        public VehicleModel CreateOrUpdateVehicleData { get; set; }
        // The live vehicle being edited; null for a vehicle that doesn't exist yet.
        public Vehicle? EditedVehicle { get; set; }
        // The live data when the editor opened: saving applies only what differs from it.
        public VehicleModel? OriginalVehicleData { get; set; }
        // For a vehicle that isn't saved yet: install a radio once saving gives it an id.
        public bool InstallRadioOnSave { get; set; }
        // For a vehicle that isn't saved yet: give the admin a key once saving gives it an id.
        public bool GiveKeyOnSave { get; set; }

        public CreateOrUpdateVehicleDataComponent()
        {
            CreateOrUpdateVehicleData = new VehicleModel();
            EditedVehicle = null;
            OriginalVehicleData = null;
        }
    }
}
