using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Permissions.Services;
using OpenRP.Framework.Features.Vehicles.Components;

namespace OpenRP.Framework.Features.Vehicles.Services
{
    public interface IVehicleAccessService
    {
        /// <summary>
        /// Creates the vehicles.admin permission and gives it to the Admin group. Called once at startup, before any character is selected.
        /// </summary>
        void CreatePermissions(IPermissionService permissionService);

        /// <summary>
        /// True when the character has the vehicles.admin permission, and may control and park any vehicle.
        /// </summary>
        bool IsAdmin(Character character);

        /// <summary>
        /// True when the character may lock or unlock the vehicle: an admin, its owner or a key holder. A spawned vehicle
        /// has no data, so here only an admin passes; the caller lets whoever sits in it through.
        /// </summary>
        bool CanControl(Character character, VehicleDataComponent? vehicleDataComponent, bool isAdmin);

        /// <summary>
        /// True when the character may park the saved vehicle: an admin, or its owner or a key holder. A server vehicle is for admins only.
        /// </summary>
        bool CanPark(Character character, VehicleDataComponent vehicleDataComponent, bool isAdmin);
    }
}
