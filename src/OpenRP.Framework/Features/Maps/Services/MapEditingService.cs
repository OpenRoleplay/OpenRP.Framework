using OpenRP.Framework.Features.FlyMode.Services;
using OpenRP.Framework.Features.Maps.Components;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System.Linq;

namespace OpenRP.Framework.Features.Maps.Services
{
    public class MapEditingService : IMapEditingService
    {
        private const float OnFootInFrontDistance = 2f;
        private const float FlyingInFrontDistance = 3f;
        private const string NotEditingMessage = "You aren't editing a map. Use /editmaps to start.";
        private const string NotPartOfMapMessage = "That object isn't part of a map.";
        private const string OtherMapName = "another map";

        private readonly IEntityManager _entityManager;
        private readonly IMapService _mapService;
        private readonly IFlyModeService _flyModeService;

        public MapEditingService(IEntityManager entityManager, IMapService mapService, IFlyModeService flyModeService)
        {
            _entityManager = entityManager;
            _mapService = mapService;
            _flyModeService = flyModeService;
        }

        public bool IsEditing(Player player)
        {
            return player.GetComponent<MapEditing>() != null;
        }

        public Map? GetEditedMap(Player player)
        {
            MapEditing? editing = player.GetComponent<MapEditing>();
            return editing == null ? null : _mapService.GetMap(editing.GetMapId());
        }

        public void StartEditing(Player player, Map map)
        {
            player.DestroyComponents<MapEditing>();
            player.AddComponent<MapEditing>(map.GetId());
        }

        public void StopEditing(Player player)
        {
            player.CancelEdit();
            if (_flyModeService.IsFlying(player))
            {
                _flyModeService.StopFlying(player);
            }
            player.DestroyComponents<MapEditing>();
        }

        public void StopEditingMap(Map map)
        {
            foreach (Player player in _entityManager.GetComponents<Player>().ToList())
            {
                if (IsEditingMap(player, map))
                {
                    StopEditing(player);
                }
            }
        }

        private static bool IsEditingMap(Player player, Map map)
        {
            MapEditing? editing = player.GetComponent<MapEditing>();
            return editing != null && editing.GetMapId() == map.GetId();
        }

        public string? SelectObject(Player player, MapObject? mapObject)
        {
            MapEditing? editing = player.GetComponent<MapEditing>();
            if (editing == null)
            {
                return NotEditingMessage;
            }
            if (mapObject == null)
            {
                return NotPartOfMapMessage;
            }
            if (mapObject.GetMapId() != editing.GetMapId())
            {
                return $"That object belongs to map {mapObject.GetMap()?.GetName() ?? OtherMapName}.";
            }

            editing.SetSelectedObjectId(mapObject.GetId());
            player.CancelEdit();
            return null;
        }

        public void ForgetObject(Player player, MapObject mapObject)
        {
            MapEditing? editing = player.GetComponent<MapEditing>();
            if (editing != null && editing.GetSelectedObjectId() == mapObject.GetId())
            {
                editing.SetSelectedObjectId(null);
            }
        }

        public Vector3 GetEditorOrigin(Player player)
        {
            return _flyModeService.GetCameraPosition(player) ?? player.Position;
        }

        public Vector3 GetPointInFrontOfEditor(Player player)
        {
            Vector3? cameraPosition = _flyModeService.GetCameraPosition(player);
            if (cameraPosition.HasValue)
            {
                return cameraPosition.Value + player.CameraFrontVector * FlyingInFrontDistance;
            }

            Vector2 pointInFront = player.GetPointInFront(OnFootInFrontDistance);
            return new Vector3(pointInFront.X, pointInFront.Y, player.Position.Z);
        }
    }
}
