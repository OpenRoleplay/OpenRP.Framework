using OpenRP.Framework.Features.DroppedItems.Components;
using OpenRP.Framework.Features.Inventories.Components;
using OpenRP.Framework.Features.Items.Components;
using OpenRP.Framework.Features.Items.Entities;
using OpenRP.Framework.Features.Players.Extensions;
using OpenRP.Framework.Features.Radios.Components;
using OpenRP.Framework.Features.Radios.Entities;
using OpenRP.Framework.Features.Vehicles.Components;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System.Collections.Generic;
using System.Linq;

namespace OpenRP.Framework.Features.Radios.Services
{
    public class RadioService : IRadioService
    {
        private readonly IEntityManager _entityManager;

        public RadioService(IEntityManager entityManager)
        {
            _entityManager = entityManager;
        }

        public Radio? GetRadio(ulong radioId)
        {
            Radio? radio = _entityManager.GetComponent<Radio>(RadioEntities.GetRadioId((int)radioId));
            return radio == null || radio.IsDeleted() ? null : radio;
        }

        public List<Radio> GetCarriedRadios(Player player)
        {
            return GetCarriedItems(player)
                .Where(inventoryItem => inventoryItem.GetItem().IsItemRadio())
                .Select(inventoryItem => inventoryItem.GetRadioId())
                .OfType<ulong>()
                .Select(GetRadio)
                .OfType<Radio>()
                .ToList();
        }

        public bool IsCarrying(Player player, Radio radio)
        {
            return GetCarriedRadios(player).Any(carriedRadio => carriedRadio.GetId() == radio.GetId());
        }

        public bool HasEarpiece(Player player)
        {
            return GetCarriedItems(player).Any(inventoryItem => inventoryItem.GetItem().IsItemRadioEarpiece());
        }

        public Radio? GetVehicleRadio(ulong vehicleId)
        {
            return _entityManager.GetComponents<Radio>().FirstOrDefault(radio => !radio.IsDeleted() && radio.GetVehicleId() == vehicleId);
        }

        public Radio? GetRadioOfVehicleIn(Player player)
        {
            Vehicle? vehicle = player.IsComponentAlive && player.InAnyVehicle ? player.Vehicle : null;
            ulong? vehicleId = vehicle?.GetComponent<VehicleDataComponent>()?.VehicleData?.Id;
            return vehicleId.HasValue ? GetVehicleRadio(vehicleId.Value) : null;
        }

        public List<Radio> GetUsableRadios(Player player)
        {
            List<Radio> radios = GetCarriedRadios(player);
            Radio? vehicleRadio = GetRadioOfVehicleIn(player);
            if (vehicleRadio != null)
            {
                radios.Add(vehicleRadio);
            }
            return radios;
        }

        public Radio? GetRadioToUse(Player player)
        {
            List<Radio> carriedRadios = GetCarriedRadios(player);
            return carriedRadios.FirstOrDefault(radio => radio.IsOn())
                ?? GetRadioOfVehicleIn(player)
                ?? carriedRadios.FirstOrDefault();
        }

        public bool CanUse(Player player, Radio radio)
        {
            return GetUsableRadios(player).Any(usableRadio => usableRadio.GetId() == radio.GetId());
        }

        public List<RadioSpeaker> GetPlayingRadios(uint frequency)
        {
            return GetCarriedSpeakers(frequency)
                .Concat(GetDroppedSpeakers(frequency))
                .Concat(GetVehicleSpeakers(frequency))
                .ToList();
        }

        private IEnumerable<RadioSpeaker> GetCarriedSpeakers(uint frequency)
        {
            foreach (Player player in _entityManager.GetComponents<Player>())
            {
                foreach (Radio radio in GetCarriedRadios(player).Where(radio => IsPlaying(radio, frequency)))
                {
                    yield return new RadioSpeaker
                    {
                        Radio = radio,
                        Position = player.Position,
                        VirtualWorld = player.VirtualWorld,
                        Holders = new List<Player> { player },
                        PlaysAloud = !radio.UsesEarpiece() || !HasEarpiece(player)
                    };
                }
            }
        }

        /// <summary>
        /// A dropped radio has no holder and plays where it lies. Dropped items aren't tied to a virtual world, so
        /// neither is its sound.
        /// </summary>
        private IEnumerable<RadioSpeaker> GetDroppedSpeakers(uint frequency)
        {
            foreach (DroppedItem droppedItem in _entityManager.GetComponents<DroppedItem>())
            {
                Item? item = _entityManager.GetComponent<Item>(ItemEntities.GetItemId((int)droppedItem.GetItemId()));
                ulong? radioId = item != null && item.IsItemRadio() ? droppedItem.GetAdditionalData().GetUlong(RadioEntities.RadioIdKey) : null;
                Radio? radio = radioId.HasValue ? GetRadio(radioId.Value) : null;
                if (radio != null && IsPlaying(radio, frequency))
                {
                    yield return new RadioSpeaker { Radio = radio, Position = droppedItem.GetPosition(), VirtualWorld = null, Holders = new List<Player>(), PlaysAloud = true };
                }
            }
        }

        /// <summary>
        /// A vehicle radio is a fixed speaker: everyone inside holds it, and it plays out loud even when the car is empty.
        /// </summary>
        private IEnumerable<RadioSpeaker> GetVehicleSpeakers(uint frequency)
        {
            Dictionary<ulong, Radio> radiosByVehicleId = _entityManager.GetComponents<Radio>()
                .Where(radio => radio.IsVehicleRadio() && IsPlaying(radio, frequency))
                .ToDictionary(radio => radio.GetVehicleId()!.Value);
            if (radiosByVehicleId.Count == 0)
            {
                yield break;
            }

            foreach (VehicleDataComponent vehicleData in _entityManager.GetComponents<VehicleDataComponent>())
            {
                Vehicle? vehicle = vehicleData.GetComponent<Vehicle>();
                if (vehicle != null && vehicleData.VehicleData != null && radiosByVehicleId.TryGetValue(vehicleData.VehicleData.Id, out Radio? radio))
                {
                    yield return new RadioSpeaker { Radio = radio, Position = vehicle.Position, VirtualWorld = vehicle.VirtualWorld, Holders = GetOccupants(vehicle), PlaysAloud = true };
                }
            }
        }

        private List<Player> GetOccupants(Vehicle vehicle)
        {
            return _entityManager.GetComponents<Player>().Where(player => player.InAnyVehicle && player.Vehicle?.Entity == vehicle.Entity).ToList();
        }

        private static bool IsPlaying(Radio radio, uint frequency)
        {
            return !radio.IsDeleted() && radio.IsOn() && radio.GetFrequency() == frequency;
        }

        /// <summary>
        /// The items directly in the inventory of the character the player plays.
        /// </summary>
        private static IEnumerable<InventoryItem> GetCarriedItems(Player player)
        {
            if (!player.IsComponentAlive || !player.IsPlayerPlayingAsCharacter())
            {
                return Enumerable.Empty<InventoryItem>();
            }

            Inventory? inventory = player.GetPlayerCurrentlyPlayingAsCharacter().GetInventory();
            return inventory?.GetInventoryItems().Where(inventoryItem => !inventoryItem.IsDeleted()) ?? Enumerable.Empty<InventoryItem>();
        }
    }
}
