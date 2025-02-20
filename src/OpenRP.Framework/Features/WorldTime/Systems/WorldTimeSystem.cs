using OpenRP.Framework.Features.CDN.Services;
using OpenRP.Framework.Features.WorldTime.Services;
using OpenRP.Framework.Shared.Chat.Enums;
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
        [Timer(30000)]
        public void UpdateWorldTime(IEntityManager entityManager, IWorldTimeService worldTimeService)
        {
            foreach (Player player in entityManager.GetComponents<Player>())
            {
                Vector3 playerPosition = player.Position;

                TimeSpan simulationTime = worldTimeService.GetCurrentSimulationTime(playerPosition.X);
                TimeSpan ingameTime = worldTimeService.GetCurrentIngameTime();

                int ingameHours = (int)ingameTime.TotalHours % 24;
                int ingameMinutes = ingameTime.Minutes % 60;

                int simulationHours = (int)simulationTime.TotalHours % 24;
                int simulationMinutes = simulationTime.Minutes % 60;

                player.SetTime(simulationHours, simulationMinutes);

                // Format with leading zeros using :D2 format specifier
                player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO,
                    $"In-Game Time: {ingameHours:D2}:{ingameMinutes:D2} | Simulation Time: {simulationHours:D2}:{simulationMinutes:D2}");
            }
        }
    }
}
