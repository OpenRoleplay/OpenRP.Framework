using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Properties.Components;
using OpenRP.Framework.Features.VirtualWorlds.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Properties.Services
{
    public class PropertyEditorService : IPropertyEditorService
    {
        private const int NameMaxLength = 64;
        private const int OutsideInterior = 0;

        private readonly IEntityManager _entityManager;
        private readonly IPropertyManager _propertyManager;
        private readonly IPropertyDoorManager _propertyDoorManager;
        private readonly IPropertyService _propertyService;
        private readonly IVirtualWorldManager _virtualWorldManager;

        public PropertyEditorService(IEntityManager entityManager, IPropertyManager propertyManager, IPropertyDoorManager propertyDoorManager,
            IPropertyService propertyService, IVirtualWorldManager virtualWorldManager)
        {
            _entityManager = entityManager;
            _propertyManager = propertyManager;
            _propertyDoorManager = propertyDoorManager;
            _propertyService = propertyService;
            _virtualWorldManager = virtualWorldManager;
        }

        public List<Property> GetProperties()
        {
            return _entityManager.GetComponents<Property>()
                .Where(property => !property.IsDeleted())
                .OrderBy(property => property.GetName(), StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public Property? GetProperty(ulong propertyId)
        {
            return GetProperties().FirstOrDefault(property => property.GetId() == propertyId);
        }

        public List<PropertyDoor> GetAllDoors()
        {
            return GetLoadedDoors()
                .OrderBy(door => door.GetProperty()?.GetName() ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(door => door.GetName(), StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public (Property? Property, string? Error) CreateProperty(string name)
        {
            string? error = GetNameError("property", name);
            if (error != null)
            {
                return (null, error);
            }
            return (_propertyManager.CreateAndLoad(new PropertyModel { Name = name.Trim() }), null);
        }

        public string? RenameProperty(Property property, string name)
        {
            string? error = GetNameError("property", name);
            if (error != null)
            {
                return error;
            }

            property.SetName(name.Trim());
            RedrawDoorLabels(property);
            return null;
        }

        public void DeleteProperty(Property property)
        {
            foreach (PropertyDoor door in property.GetDoors())
            {
                DeleteDoor(door);
            }
            MoveDoorsOutsideOf(property);
            property.ProcessDeletion();
        }

        public (PropertyDoor? Door, string? Error) CreateDoor(Property property, string name, Player player)
        {
            string? error = GetNameError("door", name);
            if (error != null)
            {
                return (null, error);
            }
            PropertyDoorModel doorModel = CreateDoorModelAtPlayer(property, name.Trim(), player);
            doorModel.InsidePropertyId = GetInsidePropertyIdForPlayer(property, player);
            return (_propertyDoorManager.CreateAndLoad(doorModel), null);
        }

        public void SetDoorInsideProperty(PropertyDoor door, Property? insideProperty)
        {
            door.SetInsidePropertyId(insideProperty?.GetId());
            _propertyService.CreateDoorLabel(door);
        }

        public string? RenameDoor(PropertyDoor door, string name)
        {
            string? error = GetNameError("door", name);
            if (error != null)
            {
                return error;
            }

            door.SetName(name.Trim());
            _propertyService.CreateDoorLabel(door);
            return null;
        }

        public void MoveDoorToPlayer(PropertyDoor door, Player player)
        {
            door.SetPosition(player.Position);
            door.SetAngle(player.Angle);
            door.SetInterior(player.Interior);
            door.SetInsidePropertyId(GetInsidePropertyIdForPlayer(door.GetProperty(), player));
            _propertyService.CreateDoorLabel(door);
        }

        public string? LinkDoors(PropertyDoor door, PropertyDoor otherDoor)
        {
            if (door.GetId() == otherDoor.GetId())
            {
                return "A door can't lead to itself.";
            }

            UnlinkDoor(door);
            UnlinkDoor(otherDoor);
            door.SetLinkedDoorId(otherDoor.GetId());
            otherDoor.SetLinkedDoorId(door.GetId());
            return null;
        }

        public void UnlinkDoor(PropertyDoor door)
        {
            PropertyDoor? linkedDoor = GetLinkedDoor(door);
            if (linkedDoor != null && linkedDoor.IsLinkedTo(door))
            {
                linkedDoor.SetLinkedDoorId(null);
            }

            if (door.GetLinkedDoorId().HasValue)
            {
                door.SetLinkedDoorId(null);
            }
        }

        public void DeleteDoor(PropertyDoor door)
        {
            _propertyService.DestroyDoorLabel(door);
            RemoveLinksTo(door);
            door.ProcessDeletion();
        }

        private IEnumerable<PropertyDoor> GetLoadedDoors()
        {
            return _entityManager.GetComponents<PropertyDoor>().Where(door => !door.IsDeleted());
        }

        private PropertyDoor? GetLinkedDoor(PropertyDoor door)
        {
            ulong? linkedDoorId = door.GetLinkedDoorId();
            return linkedDoorId.HasValue ? _propertyService.GetDoor(linkedDoorId.Value) : null;
        }

        /// <summary>
        /// The database clears these links too, but loaded doors would save the old link back over it.
        /// </summary>
        private void RemoveLinksTo(PropertyDoor door)
        {
            foreach (PropertyDoor otherDoor in GetLoadedDoors().Where(otherDoor => otherDoor.IsLinkedTo(door)).ToList())
            {
                otherDoor.SetLinkedDoorId(null);
            }
        }

        /// <summary>
        /// The property a door placed where the player stands is inside: the property whose virtual world the player
        /// is in (an apartment's entrance in a building's hallway is inside the building), otherwise the door's own
        /// property when the player is in an interior, otherwise none, outside. Admins can change it afterwards, for
        /// example for a room built into the map at interior 0.
        /// </summary>
        private ulong? GetInsidePropertyIdForPlayer(Property? doorProperty, Player player)
        {
            ulong? propertyOfVirtualWorld = _virtualWorldManager.GetPropertyIdOfVirtualWorld(player.VirtualWorld);
            if (propertyOfVirtualWorld.HasValue)
            {
                return propertyOfVirtualWorld;
            }

            return player.Interior != OutsideInterior ? doorProperty?.GetId() : null;
        }

        /// <summary>
        /// The database puts these doors outside too, but loaded doors would save the old value back over it.
        /// </summary>
        private void MoveDoorsOutsideOf(Property property)
        {
            foreach (PropertyDoor door in GetLoadedDoors().Where(door => door.GetInsidePropertyId() == property.GetId()).ToList())
            {
                SetDoorInsideProperty(door, null);
            }
        }

        private void RedrawDoorLabels(Property property)
        {
            foreach (PropertyDoor door in property.GetDoors())
            {
                _propertyService.CreateDoorLabel(door);
            }
        }

        private static PropertyDoorModel CreateDoorModelAtPlayer(Property property, string name, Player player)
        {
            Vector3 position = player.Position;
            return new PropertyDoorModel
            {
                Name = name,
                PropertyId = property.GetId(),
                X = position.X,
                Y = position.Y,
                Z = position.Z,
                Angle = player.Angle,
                Interior = player.Interior
            };
        }

        private static string? GetNameError(string thing, string name)
        {
            string trimmedName = name.Trim();
            if (trimmedName.Length == 0)
                return $"The {thing} name can't be empty.";
            if (trimmedName.Length > NameMaxLength)
                return $"The {thing} name can be at most {NameMaxLength} characters long.";
            return null;
        }
    }
}
