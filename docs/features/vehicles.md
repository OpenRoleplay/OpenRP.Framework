# Vehicles

Persistent vehicles: saved in the database, spawned at startup, started with a key, locked, and parked to keep their spot. Admins can repair them.

## How to use it

| Command | Groups | Does |
|---|---|---|
| `/engine`, `/e`, `/eon`, `/eoff` | Default | Engine on/off; needs the key |
| `/lights`, `/lon`, `/loff` | Default | Headlights, with a `/me` and a `/do` line |
| `/lock`, `/unlock`, `/locks` | Default | Locks or unlocks the doors; `/locks` toggles. See [Locking](#locking) |
| `/park`, `/savecar` | Default | The vehicle spawns here from now on. See [Parking](#parking) |
| `/repairvehicle`, `/fixveh` | Admin | Repairs the vehicle to full health. See [Repairing](#repairing) |
| `/createvehicle` | Admin | Creates a vehicle in front of you, in the [vehicle editor](vehicles/editor.md) |
| `/editvehicle` | Admin | Edits the vehicle you're in, in the [vehicle editor](vehicles/editor.md) |
| `/deletevehicle [vehicle id]` | Admin | Deletes a saved vehicle for good, after you confirm. Without an id, see [Deleting a vehicle](#deleting-a-vehicle) |

Without a key: [Hotwire](hotwire.md). Turn signals: [Turn signals](vehicles/indicators.md). Temporary vehicles: `/sv` ([Admins](admins.md)).

## Locking

`/lock` and `/unlock` lock or unlock a vehicle. `/locks` locks an unlocked one and unlocks a locked one. Locked doors stop people getting in.

Which vehicle they act on depends on your seat:

| You are | Vehicle |
|---|---|
| In the driver or front passenger seat | The vehicle you sit in |
| In a rear seat | None: "You can't reach the lock button from here." |
| On foot | The nearest vehicle in reach that you have the key for |

- **In reach** means within 5 m of the vehicle's body. The distance is measured to the nearest point of its footprint, the rectangle of its model's size turned to its heading. So a long car can be locked from beside its rear door. A model with no size data is measured from its centre.
- **On foot,** vehicles you have no key for are skipped. If a vehicle is in reach but none you have a key for is, you get "You don't have the key for this vehicle." If none is in reach, you get "Sit in the vehicle, or stand within 5 metres of it."

Who has a key:

| Who | Saved vehicle | Temporary `/sv` vehicle |
|---|---|---|
| Owner or key holder | Yes | No: it has no owner or key |
| Admin | Yes | Yes |
| Anyone else | No | Only from a front seat |

1. **Outside or inside.** From outside, you turn the key in the door. From a front seat, you push the door lock button: down to lock, up to unlock. Each sends a `/me` line, then a `/do` line with the new state.
2. **Saved vehicles keep their lock.** The database is written first, then the game changes, so they always agree. A locked saved vehicle spawns locked.
3. **Temporary vehicles** lock in the game only. The lock is gone once the vehicle is gone.
4. **One change at a time.** While a saved vehicle's lock is being saved, another lock or unlock of it says "Wait a moment." The vehicle editor's Save waits the same way.

Locking a locked vehicle says "The doors are already locked."

**Why:** a 1992 car has no remote, so locking takes a key at the door or the lock button inside.

## Parking

`/park` and `/savecar` set the spot where the vehicle spawns from now on. Only the owner or a key holder can do it, and admins too. You must be in the driver's seat.

| Who | Saved vehicle | Server vehicle |
|---|---|---|
| Owner or key holder | Yes | No |
| Admin (`vehicles.admin`) | Yes | Yes |
| Anyone else | No | No |

Temporary `/sv` vehicles can't be parked.

**Why:** parking moves a car's spawn point for good, so strangers can't do it.

## Repairing

`/repairvehicle` and `/fixveh` (Admin) repair the vehicle you sit in, or else the nearest one in reach (within 5 m of its body, as in [Locking](#locking)). `/repairvehicle <vehicle id>` repairs a saved vehicle by its id, whether or not it's loaded.

- Full health is the vehicle's max health, 1000 by default.
- The new health is saved. A saved vehicle that isn't loaded spawns repaired.
- A temporary `/sv` vehicle is repaired in the game only.
- Repairing a car you're in or next to also sends a `/do` line: "The Sultan looks as good as new."
- Repairing a loaded car by its id sends the same `/do` line at the car's position, named after you, so people near the car see it. An unloaded car gets no `/do` line.

**Why:** the live vehicle data is the one source of truth, so a repair updates both the live data and the database row.

## How it works

1. **At startup,** every saved vehicle spawns with:
   - its model, or a random one of its spawn type;
   - its saved colours, or random default colours when it's a server vehicle (those are never saved);
   - a plate made from its id, starting with the 2-letter tag of the country it's registered in (`SA` when it isn't registered; see [Jurisdictions](jurisdictions.md));
   - its saved state, its locked doors, virtual world and interior.
2. **Keys.** The engine needs a **vehicle key** item for this vehicle in your inventory. It starts about a second after you turn the key, if you're still in the driver's seat. Admins can give themselves a key in the [vehicle editor](vehicles/editor.md#giving-a-key).
3. **Spawned or saved.** Vehicles without saved data are temporary, spawned ones.
4. **The vehicle editor** edits a copy of the vehicle. Save applies the changes to the live vehicle; Cancel discards them. See [the vehicle editor](vehicles/editor.md).
5. **What gets saved.** `/engine` and `/lights` save their own state, `/lock` saves the doors, `/park` saves the position, `/repairvehicle` saves the health, and the editor saves the whole vehicle.
6. **Plates follow jurisdictions.** When a jurisdiction's tags change, it moves to another country, or it's deleted, the plates of loaded vehicles are set again.

## Deleting a vehicle

`/deletevehicle` (Admin) deletes a saved vehicle for good. With an id, it deletes that vehicle. Without one, it deletes the vehicle you sit in, else the nearest saved vehicle within 5 m of its body, in your virtual world and interior. If there's none, it says so. It then asks you to confirm. On Delete it:

- deletes the vehicle's row;
- removes its radio, and the vehicle keys that are loaded in the game;
- ejects the people inside, and destroys the vehicle in the world.

Keys in the inventories of offline characters aren't removed (see the known limitations). A temporary `/spawnveh` vehicle has no row, so the command refuses it.

## Known limitations

- **Owners get no key.** The editor's **Give key** row gives a key only to the admin who uses it. An owner doesn't get one automatically.
- **Keys of offline characters stay.** Deleting a vehicle removes only the keys that are loaded in the game. A key in the inventory of an offline character stays there.
- **Reach uses the model's size.** A model with no size data is measured from its centre, so it's in reach within 5 m of its middle.
- **Not yet checked in game:** window states, and a vehicle's virtual world and interior.

## Deep dive

- **Code:** `Features/Vehicles/`.
- **Commands:** `CreateVehicleCommand`, `EditVehicleCommand`, `DeleteVehicleCommand`, `LockCommand`, `ParkCommand` (with `/savecar`) and `RepairVehicleCommand`. The editor is `Dialogs/CreateOrUpdateVehicleDialog`, with a partial file for each group of rows (such as `.Model`, `.Owner` and `.Save`). `DeleteVehicleDialog` is the delete confirmation.
- **Components:**
  - `VehicleDataComponent`: the saved data of a loaded vehicle;
  - `EngineStarting`: on a vehicle whose key was just turned. `EngineStartSystem` starts the engine when it's due;
  - `CreateOrUpdateVehicleDataComponent`: the editor's state (its copy, the live vehicle, and the data as it was when the editor opened). One is made for each editor session.
- **Services:**
  - `IVehicleManager`: spawning (`LoadAndUnloadVehicles`, `LoadVehicle`, `ReplaceVehicle`), saving and deleting (`SaveVehicle`, `DeleteVehicle`), changing (`UpdateVehicle`, `UpdateChangedSettings`), locks (`SetVehicleLockAsync`, and `IsLockChangeInProgress` while it's being saved), finding a loaded vehicle (`FindLoadedVehicle`), and plates (`GetNumberPlate`, `RefreshNumberPlates`, `RemoveVehiclesFromJurisdiction`);
  - `IVehicleAccessService`: `CreatePermissions` (the admin permission, at startup), `IsAdmin`, `CanControl` (lock and unlock) and `CanPark`;
  - `IVehicleKeyService`: `HasVehicleKey`, `GiveVehicleKey` (gives a key, or returns why it was refused), and `RemoveVehicleKeys` for a deleted vehicle;
  - `IVehicleEngineService`: starting and stopping the engine. It's transient, because it saves through `BaseDataContext`.
- **Admin permission (`vehicles.admin`):**
  - **What it does:** an admin may lock, unlock and park any vehicle, with or without its key. `IVehicleAccessService.IsAdmin` checks it. The Admin group holds it, so its members are admins.
  - **Seeding:** `VehicleSystem.OnGameModeInit` calls `IVehicleAccessService.CreatePermissions` at startup. It creates the permission with `CreatePermissionIfNotExists`, and links it to the Admin group with `AddPermissionToGroupIfMissing`. The faction permissions are seeded the same way.
  - **When it applies:** a character's permissions are loaded when the character is selected. No character is online at startup, so nobody needs a reload. Existing admins get the permission at their next character selection.
  - **Why:** the check used to borrow `cmd.deletevehicle`. A permission of its own keeps admin status apart from that command.
- **Plates:** `VehicleHelper.GenerateCarPlate(id, tag)`, with the tag from `IJurisdictionService.GetPlateTag`.
- **Events:** `VehicleServerSystem` handles `OnJurisdictionsLoaded`, `OnJurisdictionPlateTagChanged` and `OnJurisdictionDeleted`.
- **Tables:** [`Vehicles` and the model tables](../database.md#vehicles-vehiclemodel).

Want more? → [Turn signals](vehicles/indicators.md) · [Vehicle editor](vehicles/editor.md) · [Hotwire](hotwire.md) · [Inventories](inventories.md)
