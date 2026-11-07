using OpenRP.Framework.Features.Vehicles.Components;
using OpenRP.Framework.Features.Vehicles.Services;
using SampSharp.Entities;
using System.Linq;

namespace OpenRP.Framework.Features.Vehicles.Systems
{
    public class EngineStartSystem : ISystem
    {
        // Often enough that the one-second engine start doesn't feel late.
        private const int EngineStartCheckIntervalInMilliseconds = 250;

        [Timer(EngineStartCheckIntervalInMilliseconds)]
        public void FinishDueEngineStarts(IEntityManager entityManager, IVehicleEngineService vehicleEngineService)
        {
            foreach (EngineStarting engineStarting in entityManager.GetComponents<EngineStarting>().Where(engineStarting => engineStarting.IsDue()).ToList())
            {
                vehicleEngineService.FinishEngineStart(engineStarting);
            }
        }
    }
}
