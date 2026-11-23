using OpenRP.Framework.Features.Maps.Components;
using OpenRP.Framework.Features.Maps.Entities;
using OpenRP.OpenMp.Streamer;

namespace OpenRP.Framework.Features.Harvestables.Services
{
    /// <summary>
    /// Makes the map objects of harvestable models into plants. The plant lives on the object's streamer entity, so it goes
    /// away when the object despawns, and it's moved along with the object in the editor.
    /// </summary>
    public class HarvestableMapObjectSpawnHandler : IMapObjectSpawnHandler
    {
        private readonly IHarvestableService _harvestableService;

        public HarvestableMapObjectSpawnHandler(IHarvestableService harvestableService)
        {
            _harvestableService = harvestableService;
        }

        public bool Handles(int modelId)
        {
            return _harvestableService.IsHarvestableModel(modelId);
        }

        public void OnMapObjectSpawned(MapObject mapObject, DynamicObject dynamicObject)
        {
            AddPlant(mapObject, dynamicObject);
        }

        public void OnMapObjectMoved(MapObject mapObject, DynamicObject dynamicObject)
        {
            _harvestableService.RemoveHarvestable(dynamicObject.Entity);
            AddPlant(mapObject, dynamicObject);
        }

        private void AddPlant(MapObject mapObject, DynamicObject dynamicObject)
        {
            _harvestableService.AddHarvestable(dynamicObject.Entity, mapObject.GetModelId(), mapObject.GetPosition());
        }
    }
}
