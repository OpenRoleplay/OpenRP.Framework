using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Permissions.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Permissions.Services
{
    public interface IPermissionService
    {
        void CreateDatabaseCommandPermissionsIfNotExists();

        /// <summary>
        /// The permission with this name, created first when it doesn't exist yet. For permissions that aren't command
        /// permissions, such as faction permissions.
        /// </summary>
        PermissionModel CreatePermissionIfNotExists(string name, string description);

        List<PermissionModel> GetAllPermissions();

        /// <summary>
        /// The ids of the permissions in this permission group.
        /// </summary>
        List<ulong> GetPermissionGroupPermissionIds(string groupName);

        /// <summary>
        /// Puts the permission in the group, as a link the gamemode manages, when it isn't in it yet.
        /// </summary>
        void AddPermissionToGroupIfMissing(ulong permissionId, string groupName);

        void RemoveOldCommandPermissions();
        List<string> GetCharacterPermissions(ulong characterId);
        List<PermissionModel> GetCharacterPermissionsModels(ulong characterId);
        CharacterPermissions ReloadCharacterPermissions(Character character);
        bool GiveCharacterPermissionGroupByName(ulong characterId, string permissionGroup);
    }
}
