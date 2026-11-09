using OpenRP.Framework.Features.Properties.Components;
using OpenRP.Framework.Features.VirtualWorlds.Services;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.OpenMp.Streamer;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Properties.Services
{
    public class PropertyService : IPropertyService
    {
        private const int OutsideVirtualWorld = 0;
        private const float LabelDrawDistance = 10.0f;

        private readonly IEntityManager _entityManager;
        private readonly IVirtualWorldManager _virtualWorldManager;
        private readonly IStreamerService _streamerService;

        public PropertyService(IEntityManager entityManager, IVirtualWorldManager virtualWorldManager, IStreamerService streamerService)
        {
            _entityManager = entityManager;
            _virtualWorldManager = virtualWorldManager;
            _streamerService = streamerService;
        }

        public void UseNearbyDoor(Player player)
        {
            PropertyDoor? door = GetNearbyDoor(player);
            if (door == null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "You are not near any door!");
                return;
            }

            PropertyDoor? destinationDoor = GetDestinationDoor(door);
            if (destinationDoor == null)
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.ERROR, "This door doesn't lead anywhere.");
                return;
            }

            MovePlayerToDoor(player, destinationDoor);
        }

        public PropertyDoor? GetDestinationDoor(PropertyDoor door)
        {
            ulong? linkedDoorId = door.GetLinkedDoorId();
            if (linkedDoorId.HasValue)
            {
                return GetDoor(linkedDoorId.Value);
            }

            return GetDoorsLinkingTo(door).FirstOrDefault();
        }

        public List<PropertyDoor> GetDoorsLinkingTo(PropertyDoor door)
        {
            return GetDoors().Where(otherDoor => otherDoor.IsLinkedTo(door)).OrderBy(otherDoor => otherDoor.GetId()).ToList();
        }

        public PropertyDoor? GetNearbyDoor(Player player)
        {
            return GetDoors().FirstOrDefault(door => IsPlayerAtDoor(player, door));
        }

        public PropertyDoor? GetDoor(ulong doorId)
        {
            return GetDoors().FirstOrDefault(door => door.GetId() == doorId);
        }

        public int GetDoorVirtualWorld(PropertyDoor door)
        {
            ulong? insidePropertyId = door.GetInsidePropertyId();
            return insidePropertyId.HasValue ? _virtualWorldManager.GetPropertyVirtualWorld(insidePropertyId.Value) : OutsideVirtualWorld;
        }

        public void MovePlayerToDoor(Player player, PropertyDoor door)
        {
            player.Interior = door.GetInterior();
            player.VirtualWorld = GetDoorVirtualWorld(door);
            player.Position = door.GetPosition();
            player.Angle = door.GetAngle();
        }

        public void CreateDoorLabel(PropertyDoor door)
        {
            DestroyDoorLabel(door);
            _streamerService.CreateDynamicTextLabel(
                door.GetLabelText(),
                Color.White,
                door.GetPosition(),
                LabelDrawDistance,
                virtualWorld: GetDoorVirtualWorld(door),
                interior: door.GetInterior(), parent: door.Entity);
        }

        public void DestroyDoorLabel(PropertyDoor door)
        {
            foreach (DynamicTextLabel label in door.GetComponentsInChildren<DynamicTextLabel>().ToList())
            {
                label.DestroyEntity();
            }
        }

        /// <summary>
        /// Deleted doors stay loaded until the next save, but are already gone from the game.
        /// </summary>
        private IEnumerable<PropertyDoor> GetDoors()
        {
            return _entityManager.GetComponents<PropertyDoor>().Where(door => !door.IsDeleted());
        }

        /// <summary>
        /// Doors are the same position in every interior and virtual world, so the player must also be in the door's.
        /// </summary>
        private bool IsPlayerAtDoor(Player player, PropertyDoor door)
        {
            return door.IsPlayerNearby(player)
                && player.Interior == door.GetInterior()
                && player.VirtualWorld == GetDoorVirtualWorld(door);
        }
    }
}
