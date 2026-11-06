using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Shared.BaseManager.Entities;
using SampSharp.Entities;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Properties.Components
{
    /// <summary>
    /// A building or place with doors. Its doors are child entities.
    /// </summary>
    public class Property : Component, IBaseDataComponent, IChangeable
    {
        private readonly PropertyModel _propertyModel;
        private bool _hasChanges;

        public Property(PropertyModel propertyModel)
        {
            _propertyModel = propertyModel;
        }

        public bool HasChanges()
        {
            return _hasChanges;
        }

        public void ProcessChanges(bool processChanges = true)
        {
            _hasChanges = processChanges;
        }

        public PropertyModel GetRawPropertyModel()
        {
            return _propertyModel;
        }

        public ulong GetId()
        {
            return _propertyModel.Id;
        }

        public string GetName()
        {
            return _propertyModel.Name;
        }

        /// <summary>
        /// The doors of this property that are loaded.
        /// </summary>
        public List<PropertyDoor> GetDoors()
        {
            return GetComponentsInChildren<PropertyDoor>().ToList();
        }
    }
}
