using OpenRP.Framework.Features.Jurisdictions.Services;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;
using OpenRP.Framework.Shared.ServerEvents.Services;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.Jurisdictions.Systems
{
    public class JurisdictionSystem : ISystem
    {
        private const int SyncIntervalInMilliseconds = 60000;

        /// <summary>
        /// Vehicles may have been created before this ran, so their plates are told to look again.
        /// </summary>
        [Event]
        public void OnGameModeInit(IJurisdictionManager jurisdictionManager, IFactionTaxRegistrationManager factionTaxRegistrationManager, IServerEventAggregator serverEventAggregator)
        {
            jurisdictionManager.LoadAll();
            factionTaxRegistrationManager.LoadAll();

            // Not awaited: the dispatcher logs each handler's errors itself.
            _ = serverEventAggregator.PublishAsync(new OnJurisdictionsLoadedEventArgs());
        }

        /// <summary>
        /// Registrations go first: deleting a jurisdiction deletes its registrations in the database as well.
        /// </summary>
        [Timer(SyncIntervalInMilliseconds)]
        public void ProcessChanges(IJurisdictionManager jurisdictionManager, IFactionTaxRegistrationManager factionTaxRegistrationManager)
        {
            factionTaxRegistrationManager.ProcessChanges();
            jurisdictionManager.ProcessChanges();
        }

        [Event]
        public void OnGameModeExit(IJurisdictionManager jurisdictionManager, IFactionTaxRegistrationManager factionTaxRegistrationManager)
        {
            factionTaxRegistrationManager.SaveNow();
            jurisdictionManager.SaveNow();
        }
    }
}
