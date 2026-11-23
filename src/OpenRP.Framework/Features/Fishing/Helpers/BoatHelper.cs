using SampSharp.Entities.SAMP;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Fishing.Helpers
{
    public static class BoatHelper
    {
        private static readonly HashSet<VehicleModelType> BoatModels = new HashSet<VehicleModelType>
        {
            VehicleModelType.Coastguard, VehicleModelType.Dinghy, VehicleModelType.Jetmax, VehicleModelType.Launch,
            VehicleModelType.Marquis, VehicleModelType.Predator, VehicleModelType.Reefer, VehicleModelType.Speeder,
            VehicleModelType.Squallo, VehicleModelType.Tropic
        };

        public static bool IsBoat(Vehicle vehicle)
        {
            return BoatModels.Contains(vehicle.Model);
        }

        /// <summary>
        /// The vehicle the player sits in, or else the one they stand on.
        /// </summary>
        public static Vehicle? GetVehicleInOrOn(Player player)
        {
            return player.InAnyVehicle ? player.Vehicle : player.SurfingEntity as Vehicle;
        }

        /// <summary>
        /// The boat the player sits in or stands on, or null when they aren't on a boat.
        /// </summary>
        public static Vehicle? GetBoatInOrOn(Player player)
        {
            Vehicle? vehicle = GetVehicleInOrOn(player);
            return vehicle != null && IsBoat(vehicle) ? vehicle : null;
        }
    }
}
