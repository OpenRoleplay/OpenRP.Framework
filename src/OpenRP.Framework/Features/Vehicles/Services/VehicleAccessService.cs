using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Permissions.Components;
using OpenRP.Framework.Features.Permissions.Services;
using OpenRP.Framework.Features.Vehicles.Components;
using OpenRP.Framework.Features.Vehicles.Entities;

namespace OpenRP.Framework.Features.Vehicles.Services
{
    public class VehicleAccessService : IVehicleAccessService
    {
        private const string AdminGroupName = "Admin";

        private readonly IVehicleKeyService _vehicleKeyService;

        public VehicleAccessService(IVehicleKeyService vehicleKeyService)
        {
            _vehicleKeyService = vehicleKeyService;
        }

        /// <summary>
        /// Like the faction permissions, the permission is created here and linked to the group, so the Admin group has it from the start.
        /// </summary>
        public void CreatePermissions(IPermissionService permissionService)
        {
            PermissionModel permission = permissionService.CreatePermissionIfNotExists(VehiclePermissions.Admin, VehiclePermissions.AdminDescription);
            permissionService.AddPermissionToGroupIfMissing(permission.Id, AdminGroupName);
        }

        public bool IsAdmin(Character character)
        {
            CharacterPermissions? characterPermissions = character.GetComponent<CharacterPermissions>();
            return characterPermissions != null && characterPermissions.DoesCharacterHavePermission(VehiclePermissions.Admin);
        }

        public bool CanControl(Character character, VehicleDataComponent? vehicleDataComponent, bool isAdmin)
        {
            if (isAdmin)
            {
                return true;
            }
            if (vehicleDataComponent == null)
            {
                return false;
            }
            return IsOwnerOrKeyHolder(character, vehicleDataComponent.VehicleData);
        }

        public bool CanPark(Character character, VehicleDataComponent vehicleDataComponent, bool isAdmin)
        {
            VehicleModel vehicleData = vehicleDataComponent.VehicleData;
            if (vehicleData.ServerVehicle)
            {
                return isAdmin;
            }
            return isAdmin || IsOwnerOrKeyHolder(character, vehicleData);
        }

        private bool IsOwnerOrKeyHolder(Character character, VehicleModel vehicleData)
        {
            return IsOwner(character, vehicleData) || _vehicleKeyService.HasVehicleKey(character, vehicleData.Id);
        }

        private static bool IsOwner(Character character, VehicleModel vehicleData)
        {
            return vehicleData.OwnerCharacterId == character.GetId();
        }
    }
}
