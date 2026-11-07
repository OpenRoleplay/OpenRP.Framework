using OpenRP.Framework.Features.Properties.Services;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.Properties.Systems
{
    public class PropertySystem : ISystem
    {
        private const int SyncIntervalInMilliseconds = 60000;

        /// <summary>
        /// Properties go first because doors are their child entities. Right after loading, the components match the
        /// database exactly, so that's when they're exported as default data.
        /// </summary>
        [Event]
        public void OnGameModeInit(IPropertyManager propertyManager, IPropertyDoorManager propertyDoorManager, IPropertyDefaultDataExporter propertyDefaultDataExporter)
        {
            propertyManager.LoadAll();
            propertyDoorManager.LoadAll();
            propertyDefaultDataExporter.ExportDefaultData();
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
