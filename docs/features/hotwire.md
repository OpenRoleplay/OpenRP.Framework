# Hotwire

Start a vehicle without its key. It takes tools and a few roleplay steps, and a job done without tape can short-circuit later.

## How to use it

| Command | Groups | Does |
|---|---|---|
| `/hotwire` | Default | Hotwires the vehicle you're driving |

You need a **Rusty Paperclip** and a **Worn-Out Screwdriver**. **Electrical Tape** is optional, and makes the job safe. `/testerkit` gives all three.

## How it works

1. **Checks.** You must be the driver, with the engine off.
2. **The job.** It plays out in steps every 2.5 seconds, as `/me` and `/do` messages, and the engine starts.
3. **Without tape,** while the engine runs, every 2.5 seconds there's a 1 in 20 chance the wires short. The engine dies and the vehicle's health drops to 250–390.

**Why:** shortcuts should be risky and doing it right should pay off, so actions feel real instead of instant.

## Known limitations

- **Hard-coded item ids.** The tool ids (33, 34, 35) must match the seeded items.
- **Tools aren't used up.**

## Deep dive

- **Code:** `Features/HotwireFeature/`.
- **Components:**
  - `PlayerHotwiring`, on the player;
  - `HotwiredVehicle`, on the vehicle: whether it was done safely, and its state.
- **System:** `HotwireSystem` runs every 2.5 seconds, and cleans up on `OnVehicleDeath`.

Want more? → [Vehicles](vehicles.md)
