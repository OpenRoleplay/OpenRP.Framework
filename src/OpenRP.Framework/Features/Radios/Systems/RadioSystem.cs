using OpenRP.Framework.Database;
using OpenRP.Framework.Features.Radios.Services;
using OpenRP.Framework.Shared.Batteries.Services;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;
using OpenRP.Framework.Shared.ServerEvents.Services;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.Radios.Systems
{
    public class RadioSystem : ISystem
    {
        private const int SyncIntervalInMilliseconds = 60000;
        // Battery levels change slowly; charging finishes within a second of being done.
        private const int BatteryIntervalInMilliseconds = 30000;
        private const int ChargingIntervalInMilliseconds = 1000;

        /// <summary>
        /// Frequencies go before their factions, which belong to a frequency.
        /// </summary>
        [Event]
        public void OnGameModeInit(IRadioFrequencyManager radioFrequencyManager, IRadioFrequencyFactionManager radioFrequencyFactionManager, IRadioManager radioManager,
            IServerEventAggregator serverEventAggregator)
        {
            radioFrequencyManager.LoadAll();
            radioFrequencyFactionManager.LoadAll();
            radioManager.LoadAll();

            // Not awaited: the dispatcher logs each handler's errors itself.
            _ = serverEventAggregator.PublishAsync(new OnRadiosLoadedEventArgs());
        }

        /// <summary>
        /// Factions go first: deleting a frequency deletes its factions in the database as well.
        /// </summary>
        [Timer(SyncIntervalInMilliseconds)]
        public void ProcessChanges(IRadioFrequencyManager radioFrequencyManager, IRadioFrequencyFactionManager radioFrequencyFactionManager, IRadioManager radioManager)
        {
            radioFrequencyFactionManager.ProcessChanges();
            radioFrequencyManager.ProcessChanges();
            radioManager.ProcessChanges();
        }

        /// <summary>
        /// The data context comes in per call: it's transient, and the timer reads offline characters from it.
        /// </summary>
        [Timer(BatteryIntervalInMilliseconds)]
        public void UpdateBatteries(IRadioBatteryService batteryService, IBatteryPlaceService batteryPlaceService, BaseDataContext dataContext)
        {
            batteryService.UpdateBatteries(batteryPlaceService.TakeSnapshot(dataContext));
        }

        [Timer(ChargingIntervalInMilliseconds)]
        public void ProcessCharging(IRadioBatteryService batteryService)
        {
            batteryService.ProcessCharging();
        }

        [Event]
        public void OnGameModeExit(IRadioFrequencyManager radioFrequencyManager, IRadioFrequencyFactionManager radioFrequencyFactionManager, IRadioManager radioManager)
        {
            radioFrequencyFactionManager.SaveNow();
            radioFrequencyManager.SaveNow();
            radioManager.SaveNow();
        }
    }
}
