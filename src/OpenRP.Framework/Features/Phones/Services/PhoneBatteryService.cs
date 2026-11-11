using OpenRP.Framework.Features.Inventories.Components;
using OpenRP.Framework.Features.Phones.Components;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Batteries.Entities;
using OpenRP.Framework.Shared.Batteries.Enums;
using OpenRP.Framework.Shared.Batteries.Helpers;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Phones.Services
{
    public class PhoneBatteryService : IPhoneBatteryService
    {
        // A 1992 MicroTAC-class phone: about 10 hours on standby, about 75 minutes of talk.
        private static readonly TimeSpan StandbyLife = TimeSpan.FromHours(10);
        private static readonly TimeSpan TalkLife = TimeSpan.FromMinutes(75);

        private readonly IEntityManager _entityManager;
        private readonly IPhoneService _phoneService;
        private readonly IPhoneCallService _phoneCallService;
        private readonly IPhoneAlertService _phoneAlertService;

        public PhoneBatteryService(IEntityManager entityManager, IPhoneService phoneService, IPhoneCallService phoneCallService, IPhoneAlertService phoneAlertService)
        {
            _entityManager = entityManager;
            _phoneService = phoneService;
            _phoneCallService = phoneCallService;
            _phoneAlertService = phoneAlertService;
        }

        public void UpdateBatteries(BatteryPlaces places)
        {
            DateTime now = DateTime.UtcNow;
            Dictionary<ulong, InventoryItem> itemsByPhoneId = GetItemsByPhoneId();
            foreach (Phone phone in _entityManager.GetComponents<Phone>().Where(phone => !phone.IsDeleted()))
            {
                // A phone whose item is gone, or that lies with an offline owner, rests; its gap recharges it later.
                if (itemsByPhoneId.TryGetValue(phone.GetId(), out InventoryItem? inventoryItem) && places.GetPlace(inventoryItem) != BatteryPlace.WithOfflineCharacter)
                {
                    UpdateBattery(phone, places.GetHolder(inventoryItem), now);
                }
            }
        }

        public void ProcessCharging()
        {
            DateTime now = DateTime.UtcNow;
            foreach (Phone phone in _entityManager.GetComponents<Phone>().Where(phone => phone.IsCharging()))
            {
                BatteryCharge charge = phone.GetCharge()!;
                Player? charger = _entityManager.GetComponent<Player>(charge.Charger);
                if (charger == null || !_phoneService.IsCarrying(charger, phone))
                {
                    EndCharging(phone, charge.GetLevel(now), now);
                }
                else if (charge.IsDone(now))
                {
                    EndCharging(phone, BatteryLevels.Full, now);
                    charger.SendClientMessage($"{ChatColor.White}[Phone] Your phone is fully charged.");
                }
            }
        }

        public string? BeginRecharge(Player player, Phone phone)
        {
            if (phone.IsCharging())
                return "Your phone is already charging.";
            if (BatteryLevels.IsFull(phone.GetBatteryLevel()))
                return "The battery is already full.";

            phone.BeginCharging(new BatteryCharge(phone.GetBatteryLevel(), DateTime.UtcNow, player.Entity));
            return null;
        }

        public double GetCurrentLevel(Phone phone)
        {
            return phone.GetCharge()?.GetLevel(DateTime.UtcNow) ?? phone.GetBatteryLevel();
        }

        /// <summary>
        /// A charging phone doesn't drain; the charge sets its level when it ends.
        /// </summary>
        private void UpdateBattery(Phone phone, Player? holder, DateTime now)
        {
            double oldLevel = phone.GetBatteryLevel();
            double newLevel = phone.IsCharging() ? oldLevel : BatteryLevels.GetLevelAfter(oldLevel, phone.GetBatteryUpdatedAt(), now, phone.IsOn(), GetBatteryLife(phone));
            phone.SetBattery(newLevel, now);
            ReactToLevel(phone, holder, oldLevel, newLevel);
        }

        private void ReactToLevel(Phone phone, Player? holder, double oldLevel, double newLevel)
        {
            if (!phone.IsOn())
            {
                return;
            }

            if (BatteryLevels.IsEmpty(newLevel))
            {
                if (holder != null)
                {
                    _phoneAlertService.AnnounceEmptyBattery(phone, holder);
                }
                // A call on it is cut off within a second by the call timer.
                phone.SetOn(false);
                return;
            }

            double? warningLevel = BatteryLevels.GetCrossedWarningLevel(oldLevel, newLevel);
            if (warningLevel.HasValue && holder != null)
            {
                _phoneAlertService.WarnLowBattery(phone, holder, warningLevel.Value);
            }
        }

        private static void EndCharging(Phone phone, double level, DateTime now)
        {
            phone.EndCharging();
            phone.SetBattery(level, now);
        }

        private TimeSpan GetBatteryLife(Phone phone)
        {
            return _phoneCallService.IsPhoneInCall(phone) ? TalkLife : StandbyLife;
        }

        private Dictionary<ulong, InventoryItem> GetItemsByPhoneId()
        {
            Dictionary<ulong, InventoryItem> itemsByPhoneId = new Dictionary<ulong, InventoryItem>();
            foreach (InventoryItem inventoryItem in _entityManager.GetComponents<InventoryItem>().Where(inventoryItem => !inventoryItem.IsDeleted()))
            {
                ulong? phoneId = inventoryItem.GetPhoneId();
                if (phoneId.HasValue)
                {
                    itemsByPhoneId[phoneId.Value] = inventoryItem;
                }
            }
            return itemsByPhoneId;
        }
    }
}
