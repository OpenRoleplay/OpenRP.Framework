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
        void RemoveOldCommandPermissions();
        List<string> GetCharacterPermissions(ulong characterId);
        List<PermissionModel> GetCharacterPermissionsModels(ulong characterId);
        CharacterPermissions ReloadCharacterPermissions(Character character);
        bool GiveCharacterPermissionGroupByName(ulong characterId, string permissionGroup);
    }
}
