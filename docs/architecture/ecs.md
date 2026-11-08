# The ECS model in detail

How entities, components, systems and services are used in the Framework.

## Entity ids

An `EntityId` is a random Guid (SampSharp 1.0). There are three kinds:

| Kind | Where it comes from | Example |
|---|---|---|
| Database rows | `RowEntityIds.Get(typeKey, rowId)`: the same row always gets the same id | `InventoryEntities.GetInventoryItemId((int)model.Id)` |
| Runtime-only | `EntityId.NewEntityId()` | fish hotspots, chicken coops |
| Not saved yet | `EntityId.NewEntityId()`; on the first save it becomes the row's entity id | a new inventory item |

**Why look row ids up:** you can find a row's entity directly, without searching.

Each feature keeps one type key (a Guid) per table in `Entities/<Feature>Entities.cs`. Never reuse a Guid.

**Entity ids only last while the server runs.** They're random, so they differ after a restart and between servers. Save and share row ids, never entity ids.

**Why random:** SampSharp never hands out an entity id twice, so an old reference finds nothing instead of the wrong entity. Row ids are the lasting keys; `RowEntityIds` links the two while the server runs.

An entity exists while it has a component or a child. Adding the first component creates it (`AddComponent<T>(entity, parent, args)`), and removing the last one removes it.

## Components

Components hold state and expose it through small `Get…`/`Set…`/`Is…`/`Has…` methods. They don't run game flow, use the database or get services.

| Pattern | Example |
|---|---|
| Marker for temporary player state | `InventoryOpened`, `PlayerHarvesting`, `PendingPayment` |
| Child entity, destroyed with its owner | a door under its property; a door's label: `CreateDynamicTextLabel(…, parent: door.Entity)` |
| Reference by `EntityId`, not by component | look it up when needed, it may be gone |

**Why no logic with other entities:** components get no services, so they can't log, publish events or use the database. That logic belongs in a service.

## Systems

- Handlers get what they need as **method parameters**, so systems hold no state.
- Systems are [singletons](../glossary.md#singleton). A transient service (anything with a database context) must be a handler parameter, never a constructor parameter.
- A handler reads like a short list of service calls.

## Commands and dialogs

- **Commands** are systems with `[ServerCommand]`. They check input, call a service and reply. See [Commands](../shared/commands.md).
- **Dialogs** use the shared `Better…Dialog` classes inside a feature's `I<Feature>DialogService`. See [Dialogs](../shared/dialogs.md).

Want more? → [Threading](threading.md) · [Conventions](conventions.md)
