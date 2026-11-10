using OpenRP.Framework.Features.Phones.Components;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using OpenRP.Framework.Shared.Dialogs.Extensions;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Phones.Services.Dialogs
{
    public partial class PhoneDialogService
    {
        public void OpenIncomingCall(Player player, Phone phone, uint callerNumber)
        {
            BetterMessageDialog incomingDialog = new BetterMessageDialog("Answer", "Decline");
            incomingDialog.SetTitle(TitleType.Children, GetTitle(phone), "Incoming call");
            incomingDialog.SetContent($"Incoming call from {Highlight(_phoneService.GetNumberLabel(phone, callerNumber))}.");

            _dialogService.ShowIgnoringDisconnect(player, incomingDialog, response =>
            {
                // The call may have stopped ringing while the dialog was open; then there's nothing to answer or decline.
                if (_phoneCallService.GetCallOf(player)?.IsRinging() != true)
                {
                    return;
                }

                if (response.Response == DialogResponse.LeftButton)
                {
                    _phoneCallService.Answer(player);
                    return;
                }
                _phoneCallService.HangUp(player);
            });
        }
    }
}
