using OpenRP.Framework.Features.Properties.Services;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.Properties.Systems
{
    public class PropertySystem : ISystem
    {
        private const int SyncIntervalInMilliseconds = 60000;

        /// <summary>
        /// Properties go first because doors are their child entities.
        /// </summary>
        [Event]
        public void OnGameModeInit(IPropertyManager propertyManager, IPropertyDoorManager propertyDoorManager)
        {
            propertyManager.LoadAll();
            propertyDoorManager.LoadAll();
        }

        [Timer(SyncIntervalInMilliseconds)]
        public void ProcessChanges(IPropertyManager propertyManager, IPropertyDoorManager propertyDoorManager)
        {
            propertyManager.ProcessChanges();
            propertyDoorManager.ProcessChanges();
        }

        [Event]
        public void OnGameModeExit(IPropertyManager propertyManager, IPropertyDoorManager propertyDoorManager)
        {
            propertyManager.SaveNow();
            propertyDoorManager.SaveNow();
        }
    }
}
