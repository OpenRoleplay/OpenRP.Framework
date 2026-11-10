using OpenRP.Framework.Features.EmergencyCalls.Enums;
using OpenRP.Framework.Features.Phones.Entities;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Extensions;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.EmergencyCalls.Services.Dialogs
{
    public class EmergencyCallDialogService : IEmergencyCallDialogService
    {
        private const string Title = "911";
        private const int AnswerMaxLength = 120;

        private readonly IDialogService _dialogService;

        public EmergencyCallDialogService(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public void OpenEmergencyForm(Player caller, IServerPhoneLine line, Action<EmergencyServiceType, string, string> onFilled)
        {
            BetterListDialog serviceDialog = new BetterListDialog("Choose", "Hang up");
            serviceDialog.SetTitle(TitleType.Children, Title, "Which service?");
            Dictionary<int, EmergencyServiceType> servicesByRow = new Dictionary<int, EmergencyServiceType>
            {
                [serviceDialog.AddRow("Police")] = EmergencyServiceType.Police,
                [serviceDialog.AddRow("Fire department")] = EmergencyServiceType.Fire,
                [serviceDialog.AddRow("Ambulance")] = EmergencyServiceType.Ambulance
            };

            _dialogService.ShowIgnoringDisconnect(caller, serviceDialog, response =>
            {
                if (!line.IsActive())
                    return;
                if (response.Response != DialogResponse.LeftButton || !servicesByRow.TryGetValue(response.ItemIndex, out EmergencyServiceType service))
                {
                    line.HangUp();
                    return;
                }
                OpenAnswer(caller, line, "What is happening?", what =>
                    OpenAnswer(caller, line, "Where are you? Give a street, building or landmark.", where => onFilled(service, what, where)));
            });
        }

        /// <summary>
        /// Asks one question, again when the answer is empty. Hangs up when the caller cancels.
        /// </summary>
        private void OpenAnswer(Player caller, IServerPhoneLine line, string question, Action<string> onAnswered)
        {
            BetterInputDialog answerDialog = new BetterInputDialog("Continue", "Hang up");
            answerDialog.SetTitle(TitleType.Children, Title, question);
            answerDialog.SetContent($"{question} (at most {AnswerMaxLength} characters)");

            _dialogService.ShowIgnoringDisconnect(caller, answerDialog, response =>
            {
                if (!line.IsActive())
                    return;
                if (response.Response != DialogResponse.LeftButton)
                {
                    line.HangUp();
                    return;
                }

                string answer = response.InputText.Trim();
                if (answer.Length == 0)
                {
                    OpenAnswer(caller, line, question, onAnswered);
                    return;
                }
                onAnswered(answer.Length > AnswerMaxLength ? answer[..AnswerMaxLength] : answer);
            });
        }
    }
}
