using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Fishing.Components;
using OpenRP.Framework.Features.Fishing.Entities;
using SampSharp.ColAndreas.Entities.Definitions;
using SampSharp.ColAndreas.Entities.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using SampSharp.Streamer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Fishing.Services
{
    public class FishHotspotService : IFishHotspotService
    {
        private IColAndreasService _colAndreasService;
        private IStreamerService _streamerService;
        private IWorldService _worldService;
        private IEntityManager _entityManager;
        public FishHotspotService(IColAndreasService colAndreasService, IStreamerService streamerService, IWorldService worldService, IEntityManager entityManager)
        {
            _colAndreasService = colAndreasService;
            _streamerService = streamerService;
            _worldService = worldService;
            _entityManager = entityManager;
        }

        public async Task<Vector3> GetRandomHotspotPosition()
        {
            var random = new Random();

            while (true)
            {
                // Generate random coordinates between -3100 and 3100
                float x = (float)(random.NextDouble() * 6200 - 3100);
                float y = (float)(random.NextDouble() * 6200 - 3100);

                // Check if coordinates are outside detailed map area
                if (x < -3000 || x > 3000 || y < -3000 || y > 3000)
                {
                    // Return ocean/sea position with fixed water level
                    return new Vector3(x, y, -1.5f);
                }

                // For coordinates within detailed area, check actual water collision
                var startPos = new Vector3(x, y, 1000);  // Start above terrain
                var endPos = new Vector3(x, y, -10);     // Extend below water level

                // Cast vertical ray to find water surface
                if (_colAndreasService.RayCastLine(startPos, endPos, out Vector3 waterSurface) == ColAndreasVars.WATER_MODEL)
                {
                    // Return position at known water height
                    return new Vector3(x, y, -1.5f);
                }

                // Prevent tight loop starvation
                await Task.Delay(100);
            }
        }

        public async Task<DynamicObject> CreateRandomHotspot()
        {
            Vector3 hotspotPosition = await GetRandomHotspotPosition();

            EntityId fishHotspotEntityId = FishHotspotEntities.GenerateFishHotspotId();
            _entityManager.Create(fishHotspotEntityId);
            _entityManager.AddComponent<FishHotspot>(fishHotspotEntityId);

            DynamicObject hotspotObject = _streamerService.CreateDynamicObject(18741, hotspotPosition, Vector3.Zero, parent: fishHotspotEntityId);

            Console.WriteLine($"Fish Hotspot created at {hotspotPosition.X}, {hotspotPosition.Y}, {hotspotPosition.Z}.");

            return hotspotObject;
        }
    }
}
