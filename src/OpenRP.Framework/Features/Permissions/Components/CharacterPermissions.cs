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
    public class CharacterPermissions : Component
    {
        private Character _character;
        private List<string> _characterPermissions;

        public CharacterPermissions(List<string> permissions)
        {
            _characterPermissions = permissions;
        }

        private Character GetCharacter()
        {
            if (_character != null)
            {
                return _character;
            }

            return _character = GetComponent<Character>();
        }

        public void UpdateCharacterPermissions()
        {
            Character currentCharacter = GetCharacter();

            _characterPermissions = null;
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
