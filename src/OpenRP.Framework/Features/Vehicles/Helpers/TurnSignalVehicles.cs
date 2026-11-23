using SampSharp.Entities.SAMP;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Vehicles.Helpers
{
    /// <summary>
    /// Which GTA models have turn signals. Only cars and vans have light panels: planes, helicopters, boats and bikes don't.
    /// </summary>
    public static class TurnSignalVehicles
    {
        private static readonly HashSet<VehicleModelType> AircraftModels = new HashSet<VehicleModelType>
        {
            VehicleModelType.Andromada, VehicleModelType.AT400, VehicleModelType.Beagle, VehicleModelType.Cargobob,
            VehicleModelType.Cropduster, VehicleModelType.Dodo, VehicleModelType.Hunter, VehicleModelType.Hydra,
            VehicleModelType.Leviathan, VehicleModelType.Maverick, VehicleModelType.Nevada, VehicleModelType.PoliceMaverick,
            VehicleModelType.Raindance, VehicleModelType.Rustler, VehicleModelType.SANNewsMaverick, VehicleModelType.Seasparrow,
            VehicleModelType.Shamal, VehicleModelType.Skimmer, VehicleModelType.Sparrow, VehicleModelType.Stuntplane
        };

        private static readonly HashSet<VehicleModelType> BoatModels = new HashSet<VehicleModelType>
        {
            VehicleModelType.Coastguard, VehicleModelType.Dinghy, VehicleModelType.Jetmax, VehicleModelType.Launch,
            VehicleModelType.Marquis, VehicleModelType.Predator, VehicleModelType.Reefer, VehicleModelType.Speeder,
            VehicleModelType.Squallo, VehicleModelType.Tropic,
            // A hovercraft, which floats like a boat.
            VehicleModelType.Vortex
        };

        private static readonly HashSet<VehicleModelType> BikeModels = new HashSet<VehicleModelType>
        {
            VehicleModelType.BF400, VehicleModelType.Bike, VehicleModelType.BMX, VehicleModelType.Faggio,
            VehicleModelType.FCR900, VehicleModelType.Freeway, VehicleModelType.HPV1000, VehicleModelType.MountainBike,
            VehicleModelType.NRG500, VehicleModelType.PCJ600, VehicleModelType.Pizzaboy, VehicleModelType.Quad,
            VehicleModelType.Sanchez, VehicleModelType.Wayfarer
        };

        /// <summary>
        /// True for models that have light panels to put turn signals on. Says nothing about the model's size data.
        /// </summary>
        public static bool CanHaveTurnSignals(VehicleModelType model)
        {
            return !AircraftModels.Contains(model) && !BoatModels.Contains(model) && !BikeModels.Contains(model);
        }
    }
}
