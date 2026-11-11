using OpenRP.Framework.Features.Inventories.Components;
using OpenRP.Framework.Features.Radios.Components;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Batteries.Entities;
using OpenRP.Framework.Shared.Batteries.Enums;
using OpenRP.Framework.Shared.Batteries.Helpers;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Services;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Radios.Services
{
    public class RadioBatteryService : IRadioBatteryService
    {
        // A 1992 police handheld with a NiCd battery: about one 10-hour shift switched on. Talking costs extra.
        private static readonly TimeSpan OnLife = TimeSpan.FromHours(10);
        private const double TransmitCost = 0.25;

        private readonly IEntityManager _entityManager;
        private readonly IRadioService _radioService;
        private readonly IChatService _chatService;

        public RadioBatteryService(IEntityManager entityManager, IRadioService radioService, IChatService chatService)
        {
            _entityManager = entityManager;
            _radioService = radioService;
            _chatService = chatService;
        }

        public void UpdateBatteries(BatteryPlaces places)
        {
            DateTime now = DateTime.UtcNow;
            Dictionary<ulong, InventoryItem> itemsByRadioId = GetItemsByRadioId();
            foreach (Radio radio in _entityManager.GetComponents<Radio>().Where(radio => !radio.IsDeleted() && !radio.IsVehicleRadio()))
            {
                // A radio whose item is gone, or that lies with an offline owner, rests; its gap recharges it later.
                if (itemsByRadioId.TryGetValue(radio.GetId(), out InventoryItem? inventoryItem) && places.GetPlace(inventoryItem) != BatteryPlace.WithOfflineCharacter)
                {
                    UpdateBattery(radio, places.GetHolder(inventoryItem), now);
                }
            }
        }

        public void ProcessCharging()
        {
            DateTime now = DateTime.UtcNow;
            foreach (Radio radio in _entityManager.GetComponents<Radio>().Where(radio => radio.IsCharging()))
            {
                BatteryCharge charge = radio.GetCharge()!;
                Player? charger = _entityManager.GetComponent<Player>(charge.Charger);
                if (charger == null || !_radioService.IsCarrying(charger, radio))
                {
                    EndCharging(radio, charge.GetLevel(now), now);
                }
                else if (charge.IsDone(now))
                {
                    EndCharging(radio, BatteryLevels.Full, now);
                    charger.SendClientMessage($"{ChatColor.White}[Radio] Your radio is fully charged.");
                }
            }
        }

        public string? BeginRecharge(Player player, Radio radio)
        {
            if (radio.IsVehicleRadio())
                return "A vehicle radio runs on the car.";
            if (radio.IsCharging())
                return "Your radio is already charging.";
            if (BatteryLevels.IsFull(radio.GetBatteryLevel()))
                return "The battery is already full.";

            radio.BeginCharging(new BatteryCharge(radio.GetBatteryLevel(), DateTime.UtcNow, player.Entity));
            return null;
        }

        public double GetCurrentLevel(Radio radio)
        {
            return radio.GetCharge()?.GetLevel(DateTime.UtcNow) ?? radio.GetBatteryLevel();
        }

        public void UseTransmitBattery(Player holder, Radio radio)
        {
            if (radio.IsVehicleRadio() || radio.IsCharging())
            {
                return;
            }

            double oldLevel = radio.GetBatteryLevel();
            double newLevel = BatteryLevels.GetDrained(oldLevel, TransmitCost);
            radio.SetBattery(newLevel, DateTime.UtcNow);
            ReactToLevel(radio, holder, oldLevel, newLevel);
        }

        /// <summary>
        /// A charging radio doesn't drain; the charge sets its level when it ends.
        /// </summary>
        private void UpdateBattery(Radio radio, Player? holder, DateTime now)
        {
            double oldLevel = radio.GetBatteryLevel();
            double newLevel = radio.IsCharging() ? oldLevel : BatteryLevels.GetLevelAfter(oldLevel, radio.GetBatteryUpdatedAt(), now, radio.IsOn(), OnLife);
            radio.SetBattery(newLevel, now);
            ReactToLevel(radio, holder, oldLevel, newLevel);
        }

        private void ReactToLevel(Radio radio, Player? holder, double oldLevel, double newLevel)
        {
            if (!radio.IsOn())
            {
                return;
            }

            if (BatteryLevels.IsEmpty(newLevel))
            {
                Alert(holder, "Their radio crackles and goes dead.", "The battery is empty and the radio switched off. Recharge it in /radio.");
                radio.SetOn(false);
                return;
            }

            double? warningLevel = BatteryLevels.GetCrossedWarningLevel(oldLevel, newLevel);
            if (warningLevel.HasValue)
            {
                Alert(holder, GetWarningText(warningLevel.Value), $"Low battery ({warningLevel}%). Recharge it in /radio.");
            }
        }

        private void Alert(Player? holder, string doText, string holderText)
        {
            if (holder == null || !holder.IsComponentAlive)
            {
                return;
            }

            _chatService.SendPlayerChatMessage(holder, PlayerChatMessageType.DO, doText);
            holder.SendClientMessage($"{ChatColor.White}[Radio] {holderText}");
        }

        private static string GetWarningText(double warningLevel)
        {
            return warningLevel switch
            {
                <= 2 => "Their radio chirps rapidly: battery almost dead.",
                <= 5 => "Their radio chirps twice: battery very low.",
                _ => "Their radio chirps: low battery."
            };
        }

        private static void EndCharging(Radio radio, double level, DateTime now)
        {
            radio.EndCharging();
            radio.SetBattery(level, now);
        }

        private Dictionary<ulong, InventoryItem> GetItemsByRadioId()
        {
            Dictionary<ulong, InventoryItem> itemsByRadioId = new Dictionary<ulong, InventoryItem>();
            foreach (InventoryItem inventoryItem in _entityManager.GetComponents<InventoryItem>().Where(inventoryItem => !inventoryItem.IsDeleted()))
            {
                ulong? radioId = inventoryItem.GetRadioId();
                if (radioId.HasValue)
                {
                    itemsByRadioId[radioId.Value] = inventoryItem;
                }
            }
            return itemsByRadioId;
        }
    }
}
