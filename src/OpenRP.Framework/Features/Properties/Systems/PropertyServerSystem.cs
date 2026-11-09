using OpenRP.Framework.Features.Properties.Services;
using OpenRP.Framework.Shared.ServerEvents.Attributes;
using OpenRP.Framework.Shared.ServerEvents.Entities;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;

namespace OpenRP.Framework.Features.Properties.Systems
{
    public class PropertyServerSystem : IServerSystem
    {
        [ServerEvent]
        public void OnJurisdictionDeleted(OnJurisdictionDeletedEventArgs args, IPropertyEditorService propertyEditorService)
        {
            propertyEditorService.RemovePropertiesFromJurisdiction(args.JurisdictionId);
        }
    }
}
