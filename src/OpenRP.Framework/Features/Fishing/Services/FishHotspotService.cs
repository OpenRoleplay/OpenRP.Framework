using Microsoft.Extensions.Logging;
using OpenRP.Framework.Features.Fishing.Components;
using OpenRP.Framework.Features.Fishing.Entities;
using OpenRP.OpenMp.Streamer;
using SampSharp.ColAndreas.Entities;
using SampSharp.ColAndreas.Entities.Services;
using SampSharp.Entities;
using System;
using System.Linq;
using System.Numerics;

namespace OpenRP.Framework.Features.Fishing.Services
{
    public class FishHotspotService : IFishHotspotService
    {
        private readonly IColAndreasService _colAndreasService;
        private readonly IStreamerService _streamerService;
        private readonly IEntityManager _entityManager;
        private readonly ILogger<FishHotspotService> _logger;

        public FishHotspotService(IColAndreasService colAndreasService, IStreamerService streamerService, IEntityManager entityManager, ILogger<FishHotspotService> logger)
        {
            _colAndreasService = colAndreasService;
            _streamerService = streamerService;
            _entityManager = entityManager;
            _logger = logger;
        }

        public void CreateHotspots()
        {
            for (int i = 0; i < FishingSettings.HotspotCount; i++)
            {
                CreateRandomHotspot();
            }
        }

        public void MoveHotspots()
        {
            foreach (FishHotspot hotspot in _entityManager.GetComponents<FishHotspot>().ToList())
            {
                hotspot.DestroyEntity();
            }
            CreateHotspots();
        }

        public bool IsNearHotspot(Vector3 position)
        {
            return _entityManager.GetComponents<FishHotspot>().Any(hotspot => hotspot.IsPositionInRange(position, FishingSettings.HotspotRange));
        }

        private void CreateRandomHotspot()
        {
            Vector3? position = FindRandomWaterPosition();
            if (position == null)
            {
                _logger.LogWarning("No water found for a fish hotspot after {Attempts} attempts.", FishingSettings.HotspotMaxPlacementAttempts);
                return;
            }

            EntityId hotspotEntityId = FishHotspotEntities.GenerateFishHotspotId();
            _entityManager.AddComponent<FishHotspot>(hotspotEntityId, new object[] { position.Value });
            _streamerService.CreateDynamicObject(FishingSettings.HotspotObjectModelId, position.Value, Vector3.Zero, parent: hotspotEntityId);
        }

        private Vector3? FindRandomWaterPosition()
        {
            for (int attempt = 0; attempt < FishingSettings.HotspotMaxPlacementAttempts; attempt++)
            {
                if (TryGetWaterSurface(GetRandomMapCoordinate(), GetRandomMapCoordinate(), out Vector3 waterSurface))
                {
                    return waterSurface;
                }
            }
            return null;
        }

        private static float GetRandomMapCoordinate()
        {
            return (Random.Shared.NextSingle() * 2f - 1f) * FishingSettings.HotspotMapEdge;
        }

        /// <summary>
        /// Water counts only when a ray from high above hits it first, so spots under bridges and roofs are skipped.
        /// </summary>
        private bool TryGetWaterSurface(float x, float y, out Vector3 waterSurface)
        {
            Vector3 start = new Vector3(x, y, FishingSettings.HotspotRayStartHeight);
            Vector3 end = new Vector3(x, y, FishingSettings.HotspotRayEndHeight);
            return _colAndreasService.RayCastLineModel(start, end, out waterSurface) == ColAndreasConstants.WaterObject;
        }
    }
}
