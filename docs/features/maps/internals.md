# Maps internals

How the Maps feature keeps the database, the [ECS](../../glossary.md#ecs) entities and the streamer objects in step. Start with [Maps](../maps.md) for the overview.

## Entities and components

A map, its objects, their materials and its removed buildings are entities. A material belongs to its object, and a removal belongs to its map. An object is a root entity that points to its map. See [Store and database](#store-and-database).

| Component | Holds |
|---|---|
| `Map` | The name and the loaded flag. `GetRemovedBuildings()` returns its removals. |
| `MapObject` | Model, position, rotation, world, interior, description and stream distance. Its streamer object and its materials are children. |
| `MapObjectMaterial` | One material slot: a texture or text. |
| `MapRemovedBuilding` | One removed building. |
| `MapEditing` | On the admin's player: the map being edited and the selected object. It's removed when editing stops. |

## Store and database

Two services hold the map data. Only the store changes it.

- **`IMapDatabase` / `MapDatabase`** reads and writes rows. Each call uses its own short-lived database context, so nothing stays tracked between calls. `LoadAllRows()` runs once, at startup. Each change has its own method, such as `SaveObjectTransform` or `InsertObject`. A single-row write returns false when no row was changed, for example because another server deleted it. The store turns that into an error. `DeleteMapAsync` and the bulk replace run on a worker thread. While a delete runs, regeneration and changes are refused with "This map is being deleted."
- **`IMapStore` / `MapStore`** keeps the maps in memory, with indexes of objects by id and objects by map. It writes to the database first, and changes memory only after the write succeeds. A failed write leaves memory as it was.

`MapSystem` calls `LoadAll()` and then `SpawnLoadedMaps()` in `OnGameModeInit`. The tables aren't read again after that.

**Why** an object is a root entity, not a child of its map: a map can hold hundreds of thousands of objects, and one parent with that many children would be slow. The object points to its map instead.

**Why** the store writes before it changes memory: a failed write doesn't leave memory showing a change the database doesn't have.

**Why** there's no 60-second sync for maps: the sync can't handle that many rows. The cost is that other servers see changes only after their restart.

## Spawning

- `IMapService.SpawnLoadedMaps()` runs once at startup and spawns every loaded map.
- Each object becomes a streamer object (`IStreamerService.CreateDynamicObject`) with the object's stream distance. Its parent is the map object, so it's destroyed with it. Its materials are set on it afterwards, and then the spawn handlers run.
- A change that shows in the game respawns the object: the streamer object is destroyed and created again. A move or rotate in the editor moves the streamer object in place (`MoveSpawnedObject`), and calls the move hook of the spawn handlers.
- Removed buildings: `ApplyRemovedBuildings(player)` runs on connect for every loaded map. `ApplyRemovedBuilding(removal)` runs for every online player when a removal is added or its map loads. Both call `RemoveDefaultObjects`.

**Why** each object has its own stream distance: generated grass and flowers only matter up close, so a shorter distance keeps the number of objects each player has to stream in down. The default stays 300 m for hand-placed objects.

## Spawn handlers

A spawn handler attaches behaviour to the map objects of some models. The Harvestables feature uses one to make hemp a plant.

- **The interface:** `IMapObjectSpawnHandler`, in `Features/Maps/Entities/`. It has `Handles(modelId)`, `OnMapObjectSpawned(mapObject, dynamicObject)` and `OnMapObjectMoved(mapObject, dynamicObject)`. The move hook does nothing by default.
- **Registration:** `services.AddMapObjectSpawnHandler<T>()`.
- **How it runs:** `MapService` asks each handler once per model whether it handles that model, and caches the answer. It calls `OnMapObjectSpawned` after each spawn, and `OnMapObjectMoved` after an editor move. A handler that throws is logged, and the other handlers and the spawns carry on.
- **No despawn call.** Attach the behaviour to `dynamicObject.Entity`. It goes away with the streamer object.

**Why** a per-model call and not a server event: the handlers run for every object, up to about 400,000 times at startup. A server event would be published and matched for each one.

## Bulk replace

`IMapBulkService.ReplaceObjectsAsync(map, objects, progress)` swaps every object of a map for a new list. The official world generator uses it to fill the World map.

1. **Check.** `MapBulkValidator` checks every object first. One bad object refuses the whole replace, and nothing changes. The map is then marked as being replaced, so every change, load, unload, rename and delete is refused until the replace ends. Selecting and teleporting are refused too. Players editing the map are stopped. The lock ends with the replace, unless the game part fails (see below).
2. **Database, on a worker thread.** One transaction, with a 10-minute command timeout:
   - delete the map's materials and objects;
   - insert the objects with multi-row `INSERT`s of 1,000 rows;
   - read the new object ids back in insert order, and stop if the count is wrong;
   - insert the textures the same way, check their ids and order, set the map loaded, then commit.

   Any failure rolls the transaction back, so the old objects stay.
3. **Game, on the main thread.** The old objects are despawned and forgotten, then the new ones are added and spawned, in slices of 20,000 per tick. The spawn handlers run for each new object.
4. **Progress.** Each stage reports its progress.

If the database part succeeds but the game part fails, the new objects are saved, and the admin is told to restart the server. The map stays locked until then. A later regeneration is refused with "This map is locked after a failed regeneration. Restart the server first."

**Why** one transaction: a failed run leaves the old objects in place.

**Why** raw multi-row inserts: one insert per object would need an identity lookup for each row, and would log each statement to the console. For 400,000 objects that's too slow. The ids are read back with one query instead.

**Why** slices on the main thread: the game gets a turn between them, so players stay connected.

**Why** the map refuses changes meanwhile: its objects are removed and added over several ticks, so the map is between two states until the replace ends.

## Editing

- `IMapEditorService` makes every change. It returns an error message, or null. When a change can't be saved, the error is logged and the player is told to try again.
- `IMapEditingService` holds the session: the edited map, the selected object, and where new things start (the player, or the camera while flying).
- `MapEditorSystem` sends streamer clicks and moves to the editor while a player is editing. It returns true for the ones it handles. The current streamer wrapper ignores that value, so the bool has no effect yet.
- A move first calls `IStreamerService.Update` for the player, so the objects near them are streamed in. If the object still can't be edited, the move doesn't start.
- A teleport to a map from fly mode calls `IFlyModeService.StopFlyingAt`, so the flight ends at the map.
- `IMapDialogService` shows the menus. Each flow has its own file, `Services/Dialogs/MapDialogService.*.cs`.
- `MapInputParser` reads textures, material text and removed buildings. `MapDialogInput` reads model IDs, worlds, stream distances and descriptions.

## Commands

`EditMapsCommand` is `/editmaps`: Admin, in the `Maps` group.

Tables: [Maps](../../database.md#maps).

Want more? → [Maps](../maps.md) · [The map editor](editor.md) · [Persistence](../../persistence.md) · [Harvestables](../harvestables.md)
