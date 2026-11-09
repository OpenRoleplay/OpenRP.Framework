using OpenRP.Framework.Features.Properties.Components;
using SampSharp.Entities.SAMP;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Properties.Services
{
    /// <summary>
    /// Creating, changing and deleting properties and their doors, for the admin tools. Changes show in the game
    /// right away and are saved with the next property sync. Methods that can fail return the reason, or null.
    /// </summary>
    public interface IPropertyEditorService
    {
        /// <summary>
        /// Every loaded property that isn't deleted, by name.
        /// </summary>
        List<Property> GetProperties();

        /// <summary>
        /// The loaded property with this id, or null when it doesn't exist or is deleted.
        /// </summary>
        Property? GetProperty(ulong propertyId);

        /// <summary>
        /// Every loaded door that isn't deleted, by property name and then door name.
        /// </summary>
        List<PropertyDoor> GetAllDoors();

        /// <summary>
        /// Creates a property without doors. The property is null when the name isn't valid.
        /// </summary>
        (Property? Property, string? Error) CreateProperty(string name);

        string? RenameProperty(Property property, string name);

        /// <summary>
        /// Deletes the property and all of its doors.
        /// </summary>
        void DeleteProperty(Property property);

        /// <summary>
        /// Creates a door of the property where the player stands, facing the way they face. It's inside the property
        /// whose virtual world the player is in; otherwise inside its own property when the player is in an interior,
        /// otherwise outside. The door is null when the name isn't valid.
        /// </summary>
        (PropertyDoor? Door, string? Error) CreateDoor(Property property, string name, Player player);

        string? RenameDoor(PropertyDoor door, string name);

        /// <summary>
        /// Moves the door to where the player stands, facing the way they face, in their interior. Which property
        /// it's inside is picked again, the same way as for a new door.
        /// </summary>
        void MoveDoorToPlayer(PropertyDoor door, Player player);

        /// <summary>
        /// Puts the door inside the property, in its virtual world, or outside in the shared world when it's null.
        /// </summary>
        void SetDoorInsideProperty(PropertyDoor door, Property? insideProperty);

        /// <summary>
        /// Makes the two doors lead to each other. Doors that led back to either of them no longer do.
        /// </summary>
        string? LinkDoors(PropertyDoor door, PropertyDoor otherDoor);

        /// <summary>
        /// Makes the door lead nowhere, and the door it led to no longer lead back to it.
        /// </summary>
        void UnlinkDoor(PropertyDoor door);

        /// <summary>
        /// Deletes the door. Doors that led to it lead nowhere afterwards.
        /// </summary>
        void DeleteDoor(PropertyDoor door);
    }
}
