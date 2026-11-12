using OpenRP.Framework.Features.Actors.Components;
using OpenRP.Framework.Features.Actors.Helpers;
using OpenRP.Framework.Features.Properties.Components;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using SampSharp.Entities.SAMP;
using OpenRP.Framework.Shared.Dialogs.Extensions;

namespace OpenRP.Framework.Features.Actors.Services.Dialogs
{
    public partial class ActorDialogService
    {
        private void OpenActorActions(Player player, ServerActor actor, Action onBack)
        {
            if (actor.IsDeleted())
            {
                ShowMessage(player, actor.GetName(), "This actor has been deleted.", onBack);
                return;
            }

            BetterListDialog actionsDialog = new BetterListDialog("Choose", "Back");
            actionsDialog.SetTitle(TitleType.Children, ActorsTitle, actor.GetName());
            int goToRow = actionsDialog.AddRow("Go to");
            int moveRow = actionsDialog.AddRow("Move here");
            int renameRow = actionsDialog.AddRow("Rename");
            int skinRow = actionsDialog.AddRow($"Skin {ChatColor.LightGray}({actor.GetSkin()})");
            int animationRow = actionsDialog.AddRow($"Animation {ChatColor.LightGray}({actor.GetAnimationText() ?? "none"})");
            int worldRow = actionsDialog.AddRow($"Virtual world {ChatColor.LightGray}({GetWhereText(actor)})");
            int promptRow = actionsDialog.AddRow($"AI prompt {ChatColor.LightGray}({(actor.HasActorPrompt() ? "view" : "none")})");
            int permissionsRow = actionsDialog.AddRow($"Permissions {ChatColor.LightGray}({GetPermissionCount(actor)} on)");
            int deleteRow = actionsDialog.AddRow("Delete");

            void OnActionChosen(ListDialogResponse response)
            {
                Action reopenActions = () => OpenActorActions(player, actor, onBack);
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (response.ItemIndex == goToRow)
                    _actorEditorService.MovePlayerToActor(player, actor);
                else if (response.ItemIndex == moveRow)
                    MoveActorToPlayer(player, actor, reopenActions);
                else if (response.ItemIndex == renameRow)
                    OpenRenameActor(player, actor, reopenActions);
                else if (response.ItemIndex == skinRow)
                    OpenSkinInput(player, actor, reopenActions);
                else if (response.ItemIndex == animationRow)
                    OpenAnimationChoice(player, actor, reopenActions);
                else if (response.ItemIndex == worldRow)
                    OpenVirtualWorldChoice(player, actor, reopenActions);
                else if (response.ItemIndex == promptRow)
                    ShowPrompt(player, actor, 0, reopenActions);
                else if (response.ItemIndex == permissionsRow)
                    OpenPermissions(player, actor, reopenActions);
                else if (response.ItemIndex == deleteRow)
                    OpenDeleteActor(player, actor, onBack, reopenActions);
            }

            _dialogService.ShowIgnoringDisconnect(player, actionsDialog, OnActionChosen);
        }

        private void MoveActorToPlayer(Player player, ServerActor actor, Action onMoved)
        {
            _actorEditorService.MoveActorToPlayer(actor, player);
            onMoved();
        }

        private void OpenRenameActor(Player player, ServerActor actor, Action onBack)
        {
            void Rename(string name)
            {
                string? error = _actorEditorService.RenameActor(actor, name);
                ContinueAfter(player, error, onBack, () => OpenRenameActor(player, actor, onBack));
            }

            OpenTextInput(player, $"Rename {actor.GetName()}", $"Enter the new name for {ChatColor.Highlight}{actor.GetName()}{ChatColor.White}:", Rename, onBack);
        }

        private void OpenSkinInput(Player player, ServerActor actor, Action onBack)
        {
            void SetSkin(string skinText)
            {
                string? error = _actorEditorService.SetActorSkin(actor, skinText);
                ContinueAfter(player, error, onBack, () => OpenSkinInput(player, actor, onBack));
            }

            OpenTextInput(player, "Skin", $"Enter the skin id for {ChatColor.Highlight}{actor.GetName()}{ChatColor.White} (now {actor.GetSkin()}):", SetSkin, onBack);
        }

        /// <summary>
        /// A preset, a custom library and animation, or none.
        /// </summary>
        private void OpenAnimationChoice(Player player, ServerActor actor, Action onBack)
        {
            BetterListDialog animationsDialog = new BetterListDialog("Choose", "Back");
            animationsDialog.SetTitle(TitleType.Children, ActorsTitle, actor.GetName(), "Animation");
            int noneRow = animationsDialog.AddRow("None (just stand)");
            int customRow = animationsDialog.AddRow($"{ChatColor.CornflowerBlue}Custom...");
            Dictionary<int, ActorAnimationPresets.Preset> presetsByRow = new Dictionary<int, ActorAnimationPresets.Preset>();
            foreach (ActorAnimationPresets.Preset preset in ActorAnimationPresets.All)
            {
                presetsByRow[animationsDialog.AddRow($"{preset.Description} {ChatColor.LightGray}({preset.Library}/{preset.Name})")] = preset;
            }

            _dialogService.ShowIgnoringDisconnect(player, animationsDialog, response =>
            {
                if (response.Response != DialogResponse.LeftButton)
                    onBack();
                else if (response.ItemIndex == noneRow)
                    SetAnimation(actor, null, null, onBack);
                else if (response.ItemIndex == customRow)
                    OpenCustomAnimationLibrary(player, actor, onBack);
                else if (presetsByRow.TryGetValue(response.ItemIndex, out ActorAnimationPresets.Preset? preset))
                    SetAnimation(actor, preset.Library, preset.Name, onBack);
            });
        }

        private void OpenCustomAnimationLibrary(Player player, ServerActor actor, Action onBack)
        {
            OpenTextInput(player, "Custom animation", "Enter the animation library, for example SMOKING:",
                library => OpenCustomAnimationName(player, actor, library, onBack), onBack);
        }

        private void OpenCustomAnimationName(Player player, ServerActor actor, string library, Action onBack)
        {
            OpenTextInput(player, "Custom animation", $"Enter the animation name in {ChatColor.Highlight}{library}{ChatColor.White}, for example M_smklean_loop:",
                name => SetAnimation(actor, library, name, onBack), onBack);
        }

        private void SetAnimation(ServerActor actor, string? library, string? name, Action onSet)
        {
            _actorEditorService.SetActorAnimation(actor, library, name);
            onSet();
        }

        /// <summary>
        /// Outside, or inside any property, in its virtual world.
        /// </summary>
        private void OpenVirtualWorldChoice(Player player, ServerActor actor, Action onBack)
        {
            BetterListDialog worldsDialog = new BetterListDialog("Choose", "Back");
            worldsDialog.SetTitle(TitleType.Children, ActorsTitle, actor.GetName(), "Virtual world");
            Dictionary<int, ulong?> propertyIdsByRow = new Dictionary<int, ulong?>
            {
                [worldsDialog.AddRow("Outside (shared world)")] = null
            };
            foreach (Property property in _propertyEditorService.GetProperties())
            {
                propertyIdsByRow[worldsDialog.AddRow($"Inside {property.GetName()}")] = property.GetId();
            }

            _dialogService.ShowIgnoringDisconnect(player, worldsDialog, response =>
            {
                if (response.Response == DialogResponse.LeftButton && propertyIdsByRow.TryGetValue(response.ItemIndex, out ulong? propertyId))
                {
                    _actorEditorService.SetActorInsideProperty(actor, propertyId);
                    // The actor is in another virtual world now; put the admin there so they still see it.
                    _actorEditorService.MovePlayerToActor(player, actor);
                }
                onBack();
            });
        }

        /// <summary>
        /// The prompt is read-only in-game: input boxes are too short for it.
        /// </summary>
        private void ShowPrompt(Player player, ServerActor actor, int pageIndex, Action onBack)
        {
            if (!actor.HasActorPrompt())
            {
                ShowMessage(player, "AI prompt", "This actor has no AI prompt, so it doesn't talk. Prompts are added and changed in the database (Actors.Prompt).", onBack);
                return;
            }

            List<string> pages = TextPages.Split(actor.GetActorPrompt());
            bool hasNextPage = pageIndex < pages.Count - 1;
            BetterMessageDialog promptDialog = new BetterMessageDialog(hasNextPage ? "Next" : "Back", hasNextPage ? "Back" : "");
            promptDialog.SetTitle(TitleType.Children, ActorsTitle, actor.GetName(), $"AI prompt ({pageIndex + 1}/{pages.Count})");
            promptDialog.SetContent($"{pages[pageIndex]}\n\n{ChatColor.LightGray}Prompts are changed in the database (Actors.Prompt).");

            _dialogService.ShowIgnoringDisconnect(player, promptDialog, response =>
            {
                if (hasNextPage && response.Response == DialogResponse.LeftButton)
                {
                    ShowPrompt(player, actor, pageIndex + 1, onBack);
                    return;
                }
                onBack();
            });
        }

        private void OpenDeleteActor(Player player, ServerActor actor, Action onDeleted, Action onBack)
        {
            void Delete()
            {
                _actorEditorService.DeleteActor(actor);
                onDeleted();
            }

            ShowConfirmation(player, "Delete actor", $"Are you sure you want to delete {ChatColor.Highlight}{actor.GetName()}{ChatColor.White}?", Delete, onBack);
        }
    }
}
