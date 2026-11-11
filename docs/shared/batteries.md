# Batteries

The battery rules that [phones](../features/phones.md) and handheld [radios](../features/radios.md) share. The level is a percentage (0 to 100) in real time. Each feature stores the level on its own row and runs its own timers. This module holds the shared math and works out where each device is.

## How it works

1. **Where the device is** decides what happens, every 30 seconds:

   | Where | Battery |
   |---|---|
   | In an online character's inventory (any depth, wallets too) | Drains when on. Never recharges by itself |
   | Dropped on the ground, or in any other inventory | Drains when on. Never recharges by itself |
   | In an offline character's inventory (any depth) | Rests. Recharges once its owner has been away for an hour |

2. **The "last updated" time** of every device that isn't with an offline character is refreshed each update, whether it's on or off. So a device never builds up the gap that recharges it while you play, even when it's switched off.
3. **Coming back:** when a device comes back into play (its owner logs in), the time since its last update decides:
   - **Under 2 minutes:** it was in play, so it drains if it's on;
   - **2 minutes to 1 hour:** nothing changes;
   - **1 hour or more:** `(gap - 1 h) / 24 h × 100%` is added, up to full. So 0% to full takes 25 hours away.
4. **Warnings** go out when the level drops past 10%, 5% and 2%. At 0% the device switches itself off.
5. **Recharge** (from the device's menu) takes up to 60 seconds from empty, and proportionally less when partly charged. The device stays usable meanwhile. It stops early, keeping the level reached so far, when the device leaves the player who started it.

**Why only offline recharging:** there are no chargers in the world yet. Recharging overnight while you're away matches how people charged a 1992 phone, without making a switched-off phone a free charger while you play.

## Known limitations

- **A restart ends a recharge,** at the level of the last battery update.
- **Server downtime of an hour or more** counts as time away for every device, not only for offline characters' devices.

## Deep dive

- **Code:** `Shared/Batteries/`.
- **`BatteryLevels`:** the constants and pure math: `GetLevelAfter`, `GetChargeDuration`, `GetCrossedWarningLevel` and `Format`.
- **`BatteryCharge`:** a recharge in progress, held on the device component. It's never saved.
- **`IBatteryPlaceService.TakeSnapshot(dataContext)`:** returns `BatteryPlaces`, which answers `GetPlace(inventoryItem)` and `GetHolder(inventoryItem)`.
  - **Offline characters' inventories:** offline characters have no components, so their inventory ids come from one small database query per update.
  - **Wallets:** walked up to the outermost inventory.
- **Used by:** `PhoneBatteryService` and `RadioBatteryService`.

Want more? → [Phones](../features/phones.md) · [Radios](../features/radios.md)
