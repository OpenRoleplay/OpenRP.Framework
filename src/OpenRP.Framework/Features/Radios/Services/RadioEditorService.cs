using Microsoft.Extensions.Logging;
using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Factions.Components;
using OpenRP.Framework.Features.Inventories.Components;
using OpenRP.Framework.Features.Inventories.Helpers;
using OpenRP.Framework.Features.Items.Components;
using OpenRP.Framework.Features.Items.Entities;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Features.Radios.Components;
using OpenRP.Framework.Features.Radios.Entities;
using OpenRP.Framework.Features.Radios.Enums;
using OpenRP.Framework.Features.Radios.Helpers;
using OpenRP.Framework.Shared.Batteries.Helpers;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Linq;

namespace OpenRP.Framework.Features.Radios.Services
{
    public class RadioEditorService : IRadioEditorService
    {
        private const int RadioItemId = 50;
        private const int EarpieceItemId = 51;
        private const int FrequencyNameMaxLength = 32;

        private readonly IEntityManager _entityManager;
        private readonly IRadioService _radioService;
        private readonly IRadioFrequencyService _radioFrequencyService;
        private readonly IRadioManager _radioManager;
        private readonly IRadioFrequencyManager _radioFrequencyManager;
        private readonly ILogger<RadioEditorService> _logger;

        public RadioEditorService(IEntityManager entityManager, IRadioService radioService, IRadioFrequencyService radioFrequencyService, IRadioManager radioManager,
            IRadioFrequencyManager radioFrequencyManager, ILogger<RadioEditorService> logger)
        {
            _entityManager = entityManager;
            _radioService = radioService;
            _radioFrequencyService = radioFrequencyService;
            _radioManager = radioManager;
            _radioFrequencyManager = radioFrequencyManager;
            _logger = logger;
        }

        public (Radio? Radio, string? Error) GiveRadio(Player player)
        {
            (Inventory? inventory, Item? radioItem, string? error) = GetInventoryAndItem(player, RadioItemId, "radio");
            if (error != null)
            {
                return (null, error);
            }

            Radio radio = _radioManager.CreateAndLoad(new RadioModel());
            ItemAdditionalData additionalData = ItemAdditionalData.Parse(string.Empty);
            additionalData.SetString(RadioEntities.RadioIdKey, radio.GetId().ToString());
            inventory!.AddItem(radioItem!, 1, additionalData);
            return (radio, null);
        }

        public string? GiveEarpiece(Player player)
        {
            (Inventory? inventory, Item? earpieceItem, string? error) = GetInventoryAndItem(player, EarpieceItemId, "earpiece");
            if (error != null)
            {
                return error;
            }

            inventory!.AddItem(earpieceItem!, 1, ItemAdditionalData.Parse(string.Empty));
            return null;
        }

        public string? SetPower(Radio radio, bool isOn)
        {
            if (isOn && !radio.IsVehicleRadio() && BatteryLevels.IsEmpty(radio.GetBatteryLevel()) && !radio.IsCharging())
            {
                return "The battery is dead. Recharge it first.";
            }

            radio.SetOn(isOn);
            return null;
        }

        public string? SetFrequency(Radio radio, string frequencyText)
        {
            uint? frequency = RadioFrequencyNumbers.Parse(frequencyText);
            if (frequency == null)
            {
                return $"A frequency is a whole number from {RadioFrequencyNumbers.Lowest} to {RadioFrequencyNumbers.Highest}.";
            }

            radio.SetFrequency(frequency.Value);
            return null;
        }

        public void SetUsesEarpiece(Radio radio, bool usesEarpiece)
        {
            radio.SetUsesEarpiece(usesEarpiece);
        }

        public (RadioFrequency? Frequency, string? Error) CreateFrequency(string numberText, string name, RadioFrequencyMode mode)
        {
            uint? number = RadioFrequencyNumbers.Parse(numberText);
            string? error = (number == null ? $"A frequency is a whole number from {RadioFrequencyNumbers.Lowest} to {RadioFrequencyNumbers.Highest}." : null)
                ?? (_radioFrequencyService.GetFrequency(number!.Value) != null ? $"Frequency {number} already has its own rules." : null)
                ?? GetNameError(name);
            if (error != null)
            {
                return (null, error);
            }

            RadioFrequency? frequency = TryCreateFrequency(number!.Value, name.Trim(), mode);
            return frequency == null ? (null, "The frequency couldn't be created. Another server may have just created it.") : (frequency, null);
        }

        public RadioFrequency? CreateFrequencyIfNotExists(uint number, string name, RadioFrequencyMode mode)
        {
            return _radioFrequencyService.GetFrequency(number) ?? TryCreateFrequency(number, name, mode);
        }

        public string? RenameFrequency(RadioFrequency frequency, string name)
        {
            string? error = GetNameError(name);
            if (error != null)
            {
                return error;
            }

            frequency.SetName(name.Trim());
            return null;
        }

        public void SetFrequencyMode(RadioFrequency frequency, RadioFrequencyMode mode)
        {
            frequency.SetMode(mode);
        }

        public void DeleteFrequency(RadioFrequency frequency)
        {
            foreach (RadioFrequencyFaction frequencyFaction in _radioFrequencyService.GetFrequencyFactions(frequency))
            {
                frequencyFaction.ProcessDeletion();
            }
            frequency.ProcessDeletion();
        }

        public string? AddFrequencyFaction(RadioFrequency frequency, Faction faction)
        {
            if (_radioFrequencyService.GetFrequencyFactions(frequency).Any(frequencyFaction => frequencyFaction.GetFactionId() == faction.GetId()))
            {
                return $"{faction.GetName()} is on this frequency already.";
            }

            // Unsaved until the next sync; the manager then gives it its id.
            _entityManager.AddComponent<RadioFrequencyFaction>(EntityId.NewEntityId(), new RadioFrequencyFactionModel { RadioFrequencyId = frequency.GetId(), FactionId = faction.GetId() });
            return null;
        }

        public void RemoveFrequencyFaction(RadioFrequencyFaction frequencyFaction)
        {
            frequencyFaction.ProcessDeletion();
        }

        public string? InstallVehicleRadio(ulong vehicleId)
        {
            if (_radioService.GetVehicleRadio(vehicleId) != null)
            {
                return "This vehicle has a radio already.";
            }

            try
            {
                _radioManager.CreateAndLoad(new RadioModel { VehicleId = vehicleId });
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Installing a radio in vehicle {VehicleId} failed.", vehicleId);
                return "The radio couldn't be installed. Another server may have just installed one.";
            }
        }

        public void RemoveVehicleRadio(ulong vehicleId)
        {
            _radioService.GetVehicleRadio(vehicleId)?.ProcessDeletion();
        }

        /// <summary>
        /// Inserts the frequency right away; the unique number catches one another server created in the meantime.
        /// </summary>
        private RadioFrequency? TryCreateFrequency(uint number, string name, RadioFrequencyMode mode)
        {
            try
            {
                return _radioFrequencyManager.CreateAndLoad(new RadioFrequencyModel { Number = number, Name = name, Mode = mode });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Creating radio frequency {Number} failed.", number);
                return null;
            }
        }

        private (Inventory? Inventory, Item? Item, string? Error) GetInventoryAndItem(Player player, int itemId, string itemName)
        {
            Inventory? inventory = player.IsPlayerPlayingAsCharacter() ? player.GetPlayerCurrentlyPlayingAsCharacter().GetInventory() : null;
            Item? item = _entityManager.GetComponent<Item>(ItemEntities.GetItemId(itemId));
            if (inventory == null || item == null)
            {
                return (null, null, $"That player isn't playing a character with an inventory, or the {itemName} item doesn't exist.");
            }
            if (!inventory.DoesItemFit(item, 1))
            {
                return (null, null, $"The {itemName} doesn't fit in their inventory.");
            }
            return (inventory, item, null);
        }

        private static string? GetNameError(string name)
        {
            string trimmedName = name.Trim();
            if (trimmedName.Length == 0)
                return "The name can't be empty.";
            if (trimmedName.Length > FrequencyNameMaxLength)
                return $"A name can be at most {FrequencyNameMaxLength} characters long.";
            return null;
        }
    }
}
