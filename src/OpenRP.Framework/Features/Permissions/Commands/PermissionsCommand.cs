using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Characters.Components;
using OpenRP.Framework.Features.Commands.Attributes;
using OpenRP.Framework.Features.Permissions.Services;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs;
using SampSharp.Entities.SAMP;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared.Chat.Extensions;

namespace OpenRP.Framework.Features.Permissions.Commands
{
    public class PermissionsCommand : ISystem
    {
        [ServerCommand(PermissionGroups = new[] { "Default" },
            Description = "Display your own permissions. Use this command to view a list of all permissions currently assigned to your character.")]
        public void Permissions(Player player, IPermissionService permissionManager, IDialogService dialogService)
        {
            DisplayPermissions(player, player.GetPlayerCurrentlyPlayingAsCharacter(), permissionManager, dialogService);
        }

        [ServerCommand(PermissionGroups = new[] { "Admin" },
            Description = "Display another player's permissions. Use this admin command to view the permissions assigned to a specified player.")]
        public void ShowPermissions(Player admin, IPermissionService permissionManager, IDialogService dialogService, Player target)
        {
            if (target == null)
            {
                admin.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "That player could not be found.");
                return;
            }

            DisplayPermissions(admin, target.GetPlayerCurrentlyPlayingAsCharacter(), permissionManager, dialogService, target);
        }

        private void DisplayPermissions(Player viewer, Character character, IPermissionService permissionManager, IDialogService dialogService, Player targetPlayer = null)
        {
            if (character == null)
            {
                viewer.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "That character could not be found.");
                return;
            }

            ulong characterId = character.GetId();
            List<PermissionModel> permissionModels = permissionManager.GetCharacterPermissionsModels(characterId);

            // Create and configure the dialog
            BetterTablistDialog dialog = new BetterTablistDialog("Back", null, 2);
            string title = targetPlayer != null
                ? $"{targetPlayer.Name}'s Permissions"
                : "Your Permissions";

            dialog.SetTitle(TitleType.Parents, title);
            dialog.AddHeaders("Permission", "Description");

            foreach (PermissionModel permissionModel in permissionModels)
            {
                string description = string.IsNullOrEmpty(permissionModel.Description) ? "N/A" : permissionModel.Description;
                dialog.AddRow(permissionModel.Name, description);
            }

            // Show the dialog
            dialogService.Show(viewer, dialog, response => HandleDialogResponse(response, viewer));
        }

        private void HandleDialogResponse(TablistDialogResponse response, Player player)
        {
            // Placeholder for additional functionality if needed later.
        }

    }
}
