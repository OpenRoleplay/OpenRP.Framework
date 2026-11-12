using OpenRP.Framework.Features.Actors.Components;
using OpenRP.Framework.Features.Properties.Components;
using OpenRP.Framework.Features.Properties.Services;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Helpers;
using SampSharp.Entities.SAMP;
using OpenRP.Framework.Shared.Dialogs.Extensions;

namespace OpenRP.Framework.Features.Actors.Services.Dialogs
{
    /// <summary>
    /// The /editactors menus. One file per flow: the list of actors, one actor's menu, and its permissions.
    /// </summary>
    public partial class ActorDialogService : IActorDialogService
    {
        private const string ActorsTitle = "Actors";

        private readonly IDialogService _dialogService;
        private readonly IActorEditorService _actorEditorService;
        private readonly IPropertyEditorService _propertyEditorService;
        private readonly IActorPermissionService _actorPermissionService;

        public ActorDialogService(IDialogService dialogService, IActorEditorService actorEditorService, IPropertyEditorService propertyEditorService,
            IActorPermissionService actorPermissionService)
        {
            _actorPermissionService = actorPermissionService;
            _dialogService = dialogService;
            _actorEditorService = actorEditorService;
            _propertyEditorService = propertyEditorService;
        }

        public void OpenActorEditor(Player player)
        {
            BetterListDialog actorsDialog = new BetterListDialog("Choose", "Close");
            actorsDialog.SetTitle(TitleType.Children, ActorsTitle);
            int createRow = actorsDialog.AddRow($"{ChatColor.CornflowerBlue}Create actor here");
            ServerActor? nearbyActor = _actorEditorService.GetNearbyActor(player);
            int? nearbyActorRow = nearbyActor == null ? null : actorsDialog.AddRow($"{ChatColor.CornflowerBlue}Actor nearest to me: {ChatColor.White}{GetActorName(nearbyActor)}");
            Dictionary<int, ServerActor> actorsByRow = new Dictionary<int, ServerActor>();
            foreach (ServerActor actor in _actorEditorService.GetEditableActors())
            {
                actorsByRow[actorsDialog.AddRow(GetActorLabel(actor))] = actor;
            }

            void OnActorChosen(ListDialogResponse response)
            {
                Action reopenEditor = () => OpenActorEditor(player);
                if (response.Response != DialogResponse.LeftButton)
                    return;
                if (response.ItemIndex == createRow)
                    OpenCreateActor(player, reopenEditor);
                else if (response.ItemIndex == nearbyActorRow && nearbyActor != null)
                    OpenActorActions(player, nearbyActor, reopenEditor);
                else if (actorsByRow.TryGetValue(response.ItemIndex, out ServerActor? actor))
                    OpenActorActions(player, actor, reopenEditor);
            }

            _dialogService.ShowIgnoringDisconnect(player, actorsDialog, OnActorChosen);
        }

        private void OpenCreateActor(Player player, Action onBack)
        {
            void Create(string name)
            {
                (ServerActor? actor, string? error) = _actorEditorService.CreateActor(name, player);
                ContinueAfter(player, error, () => OpenActorActions(player, actor!, onBack), () => OpenCreateActor(player, onBack));
            }

            OpenTextInput(player, "Create actor", $"The actor stands where you stand, facing your way, with your skin. Enter its name, for example {ChatColor.Highlight}Finn O'Hare{ChatColor.White}:", Create, onBack);
        }

        /// <summary>
        /// Continues with onSuccess when there's no error; otherwise shows the error and continues with onAfterError.
        /// </summary>
        private void ContinueAfter(Player player, string? error, Action onSuccess, Action onAfterError)
        {
            if (error == null)
            {
                onSuccess();
                return;
            }

            ShowMessage(player, "Not possible", error, onAfterError);
        }

        private void ShowMessage(Player player, string title, string text, Action onClose)
        {
            BetterMessageDialog messageDialog = new BetterMessageDialog("Back");
            messageDialog.SetTitle(TitleType.Children, ActorsTitle, title);
            messageDialog.SetContent(text);
            _dialogService.ShowIgnoringDisconnect(player, messageDialog, _ => onClose());
        }

        private void ShowConfirmation(Player player, string title, string question, Action onConfirmed, Action onCancelled)
        {
            BetterMessageDialog confirmationDialog = new BetterMessageDialog(DialogHelper.Yes, DialogHelper.No);
            confirmationDialog.SetTitle(TitleType.Children, ActorsTitle, title);
            confirmationDialog.SetContent(question);
            _dialogService.ShowIgnoringDisconnect(player, confirmationDialog, response =>
            {
                if (response.Response == DialogResponse.LeftButton)
                {
                    onConfirmed();
                    return;
                }
                onCancelled();
            });
        }

        private void OpenTextInput(Player player, string title, string prompt, Action<string> onEntered, Action onBack)
        {
            BetterInputDialog inputDialog = new BetterInputDialog("Continue", "Back");
            inputDialog.SetTitle(TitleType.Children, ActorsTitle, title);
            inputDialog.SetContent(prompt);
            _dialogService.ShowIgnoringDisconnect(player, inputDialog, response =>
            {
                if (response.Response != DialogResponse.LeftButton)
                {
                    onBack();
                    return;
                }
                onEntered(response.InputText ?? string.Empty);
            });
        }

        private static string GetActorName(ServerActor actor)
        {
            return $"{actor.GetName()} {ChatColor.LightGray}(#{actor.GetId()}){ChatColor.White}";
        }

        private string GetActorLabel(ServerActor actor)
        {
            return $"{GetActorName(actor)} {ChatColor.LightGray}- {GetWhereText(actor)}";
        }

        /// <summary>
        /// "outside" or "inside Little Lady", for the actor's virtual world.
        /// </summary>
        private string GetWhereText(ServerActor actor)
        {
            ulong? insidePropertyId = actor.GetInsidePropertyId();
            Property? insideProperty = insidePropertyId.HasValue ? _propertyEditorService.GetProperty(insidePropertyId.Value) : null;
            return insideProperty == null ? "outside" : $"inside {insideProperty.GetName()}";
        }
    }
}
