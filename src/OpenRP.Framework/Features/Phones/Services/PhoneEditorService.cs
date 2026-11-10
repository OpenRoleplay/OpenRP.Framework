using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Inventories.Components;
using OpenRP.Framework.Features.Inventories.Helpers;
using OpenRP.Framework.Features.Items.Components;
using OpenRP.Framework.Features.Items.Entities;
using OpenRP.Framework.Features.Phones.Components;
using OpenRP.Framework.Features.Phones.Enums;
using OpenRP.Framework.Features.Phones.Helpers;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Shared.BaseManager.Entities;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Phones.Services
{
    public class PhoneEditorService : IPhoneEditorService
    {
        private const int PhoneItemId = 49;
        private const int MaxContacts = 50;
        // 1992 phones remembered only a handful of texts and calls; the oldest makes room for the newest.
        private const int MaxMessages = 20;
        private const int MaxCallLogs = 20;
        private const int ContactNameMaxLength = 24;
        private const int MessageMaxLength = 160;
        private const int NumberAttempts = 100;

        private readonly IEntityManager _entityManager;
        private readonly IPhoneService _phoneService;
        private readonly IPhoneManager _phoneManager;
        private readonly IPhoneAlertService _phoneAlertService;
        private readonly ILogger<PhoneEditorService> _logger;

        public PhoneEditorService(IEntityManager entityManager, IPhoneService phoneService, IPhoneManager phoneManager,
            IPhoneAlertService phoneAlertService, ILogger<PhoneEditorService> logger)
        {
            _entityManager = entityManager;
            _phoneService = phoneService;
            _phoneManager = phoneManager;
            _phoneAlertService = phoneAlertService;
            _logger = logger;
        }

        public (Phone? Phone, string? Error) GivePhone(Player player)
        {
            Inventory? inventory = player.IsPlayerPlayingAsCharacter() ? player.GetPlayerCurrentlyPlayingAsCharacter().GetInventory() : null;
            Item? phoneItem = _entityManager.GetComponent<Item>(ItemEntities.GetItemId(PhoneItemId));
            if (inventory == null || phoneItem == null)
            {
                return (null, "That player isn't playing a character with an inventory, or the phone item doesn't exist.");
            }
            if (!inventory.DoesItemFit(phoneItem, 1))
            {
                return (null, "The phone doesn't fit in their inventory.");
            }

            Phone? phone = TryCreatePhone();
            if (phone == null)
            {
                return (null, "The phone couldn't be created. Try again.");
            }

            ItemAdditionalData additionalData = ItemAdditionalData.Parse(string.Empty);
            additionalData.SetString("PHONE_ID", phone.GetId().ToString());
            inventory.AddItem(phoneItem, 1, additionalData);
            return (phone, null);
        }

        public void SetPower(Phone phone, bool isOn)
        {
            phone.SetOn(isOn);
            PhoneMessage? newestUnread = _phoneService.GetMessages(phone).FirstOrDefault(message => message.IsIncoming() && !message.IsRead());
            if (isOn && newestUnread != null)
            {
                _phoneAlertService.BeepForMessage(phone, newestUnread.GetOtherNumber());
            }
        }

        public void SetSilent(Phone phone, bool isSilent)
        {
            phone.SetSilent(isSilent);
        }

        public string? AddContact(Phone phone, string name, string numberText)
        {
            uint? number = PhoneNumbers.Parse(numberText);
            string? error = GetContactNameError(name)
                ?? (number == null ? "That isn't a phone number." : null)
                ?? (_phoneService.GetContacts(phone).Count >= MaxContacts ? $"The phone book is full ({MaxContacts} names)." : null)
                ?? (_phoneService.GetContacts(phone).Any(contact => contact.GetNumber() == number) ? "That number is in the phone book already." : null);
            if (error != null)
            {
                return error;
            }

            CreateRow<PhoneContact>(new PhoneContactModel { PhoneId = phone.GetId(), Name = name.Trim(), Number = number!.Value });
            return null;
        }

        public string? RenameContact(PhoneContact contact, string name)
        {
            string? error = GetContactNameError(name);
            if (error != null)
            {
                return error;
            }

            contact.SetName(name.Trim());
            return null;
        }

        public void DeleteContact(PhoneContact contact)
        {
            contact.ProcessDeletion();
        }

        public string? SendText(Phone phone, uint number, string text)
        {
            string trimmedText = text.Trim();
            string? error = GetSendTextError(phone, number, trimmedText);
            if (error != null)
            {
                return error;
            }

            Phone? recipient = _phoneService.GetPhoneByNumber(number);
            if (recipient == null)
            {
                return $"The message to {PhoneNumbers.Format(number)} couldn't be delivered. That number isn't in use.";
            }

            DateTime sentAt = DateTime.UtcNow;
            AddMessage(phone, number, trimmedText, false, sentAt);
            AddMessage(recipient, phone.GetNumber(), trimmedText, true, sentAt);
            _phoneAlertService.BeepForMessage(recipient, phone.GetNumber());
            return null;
        }

        public void SetMessageRead(PhoneMessage message)
        {
            if (!message.IsRead())
            {
                message.SetRead();
            }
        }

        public void DeleteMessage(PhoneMessage message)
        {
            message.ProcessDeletion();
        }

        public void AddCallLog(Phone phone, uint otherNumber, PhoneCallLogType type, DateTime startedAt, int durationSeconds)
        {
            CreateRow<PhoneCallLog>(new PhoneCallLogModel
            {
                PhoneId = phone.GetId(),
                OtherNumber = otherNumber,
                Type = type,
                StartedAt = startedAt,
                DurationSeconds = durationSeconds,
                // Only missed calls are news.
                IsSeen = type != PhoneCallLogType.Missed
            });
            DeleteOldest(_phoneService.GetCallLogs(phone), MaxCallLogs);
        }

        public void SetMissedCallsSeen(Phone phone)
        {
            foreach (PhoneCallLog callLog in _phoneService.GetCallLogs(phone).Where(callLog => !callLog.IsSeen()))
            {
                callLog.SetSeen();
            }
        }

        private void AddMessage(Phone phone, uint otherNumber, string text, bool isIncoming, DateTime sentAt)
        {
            CreateRow<PhoneMessage>(new PhoneMessageModel
            {
                PhoneId = phone.GetId(),
                OtherNumber = otherNumber,
                Text = text,
                IsIncoming = isIncoming,
                IsRead = !isIncoming,
                SentAt = sentAt
            });
            DeleteOldest(_phoneService.GetMessages(phone), MaxMessages);
        }

        /// <summary>
        /// The list is newest first; everything past the limit goes.
        /// </summary>
        private static void DeleteOldest<T>(List<T> newestFirst, int limit) where T : IDeletable
        {
            foreach (T row in newestFirst.Skip(limit))
            {
                row.ProcessDeletion();
            }
        }

        /// <summary>
        /// Unsaved until the next sync; the manager then gives it its id.
        /// </summary>
        private void CreateRow<T>(object model) where T : Component
        {
            _entityManager.AddComponent<T>(EntityId.NewEntityId(), model);
        }

        /// <summary>
        /// Inserts a phone with a random free number. The loaded phones are checked first; the unique index catches a
        /// number another server took in the meantime.
        /// </summary>
        private Phone? TryCreatePhone()
        {
            for (int attempt = 0; attempt < NumberAttempts; attempt++)
            {
                uint number = (uint)Random.Shared.Next((int)PhoneNumbers.LowestPlayerNumber, (int)PhoneNumbers.HighestPlayerNumber + 1);
                if (_phoneService.IsNumberTaken(number))
                {
                    continue;
                }

                try
                {
                    return _phoneManager.CreateAndLoad(new PhoneModel { Number = number });
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Creating a phone with number {Number} failed; trying another number.", number);
                }
            }
            return null;
        }

        private string? GetSendTextError(Phone phone, uint number, string text)
        {
            if (!phone.IsOn())
                return "Your phone is turned off.";
            if (text.Length == 0)
                return "The message is empty.";
            if (text.Length > MessageMaxLength)
                return $"A message can be at most {MessageMaxLength} characters long.";
            if (PhoneNumbers.IsServerNumber(number))
                return $"Messages to {PhoneNumbers.Format(number)} can't be delivered. Call it instead.";
            if (number == phone.GetNumber())
                return "You can't text your own number.";
            return null;
        }

        private static string? GetContactNameError(string name)
        {
            string trimmedName = name.Trim();
            if (trimmedName.Length == 0)
                return "The name can't be empty.";
            if (trimmedName.Length > ContactNameMaxLength)
                return $"A name can be at most {ContactNameMaxLength} characters long.";
            return null;
        }
    }
}
