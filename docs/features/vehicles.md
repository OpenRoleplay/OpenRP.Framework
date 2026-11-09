# Vehicles

Persistent vehicles: saved in the database, spawned at startup, started with a key, and parked to keep their spot.

## How to use it

| Command | Groups | Does |
|---|---|---|
| `/engine`, `/e`, `/eon`, `/eoff` | Default | Engine on/off; needs the key |
| `/lights`, `/lon`, `/loff` | Default | Headlights |
| `/park`, `/savecar` | Default | The vehicle spawns here from now on |
| `/createvehicle` | Admin | Creates a vehicle in the vehicle editor |
| `/editvehicle` | Admin | Edits the vehicle you're in |

Without a key: [Hotwire](hotwire.md). Temporary vehicles: `/sv` ([Admins](admins.md)).

## How it works

1. **At startup,** every saved vehicle spawns with:
   - its model, or a random one of its spawn type;
   - its colours, or random default colours;
   - a plate made from its id, starting with the 2-letter tag of the country it's registered in (`SA` when it isn't registered; see [Jurisdictions](jurisdictions.md));
   - its saved state.
2. **Keys.** The engine needs a **vehicle key** item for this vehicle in your inventory. It starts about a second after you turn the key, if you're still in the driver's seat.
3. **Spawned or saved.** Vehicles without saved data are temporary, spawned ones.
4. **The vehicle editor** (`/createvehicle`, `/editvehicle`) edits every setting and saves at once. "Registered in" picks the vehicle's jurisdiction.
5. **Plates follow jurisdictions.** When a jurisdiction's tags change, it moves to another country, or it's deleted, the plates of loaded vehicles are set again.

**Why no key for `/park`:** you don't need a key to park a car in real life.

## Known limitations

- **Only `/park` and the editor save.** Position and damage aren't saved otherwise.
- **No lock command,** although `SetVehicleLockAsync` exists.
- **An old, long editor.** `CreateOrUpdateVehicleDialog` is over 500 lines and predates the conventions.

## Deep dive

- **Code:** `Features/Vehicles/`.
- **Components:**
  - `VehicleDataComponent`: the saved data;
  - `EngineStarting`: on a vehicle whose key was just turned. `EngineStartSystem` starts the engine when it's due;
  - `CreateOrUpdateVehicleDataComponent`: on an admin, while editing.
- **Services:**
  - `IVehicleManager`: `LoadAndUnloadVehicles`, `CreateVehicle`, `UpdateVehicle`, `RefreshNumberPlates` and `RemoveVehiclesFromJurisdiction`;
  - `IVehicleKeyService.HasVehicleKey`;
  - `IVehicleEngineService`: starting and stopping the engine. It's transient, because it saves through `BaseDataContext`.
- **Plates:** `VehicleHelper.GenerateCarPlate(id, tag)`, with the tag from `IJurisdictionService.GetPlateTag`.
- **The editor's "Registered in" row** is in its own partial file, `CreateOrUpdateVehicleDialog.Jurisdiction.cs`.
- **Events:** `VehicleServerSystem` handles `OnJurisdictionsLoaded`, `OnJurisdictionPlateTagChanged` and `OnJurisdictionDeleted`.
- **Tables:** [`Vehicles` and the model tables](../database.md#vehicles-vehiclemodel).

Want more? → [Hotwire](hotwire.md) · [Inventories](inventories.md)
