using OpenRP.Framework.Features.Phones.Components;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Phones.Services.Dialogs
{
    public interface IPhoneDialogService
    {
        /// <summary>
        /// Opens the phone menu, after picking a phone when the player carries several.
        /// </summary>
        void OpenPhone(Player player);

        /// <summary>
        /// Asks whether to answer the call ringing on the phone.
        /// </summary>
        void OpenIncomingCall(Player player, Phone phone, uint callerNumber);
    }
}
