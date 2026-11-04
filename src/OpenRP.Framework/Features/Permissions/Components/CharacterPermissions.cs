using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Permissions.Services;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Permissions.Components
{
    /// <summary>
    /// The permissions of the selected character, loaded when the character is selected. To change them, use
    /// IPermissionService.ReloadCharacterPermissions, which replaces this component.
    /// </summary>
    public class CharacterPermissions : Component
    {
        private readonly List<string> _characterPermissions;

        public CharacterPermissions(List<string> permissions)
        {
            _characterPermissions = permissions;
        }

        public List<string> GetCharacterPermissions()
        {
            return _characterPermissions;
        }

        public bool DoesCharacterHavePermission(string permission)
        {
            List<string> characterPermissions = GetCharacterPermissions();

            return characterPermissions.Contains(permission);
        }

        public bool DoesPlayerHaveCommandPermission(string command)
        {
            string parsedCommand = command.Split(' ')[0].TrimStart('/').ToLower();

            return DoesCharacterHavePermission($"cmd.{parsedCommand}");
        }
    }
}
