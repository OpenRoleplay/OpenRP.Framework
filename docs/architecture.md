# Architecture

The Framework is a set of **features** (accounts, factions, properties, …), each in its own folder, built on SampSharp's [ECS](glossary.md#ecs). Shared code sits in `Shared/`.

## How the code is organised

```
src/OpenRP.Framework/
  Features/<Feature>/   everything for one feature: Commands, Components, Services, Systems, …
  Shared/               code used by several features (BaseManager, Chat, Commands, Dialogs, ServerEvents, …)
  Database/             the table models and BaseDataContext
  Extensions/           AddOpenRoleplayFramework() and ApplyOpenRoleplayFrameworkModelConfigurations()
```

**Why:** you find everything about a feature in one place, and a feature can be read, changed or removed on its own.

## The four building blocks

| Block | What it is | Example |
|---|---|---|
| [Entity](glossary.md#entity) | An identity | a door |
| [Component](glossary.md#component) | State on an entity | `PropertyDoor` |
| [System](glossary.md#system) | Reacts to events and timers | `PropertySystem` |
| [Service](glossary.md#service) | The logic | `IPropertyService` |

## When code runs

| Trigger | Attribute | For |
|---|---|---|
| open.mp callbacks | `[Event]` | `OnPlayerConnect`, `OnGameModeInit`, … |
| Timers | `[Timer(ms)]` | work that is really time-based |
| Framework events | `[ServerEvent]` | `OnCharacterSelected`, … ([list](server-events.md)) |

**Why events:** a feature announces what happened, and others react without the two knowing each other. That keeps features independent.

## The one rule you must not break

Only the [main thread](glossary.md#main-thread) may touch players, objects and components.

**Why:** open.mp, its plugins and SampSharp aren't thread-safe. See [Threading](architecture/threading.md).

Want more?
- [The ECS model in detail](architecture/ecs.md)
- [Threading](architecture/threading.md)
- [Conventions and adding a feature](architecture/conventions.md)
