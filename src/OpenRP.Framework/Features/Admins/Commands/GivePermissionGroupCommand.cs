using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Permissions.Components;
using OpenRP.Framework.Features.Permissions.Services;
using OpenRP.Framework.Shared.Chat.Enums;
using SampSharp.Entities.SAMP;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared.Chat.Extensions;

namespace OpenRP.Framework.Features.Admins.Commands
{
    public class GivePermissionGroupCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new string[] { "Admin" },
            Description = "Give a permission group to a player.",
            CommandGroups = new string[] { "Admin", "Permissions" })]
        public void GivePermissionGroup(Player player, IPermissionService permissionManager, Player playerid, string permission_group)
        {
            // Ensure the target player is currently playing as a character.
            if (playerid.IsPlayerPlayingAsCharacter())
            {
                // Retrieve the character associated with the target player.
                Character targetCharacter = playerid.GetPlayerCurrentlyPlayingAsCharacter();
                CharacterPermissions targetCharacterPermissions = targetCharacter.GetComponent<CharacterPermissions>();

                // Attempt to give the permission group and capture the success status.
                bool success = permissionManager.GiveCharacterPermissionGroupByName(targetCharacter.GetDatabaseId(), permission_group);

                // Provide feedback based on whether the operation succeeded.
                if (success)
                {
                    targetCharacterPermissions.UpdateCharacterPermissions();

                    player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO,
                        $"Successfully granted the permission group '{permission_group}' to player {targetCharacter.GetCharacterName()}.");
                }
                else
                {
                    player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR,
                        $"Failed to grant the permission group '{permission_group}' to player {targetCharacter.GetCharacterName()}. Please check the permission group name.");
                }
            }
            else
            {
                // Notify the sender if the target player is not currently playing as a character.
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR,
                    $"Player {playerid.Name} is not currently playing as a character.");
            }
        }

    }
}
