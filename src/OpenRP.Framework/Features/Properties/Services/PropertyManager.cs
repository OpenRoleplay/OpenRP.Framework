using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Properties.Components;
using OpenRP.Framework.Features.Properties.Entities;
using OpenRP.Framework.Shared.BaseManager.Helpers;
using SampSharp.Entities;
using System;

namespace OpenRP.Framework.Features.Properties.Services
{
    public class PropertyManager : BaseManager<PropertyModel, Property>, IPropertyManager
    {
        private readonly IPropertyService _propertyService;

        public PropertyManager(BaseDataContext dataContext, IEntityManager entityManager, IPropertyService propertyService, ILogger<PropertyManager> logger)
            : base(dataContext, entityManager, logger)
        {
            _propertyService = propertyService;
        }

        protected override DbSet<PropertyModel> GetDbSet() => _dataContext.Properties;

        protected override Guid EntityTypeKey => PropertyEntities.PropertyType;

        protected override PropertyModel GetModelFromComponent(Property component)
        {
            return component.GetRawPropertyModel();
        }

        /// <summary>
        /// The door labels show the property's name, so they follow when a property is renamed elsewhere.
        /// </summary>
        protected override void OnComponentLoaded(Property component)
        {
            foreach (PropertyDoor door in component.GetDoors())
            {
                _propertyService.CreateDoorLabel(door);
            }
        }
    }
}
