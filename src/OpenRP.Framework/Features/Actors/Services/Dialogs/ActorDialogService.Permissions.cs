using OpenRP.Framework.Features.Actors.Components;
using OpenRP.Framework.Features.Actors.Entities;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Extensions;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Actors.Services.Dialogs
{
    public partial class ActorDialogService
    {
        /// <summary>
        /// Every permission features offer, each switched on or off for this actor by picking it.
        /// </summary>
        private void OpenPermissions(Player player, ServerActor actor, Action onBack)
        {
            BetterListDialog permissionsDialog = new BetterListDialog("Switch", "Back");
            permissionsDialog.SetTitle(TitleType.Children, ActorsTitle, actor.GetName(), "Permissions");
            Dictionary<int, ActorPermissionDefinition> definitionsByRow = new Dictionary<int, ActorPermissionDefinition>();
            foreach (ActorPermissionDefinition definition in _actorPermissionService.GetDefinitions())
            {
                definitionsByRow[permissionsDialog.AddRow(GetPermissionLabel(actor, definition))] = definition;
            }
            if (definitionsByRow.Count == 0)
            {
                ShowMessage(player, "Permissions", "No feature offers permissions for actors.", onBack);
                return;
            }

            _dialogService.ShowIgnoringDisconnect(player, permissionsDialog, response =>
            {
                if (response.Response != DialogResponse.LeftButton)
                {
                    onBack();
                }
                else if (definitionsByRow.TryGetValue(response.ItemIndex, out ActorPermissionDefinition? definition))
                {
                    _actorPermissionService.SetPermission(actor, definition.Name, !_actorPermissionService.HasPermission(actor, definition.Name));
                    OpenPermissions(player, actor, onBack);
                }
            });
        }

        private string GetPermissionLabel(ServerActor actor, ActorPermissionDefinition definition)
        {
            string state = _actorPermissionService.HasPermission(actor, definition.Name) ? $"{ChatColor.Highlight}[On]" : $"{ChatColor.LightGray}[Off]";
            return $"{state}{ChatColor.White} {definition.Name} {ChatColor.LightGray}{definition.Description}";
        }

        private int GetPermissionCount(ServerActor actor)
        {
            int count = 0;
            foreach (ActorPermissionDefinition definition in _actorPermissionService.GetDefinitions())
            {
                if (_actorPermissionService.HasPermission(actor, definition.Name))
                {
                    count++;
                }
            }
            return count;
        }
    }
}
