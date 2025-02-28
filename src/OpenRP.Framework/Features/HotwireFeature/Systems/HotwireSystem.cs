using OpenRP.Framework.Features.HotwireFeature.Components;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Services;
using SampSharp.Entities.SAMP;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRP.Framework.Shared.Chat.Extensions;

namespace OpenRP.Framework.Features.HotwireFeature.Systems
{
    public class HotwireSystem : ISystem
    {
        [Timer(2500)]
        public void HotwiredVehicleTimer(IEntityManager entityManager, IChatService chatService)
        {
            Random random = new Random();

            // Retrieve all players that are hotwiring.
            List<PlayerHotwiring> hotwiringPlayers = entityManager.GetComponents<PlayerHotwiring>().ToList();
            foreach (PlayerHotwiring hotwiringPlayer in hotwiringPlayers)
            {
                Player player = hotwiringPlayer.Player;
                switch (hotwiringPlayer.ProgressStep)
                {
                    case 0:
                        // Step 1: The player uses the tools.
                        chatService.SendPlayerChatMessage(player, PlayerChatMessageType.ME, "carefully inserts the rusty paperclip and manipulates the worn-out screwdriver into the ignition.");
                        hotwiringPlayer.ProgressStep++;
                        break;
                    case 1:
                        // Step 2: If Electrical Tape was used, this is a safe hotwire.
                        // Retrieve the vehicle the player is in and its hotwired component.
                        if (player.InAnyVehicle)
                        {
                            Vehicle veh = entityManager.GetComponent<Vehicle>(player.Vehicle);
                            HotwiredVehicle hotwiredVehicle = veh.GetComponent<HotwiredVehicle>();
                            if (hotwiredVehicle != null)
                            {
                                if (hotwiredVehicle.SafelyHotwired)
                                {
                                    chatService.SendPlayerChatMessage(player, PlayerChatMessageType.ME, "applies electrical tape to secure the connection, ensuring a safe hotwire.");
                                }
                                else
                                {
                                    chatService.SendPlayerChatMessage(player, PlayerChatMessageType.DO, "The wires spark dangerously as they make erratic contact.");
                                }
                            }
                        }
                        hotwiringPlayer.ProgressStep++;
                        break;
                    case 2:
                        // Step 3: Finalize the hotwire process.
                        if (player.InAnyVehicle)
                        {
                            Vehicle veh = entityManager.GetComponent<Vehicle>(player.Vehicle);
                            HotwiredVehicle hotwiredVehicle = veh.GetComponent<HotwiredVehicle>();
                            hotwiredVehicle.SuccessfulHotwire();
                            chatService.SendPlayerChatMessage(player, PlayerChatMessageType.DO, "The engine roars to life after a few sparks fly.");
                            player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, "You successfully hotwired the vehicle!");
                        }
                        // Re-enable player control and end the hotwire process.
                        hotwiringPlayer.EndHotwiring();
                        break;
                }
            }

            // Process all hotwired vehicles for potential shorting.
            List<HotwiredVehicle> hotwiredVehicles = entityManager.GetComponents<HotwiredVehicle>().ToList();
            foreach (HotwiredVehicle hv in hotwiredVehicles)
            {
                // Only consider vehicles that were not safely hotwired.
                if (!hv.SafelyHotwired)
                {
                    // 1 in 20 chance (5%) that the wires short.
                    if (random.Next(20) == 0)
                    {
                        Vehicle veh = hv.Vehicle;

                        // Cause electrical short
                        hv.TriggerHotwireElectricalShort();

                        // If there's a driver, send a /do message to inform them.
                        Player? vehicleDriver = entityManager.GetComponents<Player>().Where(i => i.Vehicle == hv.Vehicle.Entity && i.VehicleSeat == 0).SingleOrDefault();
                        if (vehicleDriver != null)
                        {
                            chatService.SendPlayerChatMessage(vehicleDriver, PlayerChatMessageType.DO, "The wires suddenly short, causing a surge that leaves the vehicle battered.");
                        }
                    }
                }
            }
        }

        [Event]
        public void OnVehicleDeath(Vehicle vehicle, Player killerid)
        {
            vehicle.DestroyComponents<HotwiredVehicle>();
        }
    }
}
