# Maps internals

How the Maps feature keeps the database, the [ECS](../../glossary.md#ecs) entities and the streamer objects in step. Start with [Maps](../maps.md) for the overview.

## Entities and components

A map, its objects, their materials and its removed buildings are entities. Each one is a child of the entity that owns it: a material belongs to an object, and an object or a removal belongs to a map. The ids come from `MapEntities`, with one type key per table.

| Component | Holds |
|---|---|
| `Map` | The name, and the loaded and deleted flags. `GetObjects()` and `GetRemovedBuildings()` skip deleted children. |
| `MapObject` | Model, position, rotation, world, interior and description. Its streamer object is a child. |
| `MapObjectMaterial` | One material slot: a texture or text. |
| `MapRemovedBuilding` | One removed building. |
| `MapEditing` | On the admin's player: the map being edited and the selected object. It's removed when editing stops. |

## Managers and sync

- `IMapManager`, `IMapObjectManager`, `IMapObjectMaterialManager` and `IMapRemovedBuildingManager` are [BaseManagers](../../persistence/base-manager.md). `CreateAndLoad` saves a new row at once.
- `MapSystem` loads the tables at startup, maps first, so children find their map. It saves every 60 seconds and on shutdown.
  - **Why:** children are saved before the maps they belong to, so their deletes run before the database cascades a map's delete to them.
- Changes from another server show here after startup. A new or changed object is spawned, or respawned, when its map is loaded. A changed material respawns its object. A map loaded or unloaded there is spawned or despawned to match. None of this runs before `SpawnLoadedMaps()` has run.
- Removed buildings from another server apply live too: a new one is applied to the online players when its map is loaded, and a map loaded there applies all of its removed buildings.
- A deleted map keeps its name until the next save. `FindDeletedMapByName` finds it, so a new or renamed map can't take that name until then.

## Spawning

- `IMapService.SpawnLoadedMaps()` runs once at startup and spawns every loaded map.
- Each object becomes a streamer object (`IStreamerService.CreateDynamicObject`) whose parent is the map object, so it's destroyed with it. Its materials are set on it afterwards.
- Any change that shows in the game respawns the object: the streamer object is destroyed and created again, when its map is loaded. The exception is a move or rotate in the editor, which moves the streamer object in place (`MoveSpawnedObject`).
- Removed buildings: `ApplyRemovedBuildings(player)` runs on connect for every loaded map. `ApplyRemovedBuilding(removal)` runs for every online player when a removal is added or its map loads. Both call `RemoveDefaultObjects`.

## Editing

- `IMapEditorService` makes every change. It returns an error message, or null. When a new row can't be saved, the error is logged and the player is told to try again.
- `IMapEditingService` holds the session: the edited map, the selected object, and where new things start (the player, or the camera while flying).
- `MapEditorSystem` sends streamer clicks and moves to the editor while a player is editing. It returns true for the ones it handles. The current streamer wrapper ignores that value, so the bool has no effect yet.
- A move first calls `IStreamerService.Update` for the player, so the objects near them are streamed in. If the object still can't be edited, the move doesn't start.
- A teleport to a map from fly mode calls `IFlyModeService.StopFlyingAt`, so the flight ends at the map.
- `IMapDialogService` shows the menus. Each flow has its own file, `Services/Dialogs/MapDialogService.*.cs`.
- `MapInputParser` reads textures, material text and removed buildings. `MapDialogInput` reads model IDs, worlds and descriptions.

## Commands

`EditMapsCommand` is `/editmaps`: Admin, in the `Maps` group.

Tables: [Maps](../../database.md#maps).

Want more? → [Maps](../maps.md) · [The map editor](editor.md) · [Persistence](../../persistence.md)
