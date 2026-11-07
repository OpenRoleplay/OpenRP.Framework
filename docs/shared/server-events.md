# How server events are dispatched

The rules and internals of `Shared/ServerEvents/`. For the list of events and a short example, see [Server events](../server-events.md).

## Handlers

- **Where:** a handler is a `[ServerEvent]` method in a class that implements `IServerSystem`.
- **Which event:** its one `ServerEventArgs` parameter decides which event it handles. The method name doesn't matter, but by convention it's `On<Event>`.
- **Other parameters** are services. They come from the root service provider, not a scope.
  - **Why:** a handler may pass them to a dialog whose callback runs after the handler has returned, so they must stay alive.
- **Async handlers** return `Task`. An `async void` handler isn't awaited, and loses its errors.

## Publishing

`IServerEventAggregator.PublishAsync` runs every matching handler **one after another**, and waits for all of them.

## Registration and errors

- **Registration:** `AddServerSystemsInAssembly()` registers the `IServerSystem` classes of the calling assembly as singletons. The Framework calls it for itself; each host calls it for its own assembly.
- **Startup checks:** the dispatcher is built at startup (`OpenRoleplayFrameworkSystem.OnGameModeInit`). Problems are logged right away:
  - a missing event args parameter;
  - a parameter that isn't a registered service;
  - an unregistered server system;
  - an `async void` handler (a warning).
- **Errors while running:** an exception in one handler is logged with its name, and the other handlers still run.
- **Logging:** the dispatcher logs to `Logs/ServerEvents/` too.

Want more? → [Server events](../server-events.md) · [Architecture](../architecture.md)
