using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Properties.Components;
using OpenRP.Framework.Features.Properties.Entities;
using OpenRP.Framework.Shared.BaseManager.Helpers;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.Properties.Services
{
    public class PropertyDoorManager : BaseManager<PropertyDoorModel, PropertyDoor>, IPropertyDoorManager
    {
        private readonly IPropertyService _propertyService;

        public PropertyDoorManager(BaseDataContext dataContext, IEntityManager entityManager, IPropertyService propertyService, ILogger<PropertyDoorManager> logger)
            : base(dataContext, entityManager, logger)
        {
            _propertyService = propertyService;
        }

        protected override DbSet<PropertyDoorModel> GetDbSet() => _dataContext.PropertyDoors;

        protected override EntityId GetEntityId(PropertyDoorModel model)
        {
            return PropertyEntities.GetPropertyDoorId((int)model.Id);
        }

        protected override EntityId? GetParentEntityId(PropertyDoorModel model)
        {
            return model.PropertyId.HasValue ? PropertyEntities.GetPropertyId((int)model.PropertyId.Value) : null;
        }

        protected override PropertyDoorModel GetModelFromComponent(PropertyDoor component)
        {
            return component.GetRawPropertyDoorModel();
        }

        protected override void OnComponentLoaded(PropertyDoor component)
        {
            _propertyService.CreateDoorLabel(component);
        }
    }
}
