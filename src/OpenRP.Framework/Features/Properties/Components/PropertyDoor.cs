using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Shared.BaseManager.Entities;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.Properties.Components
{
    /// <summary>
    /// A door of a property, linked to the door on the other side (for example an entrance and its exit).
    /// </summary>
    public class PropertyDoor : Component, IBaseDataComponent, IChangeable
    {
        private const float NearbyRange = 3.0f;
        private const int OutsideInterior = 0;

        private readonly PropertyDoorModel _propertyDoorModel;
        private bool _hasChanges;

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

        public Vector3 GetPosition()
        {
            return new Vector3(_propertyDoorModel.X, _propertyDoorModel.Y, _propertyDoorModel.Z);
        }

        public float GetAngle()
        {
            return _propertyDoorModel.Angle;
        }

        public int GetInterior()
        {
            return _propertyDoorModel.Interior ?? OutsideInterior;
        }

        /// <summary>
        /// True when the door is inside an interior rather than in the open world.
        /// </summary>
        public bool IsInside()
        {
            return GetInterior() != OutsideInterior;
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
