using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Shared.BaseManager.Entities;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Properties.Components
{
    /// <summary>
    /// A door of a property, linked to the door on the other side (for example an entrance and its exit).
    /// </summary>
    public class PropertyDoor : Component, IBaseDataComponent, IChangeable, IDeletable
    {
        private const float NearbyRange = 3.0f;
        private const int OutsideInterior = 0;

        private readonly PropertyDoorModel _propertyDoorModel;
        private bool _hasChanges;
        private bool _isDeleted;

        public PropertyDoor(PropertyDoorModel propertyDoorModel)
        {
            _propertyDoorModel = propertyDoorModel;
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

        public PropertyDoorModel GetRawPropertyDoorModel()
        {
            return _propertyDoorModel;
        }

        public ulong GetId()
        {
            return _propertyDoorModel.Id;
        }

        public string GetName()
        {
            return _propertyDoorModel.Name;
        }

        public void SetName(string name)
        {
            _propertyDoorModel.Name = name;
            ProcessChanges();
        }

        public ulong? GetPropertyId()
        {
            return _propertyDoorModel.PropertyId;
        }

        /// <summary>
        /// The id of the door this one leads to, if any.
        /// </summary>
        public ulong? GetLinkedDoorId()
        {
            return _propertyDoorModel.LinkedToPropertyDoorId;
        }

        /// <summary>
        /// Makes this door lead to the door with this id, or nowhere when it's null.
        /// </summary>
        public void SetLinkedDoorId(ulong? linkedDoorId)
        {
            _propertyDoorModel.LinkedToPropertyDoorId = linkedDoorId;
            ProcessChanges();
        }

        public bool IsLinkedTo(PropertyDoor otherDoor)
        {
            return GetLinkedDoorId() == otherDoor.GetId();
        }

        public Vector3 GetPosition()
        {
            return new Vector3(_propertyDoorModel.X, _propertyDoorModel.Y, _propertyDoorModel.Z);
        }

        public void SetPosition(Vector3 position)
        {
            _propertyDoorModel.X = position.X;
            _propertyDoorModel.Y = position.Y;
            _propertyDoorModel.Z = position.Z;
            ProcessChanges();
        }

        public float GetAngle()
        {
            return _propertyDoorModel.Angle;
        }

        public void SetAngle(float angle)
        {
            _propertyDoorModel.Angle = angle;
            ProcessChanges();
        }

        public int GetInterior()
        {
            return _propertyDoorModel.Interior ?? OutsideInterior;
        }

        public void SetInterior(int interior)
        {
            _propertyDoorModel.Interior = interior;
            ProcessChanges();
        }

        /// <summary>
        /// The id of the property whose virtual world the door is in, or null when it's outside, in the shared world.
        /// </summary>
        public ulong? GetInsidePropertyId()
        {
            return _propertyDoorModel.InsidePropertyId;
        }

        public void SetInsidePropertyId(ulong? insidePropertyId)
        {
            _propertyDoorModel.InsidePropertyId = insidePropertyId;
            ProcessChanges();
        }

        /// <summary>
        /// True when the door is inside a property, in its virtual world, rather than in the shared world.
        /// </summary>
        public bool IsInside()
        {
            return GetInsidePropertyId() != null;
        }

        public bool IsPlayerNearby(Player player)
        {
            return player.IsInRangeOfPoint(NearbyRange, GetPosition());
        }

        /// <summary>
        /// The property this door belongs to, or null when it has none.
        /// </summary>
        public Property? GetProperty()
        {
            return GetComponentInParent<Property>();
        }

        /// <summary>
        /// The text shown above the door, for example "Entrance of Palomino Creek Little Lady".
        /// </summary>
        public string GetLabelText()
        {
            string? propertyName = GetProperty()?.GetName();
            if (string.IsNullOrEmpty(propertyName))
            {
                return GetName();
            }
            if (string.IsNullOrEmpty(GetName()))
            {
                return propertyName;
            }
            return $"{GetName()} of {propertyName}";
        }
    }
}
