using OpenRP.Framework.Database;
using OpenRP.Framework.Features.Phones.Services;
using OpenRP.Framework.Shared.Batteries.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Phones.Systems
{
    public class PhoneSystem : ISystem
    {
        private const int SyncIntervalInMilliseconds = 60000;
        private const int CallIntervalInMilliseconds = 1000;
        // Battery levels change slowly; charging finishes within a second of being done.
        private const int BatteryIntervalInMilliseconds = 30000;
        private const int ChargingIntervalInMilliseconds = 1000;

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

        /// <summary>
        /// The data context comes in per call: it's transient, and the timer reads offline characters from it.
        /// </summary>
        [Timer(BatteryIntervalInMilliseconds)]
        public void UpdateBatteries(IPhoneBatteryService batteryService, IBatteryPlaceService batteryPlaceService, BaseDataContext dataContext)
        {
            batteryService.UpdateBatteries(batteryPlaceService.TakeSnapshot(dataContext));
        }

        [Timer(ChargingIntervalInMilliseconds)]
        public void ProcessCharging(IPhoneBatteryService batteryService)
        {
            batteryService.ProcessCharging();
        }

        [Event]
        public void OnPlayerDisconnect(Player player, DisconnectReason disconnectReason, IPhoneCallService phoneCallService)
        {
            phoneCallService.EndCallsOf(player);
        }
    }
}
