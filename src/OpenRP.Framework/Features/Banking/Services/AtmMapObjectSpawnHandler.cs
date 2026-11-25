using OpenRP.Framework.Features.Banking.Entities;
using OpenRP.Framework.Features.Maps.Components;
using OpenRP.Framework.Features.Maps.Entities;
using OpenRP.OpenMp.Streamer;

namespace OpenRP.Framework.Features.Banking.Services
{
    /// <summary>
    /// Makes every placed ATM map object (kmb_atm1) an ATM. The ATM lives on the object's streamer entity, so it goes
    /// away when the object despawns, and it reads its position from the map object, so editor moves need nothing.
    /// </summary>
    public class AtmMapObjectSpawnHandler : IMapObjectSpawnHandler
    {
        private readonly IAtmService _atmService;

        public AtmMapObjectSpawnHandler(IAtmService atmService)
        {
            _atmService = atmService;
        }

        public bool Handles(int modelId)
        {
            return modelId == BankingRules.AtmModelId;
        }

        public void OnMapObjectSpawned(MapObject mapObject, DynamicObject dynamicObject)
        {
            _atmService.AddAtm(dynamicObject.Entity, mapObject.Entity);
        }
    }
}
