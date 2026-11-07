# BaseManager

`Shared/BaseManager/` keeps the components of one table in sync with the database. The components in memory are the live state.

Used by factions (5 tables), inventories and inventory items, and properties and doors.

**Why:** memory is fast on the game's single thread, and saving in batches cuts database load. Each sync also picks up other servers' changes.

## The cycle

| When | Call | What happens |
|---|---|---|
| Server start | `LoadAll()` | every row becomes an entity with its component |
| Every 60 s (`[Timer]`) | `ProcessChanges()` | save, then pull others' changes |
| Server stop | `SaveNow()` | save, so a restart loses nothing |

**Saving** (`SaveNow`) has three steps:
1. Save changed components.
2. Insert new ones.
3. Delete the marked ones.

If the batch fails, rows are saved one by one, so one bad row doesn't block the rest.

**Pulling** loads rows added or changed elsewhere since the last check, by `CreatedOn`/`UpdatedOn` with a 5-second overlap, and removes rows deleted elsewhere.
- **Local unsaved changes win** until they're saved.
- **Between two servers,** the last save wins.
- **A deletion elsewhere wins** over local changes.

## New and deleted rows

- **Created unsaved:** a new component has id 0 and a temporary entity id. It moves to its real id on the first save, so don't keep its `EntityId`.
- **`CreateAndLoad(model)`** inserts right away and returns the component. Use it when you need the id now, for example to link a new door.
- **`LoadById(id)`** loads one row now, instead of at the next sync.
- **Deleting:** mark it with `ProcessDeletion()`. It's deleted at the next save. Until then, lookups must skip it.

## Writing a manager

```csharp
public class PropertyDoorManager : BaseManager<PropertyDoorModel, PropertyDoor>, IPropertyDoorManager
{
    protected override DbSet<PropertyDoorModel> GetDbSet() => _dataContext.PropertyDoors;
    protected override EntityId GetEntityId(PropertyDoorModel model) => PropertyEntities.GetPropertyDoorId((int)model.Id);
    protected override EntityId? GetParentEntityId(PropertyDoorModel model) => /* the property, if any */;
    protected override PropertyDoorModel GetModelFromComponent(PropertyDoor component) => component.GetRawPropertyDoorModel();
    protected override void OnComponentLoaded(PropertyDoor component) { /* labels, objects */ }
}
```

**The component implements:**
- `IBaseDataComponent` (`GetId`);
- `IChangeable` (its `Set…` methods call `ProcessChanges()`);
- `IDeletable`, when rows can be deleted.

**`OnComponentLoaded`** runs after every load or reload, for example to redraw a door's label after a rename on another server.

**Wiring:**
- call `LoadAll()` in `OnGameModeInit` (parents first);
- call `ProcessChanges()` on a 60-second timer;
- call `SaveNow()` in `OnGameModeExit`.

Managers are singletons with their own database context. That's safe because everything runs on the main thread, and the change tracker is cleared after every save.

Want more? → [Persistence](../persistence.md) · [Sessions](sessions.md)
