# Turn signals

Turn signals are flashing orange lights on the sides of a car. The driver switches them with the look keys or with `/blinker`.

## How to use it

| Input | Does |
|---|---|
| Look-left key | Turns the left signal on. Press it again to turn it off |
| Look-right key | Turns the right signal on. Press it again to turn it off |
| Both look keys pressed together | Turns the hazards on |
| Any look key while the hazards are on | Turns the hazards off |
| `/blinker left`, `/blinker right` | Sets that side on |
| `/blinker hazards` | Turns the hazards on |
| `/blinker off` | Turns the signals off |

`/indicator` does the same as `/blinker`.

- **Driver only.** The keys and the commands work only from the driver's seat.
- **Keys toggle, commands set.** Pressing the other side's key switches to that side. `/blinker left` while the left signal is already on gives "The left turn signal is already on."
- **Both keys must be pressed together.** The hazards need both look keys pressed at the same moment. Holding one key and pressing the other turns on the side of the key you pressed.
- **Key cooldown.** After a look key changes the signals, the look keys do nothing for 500 ms. `/blinker` has no cooldown.
- **A `/me` line** sends for each change, for example "flicks the turn signal lever left." The hazards send "presses the hazard switch."

## Which vehicles

Planes, helicopters, boats, bikes and quads don't have turn signals. Nor does a model with no size data. The full list is in the [deep dive](#deep-dive).

## How it works

1. **Lights.** Two flashing lights go on each lit side, one at the front and one at the back, attached to the car. The hazards light both sides.
2. **No timer.** The lights flash on their own.
3. **Leaving the car.** When the driver leaves the seat or disconnects, the left and right signals go off. The hazards stay on. The next driver switches them off with a look key or with `/blinker off`.
4. **Destroying the vehicle** removes its lights too.
5. **Not saved.** A restart turns them off.

**Why:** a real car's hazards stay on when the driver gets out. The look keys are the ones X-RP uses. There's no sound, because a 1992 stalk just clicks.

**Why:** the cooldown stops a key that bounces, or a quick double tap, from switching the signals twice.

## Known limitations

- **Placement comes from the model's size.** The lights are placed from it, and not every model has been checked yet.

## Deep dive

- **Code:** `Features/Vehicles/`.
- **Command:** `IndicatorCommand` for `/blinker` and `/indicator`. It shows the syntax when no side is given.
- **System:** `VehicleIndicatorSystem`:
  - reads look-key presses (`OnPlayerKeyStateChange`);
  - turns the left and right signals off when the driver leaves the seat (`OnPlayerStateChange`) or disconnects (`OnPlayerDisconnect`), which also removes the player's key cooldown.
- **Service:** `IVehicleIndicatorService` (`VehicleIndicatorService`):
  - `SetIndicator` for `/blinker`, with no cooldown;
  - `SetIndicatorFromKeyPress` for the look keys, which checks the cooldown and skips presses that wouldn't change the signals;
  - `TurnOffTurnSignalsDrivenBy`, which keeps the hazards;
  - `HasTurnSignals`, `GetIndicatorState` and `RemoveKeyCooldown`.
- **Components:**
  - `VehicleIndicators` on the vehicle: the state, the driver who switched it on, and the blinker objects it put on the car. Removing it removes the lights. The driver can be stale once the hazards stay on, and the next driver's change overwrites it.
  - `IndicatorKeyCooldown` on the player: when their last look-key change started. It lasts `IndicatorRules.KeyCooldownInMilliseconds`, which is 500.
- **Rules:** `IndicatorRules` maps key presses and command words to states, and holds the cooldown time and the messages. `IndicatorState` is the enum: Off, Left, Right and Hazards. For a look-key press, the new state is:

  | Now | Left key | Right key | Both keys |
  |---|---|---|---|
  | Off | Left | Right | Hazards |
  | Left | Off | Right | Hazards |
  | Right | Left | Off | Hazards |
  | Hazards | Off | Off | Off |

  A key counts only when it was pressed in that change. A key that was already held counts for nothing.
- **Lights:** `DynamicObject`s of model 19294, attached to the vehicle with the streamer. Each sits at the vehicle's size divided by 2.23 on x and y, and 0.1 above the origin on z. They are children of the vehicle's entity, and `VehicleIndicators` keeps the list of them.
- **Models without turn signals** (`TurnSignalVehicles`):
  - Aircraft: Andromada, AT-400, Beagle, Cargobob, Cropduster, Dodo, Hunter, Hydra, Leviathan, Maverick, Nevada, Police Maverick, Raindance, Rustler, SAN News Maverick, Seasparrow, Shamal, Skimmer, Sparrow, Stuntplane.
  - Boats: Coastguard, Dinghy, Jetmax, Launch, Marquis, Predator, Reefer, Speeder, Squallo, Tropic, and Vortex (a hovercraft).
  - Bikes and quads: BF-400, Bike, BMX, Faggio, FCR-900, Freeway, HPV1000, Mountain Bike, NRG-500, PCJ-600, Pizzaboy, Quad, Sanchez, Wayfarer.

Want more? → [Vehicles](../vehicles.md) · [Commands](../../commands.md)
