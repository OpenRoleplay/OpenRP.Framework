# ChickenCoops

Coops where chickens breed and lay eggs that players can collect. You have to crouch while collecting, or you scare the chickens.

## How to use it

| Command | Groups | Does |
|---|---|---|
| `/collectegg` | Default | Collect an egg from the coop you're at (within 3 units) |

Code places a coop with `IChickenCoopService.CreateChickenCoop(position)`, for example on a mapping's coop object.

## How it works

1. **Breeding and laying.** Once an hour, a coop gains a chicken. Every 15 minutes, each chicken may lay an egg.
2. **Collecting.** `/collectegg` takes a few steps, 2.5 seconds each, with messages and sounds.
3. **The result.** If you were crouched, you get a Chicken Egg (item 36). If not, the chickens panic and you get nothing.

**Why crouching:** small mechanics reward doing it right, so actions feel real instead of instant.

## Known limitations

- **Not saved.** Coops start fresh, with no chickens or eggs carried over, after every restart.

## Deep dive

- **Code:** `Features/ChickenCoops/`.
- **Components:**
  - `ChickenCoop`: counts and timers, on a runtime entity;
  - `PlayerCollectingEgg`: on the player, while collecting.
- **Service:** `IChickenCoopService`.
- **System:** `ChickenCoopSystem` advances collections every 500 ms, and updates coops every 60 s.
- **Sounds** come from the [CDN](cdn.md): `sfx/chickenActivity.mp3`, `chickenEggPop.mp3` and `chickenAlarmCall.mp3`.

Want more? → [Inventories](inventories.md)
