using System.Collections.Generic;

namespace OpenRP.Framework.Features.Permissions.Entities
{
    /// <summary>
    /// Gives characters permissions on top of their permission groups, for example through faction ranks. Every
    /// registered source is added when a character's permissions are (re)loaded.
    /// </summary>
    public interface ICharacterPermissionSource
    {
        /// <summary>
        /// The names of the permissions this source gives the character.
        /// </summary>
        IEnumerable<string> GetPermissions(ulong characterId);
    }
}
