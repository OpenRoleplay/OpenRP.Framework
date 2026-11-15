using OpenRP.Framework.Features.Vehicles.Components;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace OpenRP.Framework.Features.Vehicles.Helpers
{
    /// <summary>
    /// Finds the vehicles a player means when they don't name one: the ones in reach, in their world and interior.
    /// Reach is measured from the vehicle's footprint, so a long vehicle can be reached at its rear.
    /// </summary>
    public static class NearbyVehicleFinder
    {
        // How close a vehicle must be when the player isn't sitting in one.
        public const float NearbyDistance = 5.0f;

        /// <summary>
        /// The loaded vehicles within the distance, saved or spawned from /spawnveh, the nearest first.
        /// </summary>
        public static List<Vehicle> FindNearbyVehicles(IEntityManager entityManager, IVehicleInfoService vehicleInfoService, Player player)
        {
            return entityManager.GetComponents<Vehicle>()
                .Where(vehicle => IsNearbyInSameWorld(vehicle, player, vehicleInfoService))
                .OrderBy(vehicle => GetDistanceToVehicle(vehicle, player, vehicleInfoService))
                .ToList();
        }

        /// <summary>
        /// The closest loaded vehicle within the distance, saved or spawned from /spawnveh.
        /// </summary>
        public static Vehicle? FindNearestVehicle(IEntityManager entityManager, IVehicleInfoService vehicleInfoService, Player player)
        {
            return FindNearbyVehicles(entityManager, vehicleInfoService, player).FirstOrDefault();
        }

        /// <summary>
        /// The closest saved vehicle within the distance, one that has a database row.
        /// </summary>
        public static Vehicle? FindNearestSavedVehicle(IEntityManager entityManager, IVehicleInfoService vehicleInfoService, Player player)
        {
            return FindNearbyVehicles(entityManager, vehicleInfoService, player)
                .FirstOrDefault(vehicle => vehicle.GetComponent<VehicleDataComponent>() != null);
        }

        private static bool IsNearbyInSameWorld(Vehicle vehicle, Player player, IVehicleInfoService vehicleInfoService)
        {
            return vehicle.VirtualWorld == player.VirtualWorld
                && vehicle.Interior == player.Interior
                && GetDistanceToVehicle(vehicle, player, vehicleInfoService) <= NearbyDistance;
        }

        /// <summary>
        /// The distance from the player to the nearest point of the vehicle's footprint, the rectangle of its model's size
        /// turned to its heading. Without size data, the distance to its centre.
        /// </summary>
        private static float GetDistanceToVehicle(Vehicle vehicle, Player player, IVehicleInfoService vehicleInfoService)
        {
            Vector3 offset = player.Position - vehicle.Position;
            Vector3 size = vehicleInfoService.GetModelInfo(vehicle.Model, VehicleModelInfoType.Size);
            if (size == Vector3.Zero)
            {
                return offset.Length();
            }

            // A heading of 0 faces north (+Y), and it turns counter-clockwise, so the nose is (-sin, cos) and the right side (cos, sin).
            float headingInRadians = vehicle.Angle * MathF.PI / 180f;
            float cos = MathF.Cos(headingInRadians);
            float sin = MathF.Sin(headingInRadians);
            float alongVehicleSide = offset.X * cos + offset.Y * sin;
            float alongVehicleNose = -offset.X * sin + offset.Y * cos;

            float distanceOutsideSide = MathF.Max(MathF.Abs(alongVehicleSide) - size.X / 2f, 0f);
            float distanceOutsideNoseOrTail = MathF.Max(MathF.Abs(alongVehicleNose) - size.Y / 2f, 0f);
            return MathF.Sqrt(distanceOutsideSide * distanceOutsideSide + distanceOutsideNoseOrTail * distanceOutsideNoseOrTail + offset.Z * offset.Z);
        }
    }
}
