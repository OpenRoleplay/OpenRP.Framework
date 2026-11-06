using OpenRP.Framework.Features.Factions.Services;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.Factions.Systems
{
    public class FactionSystem : ISystem
    {
        private const int SyncIntervalInMilliseconds = 60000;

        [Event]
        public void OnGameModeInit(IFactionManager factionManager)
        {
            factionManager.LoadAll();
        }

        [Timer(SyncIntervalInMilliseconds)]
        public void ProcessChanges(IFactionManager factionManager)
        {
            factionManager.ProcessChanges();
        }

        [Event]
        public void OnGameModeExit(IFactionManager factionManager)
        {
            factionManager.SaveNow();
        }
    }
}
