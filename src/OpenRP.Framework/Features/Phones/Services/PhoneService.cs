using OpenRP.Framework.Features.Inventories.Components;
using OpenRP.Framework.Features.Phones.Components;
using OpenRP.Framework.Features.Phones.Entities;
using OpenRP.Framework.Features.Phones.Enums;
using OpenRP.Framework.Features.Phones.Helpers;
using OpenRP.Framework.Features.Players.Extensions;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Phones.Services
{
    public class PhoneService : IPhoneService
    {
        private readonly IEntityManager _entityManager;
        private readonly Dictionary<uint, IServerPhoneNumber> _serverNumbersByNumber;

        public PhoneService(IEntityManager entityManager, IEnumerable<IServerPhoneNumber> serverNumbers)
        {
            _entityManager = entityManager;
            _serverNumbersByNumber = serverNumbers.ToDictionary(serverNumber => serverNumber.GetNumber());
        }

        public Phone? GetPhone(ulong phoneId)
        {
            Phone? phone = _entityManager.GetComponent<Phone>(PhoneEntities.GetPhoneId((int)phoneId));
            return phone == null || phone.IsDeleted() ? null : phone;
        }

        public Phone? GetPhoneByNumber(uint number)
        {
            return _entityManager.GetComponents<Phone>().FirstOrDefault(phone => !phone.IsDeleted() && phone.GetNumber() == number);
        }

        public bool IsNumberTaken(uint number)
        {
            return GetPhoneByNumber(number) != null;
        }

        public IServerPhoneNumber? GetServerNumber(uint number)
        {
            return _serverNumbersByNumber.GetValueOrDefault(number);
        }

        public Player? GetCarrier(Phone phone)
        {
            return _entityManager.GetComponents<Player>().FirstOrDefault(player => IsCarrying(player, phone));
        }

        public List<Phone> GetCarriedPhones(Player player)
        {
            return GetCarriedPhoneItems(player)
                .Select(inventoryItem => inventoryItem.GetPhoneId())
                .OfType<ulong>()
                .Select(GetPhone)
                .OfType<Phone>()
                .ToList();
        }

        public bool IsCarrying(Player player, Phone phone)
        {
            return GetCarriedPhoneItems(player).Any(inventoryItem => inventoryItem.GetPhoneId() == phone.GetId());
        }

        public Phone? GetPhoneToUse(Player player)
        {
            List<Phone> phones = GetCarriedPhones(player);
            return phones.FirstOrDefault(phone => phone.IsOn()) ?? phones.FirstOrDefault();
        }

        public string GetNumberLabel(Phone phone, uint number)
        {
            PhoneContact? contact = GetContacts(phone).FirstOrDefault(contact => contact.GetNumber() == number);
            if (contact != null)
            {
                return contact.GetName();
            }
            return GetServerNumber(number)?.GetName() ?? PhoneNumbers.Format(number);
        }

        public List<PhoneContact> GetContacts(Phone phone)
        {
            return _entityManager.GetComponents<PhoneContact>()
                .Where(contact => !contact.IsDeleted() && contact.GetPhoneId() == phone.GetId())
                .OrderBy(contact => contact.GetName(), StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public List<PhoneMessage> GetMessages(Phone phone)
        {
            return _entityManager.GetComponents<PhoneMessage>()
                .Where(message => !message.IsDeleted() && message.GetPhoneId() == phone.GetId())
                .OrderByDescending(message => message.GetSentAt())
                .ToList();
        }

        public int GetUnreadMessageCount(Phone phone)
        {
            return GetMessages(phone).Count(message => message.IsIncoming() && !message.IsRead());
        }

        public List<PhoneCallLog> GetCallLogs(Phone phone)
        {
            return _entityManager.GetComponents<PhoneCallLog>()
                .Where(callLog => !callLog.IsDeleted() && callLog.GetPhoneId() == phone.GetId())
                .OrderByDescending(callLog => callLog.GetStartedAt())
                .ToList();
        }

        public int GetNewMissedCallCount(Phone phone)
        {
            return GetCallLogs(phone).Count(callLog => callLog.GetCallType() == PhoneCallLogType.Missed && !callLog.IsSeen());
        }

        public uint? GetLastDialedNumber(Phone phone)
        {
            return GetCallLogs(phone).FirstOrDefault(callLog => callLog.GetCallType() == PhoneCallLogType.Outgoing)?.GetOtherNumber();
        }

        public string? GetNewsSummary(Phone phone)
        {
            List<string> parts = new List<string>();
            int missedCalls = GetNewMissedCallCount(phone);
            int unreadMessages = GetUnreadMessageCount(phone);
            if (missedCalls > 0)
            {
                parts.Add(missedCalls == 1 ? "1 missed call" : $"{missedCalls} missed calls");
            }
            if (unreadMessages > 0)
            {
                parts.Add(unreadMessages == 1 ? "1 new message" : $"{unreadMessages} new messages");
            }
            return parts.Count == 0 ? null : string.Join(", ", parts);
        }

        /// <summary>
        /// The phone items directly in the inventory of the character the player plays.
        /// </summary>
        private static IEnumerable<InventoryItem> GetCarriedPhoneItems(Player player)
        {
            if (!player.IsComponentAlive || !player.IsPlayerPlayingAsCharacter())
            {
                return Enumerable.Empty<InventoryItem>();
            }

            Inventory? inventory = player.GetPlayerCurrentlyPlayingAsCharacter().GetInventory();
            return inventory?.GetInventoryItems().Where(inventoryItem => !inventoryItem.IsDeleted() && inventoryItem.GetItem().IsItemPhone())
                ?? Enumerable.Empty<InventoryItem>();
        }
    }
}
