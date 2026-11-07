# The ECS model in detail

How entities, components, systems and services are used in the Framework.

## Entity ids

An `EntityId` is a type (a Guid) plus a number. There are three kinds:

| Kind | Number | Example |
|---|---|---|
| Database rows | the row id | `InventoryEntities.GetInventoryItemId((int)model.Id)` |
| Runtime-only | a counter that only goes up | fish hotspots, chicken coops |
| Not saved yet | `UnsavedEntities.GenerateUnsavedId()` | a new row before its first save |

**Why row ids:** you can find a row's entity directly, without searching.

Each feature defines its types in `Entities/<Feature>Entities.cs` as `[EntityType]` Guids. Never reuse a Guid.

## Components

Components hold state and expose it through small `Get…`/`Set…`/`Is…`/`Has…` methods. They don't run game flow, use the database or get services.

| Pattern | Example |
|---|---|
| Marker for temporary player state | `InventoryOpened`, `PlayerHarvesting`, `PendingPayment` |
| Child entity, destroyed with its owner | a door's label under the door, a door under its property |
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
