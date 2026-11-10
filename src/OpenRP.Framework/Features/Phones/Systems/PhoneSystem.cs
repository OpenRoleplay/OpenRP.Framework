using OpenRP.Framework.Features.Phones.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Phones.Systems
{
    public class PhoneSystem : ISystem
    {
        private const int SyncIntervalInMilliseconds = 60000;
        private const int CallIntervalInMilliseconds = 1000;

        /// <summary>
        /// Phones go first, because contacts, texts and call logs belong to a phone.
        /// </summary>
        [Event]
        public void OnGameModeInit(IPhoneManager phoneManager, IPhoneContactManager phoneContactManager, IPhoneMessageManager phoneMessageManager, IPhoneCallLogManager phoneCallLogManager)
        {
            phoneManager.LoadAll();
            phoneContactManager.LoadAll();
            phoneMessageManager.LoadAll();
            phoneCallLogManager.LoadAll();
        }

        [Timer(SyncIntervalInMilliseconds)]
        public void ProcessChanges(IPhoneManager phoneManager, IPhoneContactManager phoneContactManager, IPhoneMessageManager phoneMessageManager, IPhoneCallLogManager phoneCallLogManager)
        {
            phoneManager.ProcessChanges();
            phoneContactManager.ProcessChanges();
            phoneMessageManager.ProcessChanges();
            phoneCallLogManager.ProcessChanges();
        }

        [Event]
        public void OnGameModeExit(IPhoneManager phoneManager, IPhoneContactManager phoneContactManager, IPhoneMessageManager phoneMessageManager, IPhoneCallLogManager phoneCallLogManager)
        {
            phoneManager.SaveNow();
            phoneContactManager.SaveNow();
            phoneMessageManager.SaveNow();
            phoneCallLogManager.SaveNow();
        }

        [Timer(CallIntervalInMilliseconds)]
        public void ProcessCalls(IPhoneCallService phoneCallService)
        {
            phoneCallService.ProcessCalls();
        }

        [Event]
        public void OnPlayerDisconnect(Player player, DisconnectReason disconnectReason, IPhoneCallService phoneCallService)
        {
            phoneCallService.EndCallsOf(player);
        }
    }
}
