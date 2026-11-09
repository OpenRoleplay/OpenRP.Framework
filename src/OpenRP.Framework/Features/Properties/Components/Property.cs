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
    public class Property : Component, IBaseDataComponent, IChangeable, IDeletable
    {
        private readonly PropertyModel _propertyModel;
        private bool _hasChanges;
        private bool _isDeleted;

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

        public bool IsDeleted()
        {
            return _isDeleted;
        }

        public void ProcessDeletion(bool processDeletion = true)
        {
            _isDeleted = processDeletion;
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

        public void SetName(string name)
        {
            _propertyModel.Name = name;
            ProcessChanges();
        }

        /// <summary>
        /// The jurisdiction the property lies in, where it would pay property tax, or null for none.
        /// </summary>
        public ulong? GetJurisdictionId()
        {
            return _propertyModel.JurisdictionId;
        }

        public void SetJurisdictionId(ulong? jurisdictionId)
        {
            _propertyModel.JurisdictionId = jurisdictionId;
            ProcessChanges();
        }

        /// <summary>
        /// The doors of this property that are loaded and not deleted.
        /// </summary>
        public List<PropertyDoor> GetDoors()
        {
            return GetComponentsInChildren<PropertyDoor>().Where(door => !door.IsDeleted()).ToList();
        }
    }
}
