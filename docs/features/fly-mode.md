# Fly mode

`/fly` (Admin) turns you into a free-flying camera. Use it again to land where you started, with the same health, armour, skin and weapons.

## How to use it

| Command | Groups | Does |
|---|---|---|
| `/fly` | Admin | Starts fly mode, or lands if you're already flying |

1. Type `/fly`. You need to be playing a character, and you can't start from inside a vehicle.
2. Hold W, A, S or D to fly where you look. Look up to climb, and look down to descend.
3. Hold Sprint to go faster, or Walk to go slower.
4. Type `/fly` again to land.

The [map editor](maps/editor.md) has a fly mode switch too. Its **Teleport to map** lands you at the map instead.

## How it works

1. **An invisible camera.** Your camera attaches to an invisible object. Your keys move that object, and the camera follows it.
   - **Why:** a camera can only be attached to an object, so an invisible one carries it.
2. **Speed builds up.** Speed rises while you hold a direction, and reaches full speed after about 3 seconds. Letting go of all keys resets it. Sprint doubles the speed, and Walk cuts it to a fifth.
3. **Landing puts you back.** Taking off saves your position, facing, interior, virtual world, health, armour, skin and weapons with their ammo. Landing spawns you where you started, and the saved state is put back at that spawn. A teleport to a map from the editor lands you at the map instead, and the saved state is still put back there.
   - **Why:** leaving the flying camera spawns the player again, which resets their state. So the state is restored after the spawn.

## Known limitations

- **No collisions.** The camera goes through walls, the ground and objects.
- **Same virtual world.** The camera stays in the virtual world you took off in, so it only shows that world's objects. Flying out of a property interior shows only objects of that world.

## Deep dive

- **Code:** `Features/FlyMode/`.
- **Components:**
  - `FlyMode`: on a flying player. Holds the camera object, the movement state and the saved state. Destroying it destroys the object.
  - `FlyModeRestore`: on a player who just landed, until they spawn.
- **Services:**
  - `IFlyModeService`: start, stop, stop at a position (`StopFlyingAt`, used by the map teleport) and toggle.
  - `IFlyModeMovementService`: reads the keys on every player update and moves the camera object. It converts the up/down and left/right axes back to signed, because SampSharp reads them as unsigned.
  - `IFlyModeRestoreService`: puts the saved state back on spawn.
- **Systems:** `FlyModeSystem` handles `OnPlayerUpdate` and `OnPlayerSpawn`.
- **Command:** `FlyCommand`.
- **Helpers:** `FlySpeeds` (speed and sprint or walk) and `FlyDirections` (the point the camera flies towards).

Want more? → [The map editor](maps/editor.md) · [Admins](admins.md)
