using OpenRP.Framework.Features.CDN.Services;
using OpenRP.Framework.Features.WorldTime.Services;
using OpenRP.Framework.Shared.Chat.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.WorldTime.Systems
{
    public class WorldTimeSystem : ISystem
    {
        [Timer(60000)]
        public void UpdateWorldTime(IEntityManager entityManager, IWorldTimeService worldTimeService)
        {
            foreach (Player player in entityManager.GetComponents<Player>())
            {
                Vector3 playerPosition = player.Position;

                TimeSpan simulationTime = worldTimeService.GetCurrentSimulationTime(playerPosition.X);
                TimeSpan ingameTime = worldTimeService.GetCurrentIngameTime();

                player.SetTime(simulationTime.Hours, simulationTime.Minutes);
            }
        }
    }
}
