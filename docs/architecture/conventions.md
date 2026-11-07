# Conventions

The rules for new code and code you touch.

## Where code goes

- **One feature:** keep it in that feature. When a second feature needs it, move it to `Shared/`.
- **Other features:** calling another feature's service or reading its component is fine. Making it *react* goes through a [server event](../server-events.md).
- **Namespaces:** follow the folders (`OpenRP.Framework.Features.<Feature>.<Folder>`).
- **One `Add<Feature>()` per feature,** chained into `AddOpenRoleplayFramework()`. A feature that needs settings exposes `Add<Feature>(options => …)` instead, for the host to call.

## Naming

Methods start with a verb that says exactly what they do:

| Prefix | Means |
|---|---|
| `Get…` | returns something, changes nothing |
| `Set…` | changes one value |
| `Is…` / `Has…` / `Can…` | returns a `bool`, changes nothing |
| `Create…` / `Destroy…` | makes or removes entities or objects |
| `Load…` / `Save…` | database ↔ game |
| `Add…` / `Remove…` | changes a collection |
| `Send…` / `Show…` / `Open…` | output to a player |
| `Begin…` / `End…` | starts or ends something that takes time |
| `On…` | event handlers only |

Also:
- **Methods:** keep them short (around 20 lines), and use guard clauses instead of nested `if`s.
- **Magic numbers:** use named constants.
- **Comments** explain *why*. Public members get a one-line `/// <summary>`.

**Why:** a reader should understand a method from its name, without opening it.

## Adding a feature

1. Make `Features/<Name>/` with only the folders you need.
2. Add entity types and components.
3. Put the logic in services, and register them in one `Add<Name>()`.
4. React in systems.
5. Publish a server event for anything others may react to.
6. Add tables in `Models/`, and note `Database:` in the commit message. See [Persistence](../persistence.md).
7. Add commands and dialogs.
8. Test it through a host (the Boilerplate).

## Known inconsistencies

Older code doesn't follow every rule yet. Fix what you touch, but don't mass-refactor.

- **Console output:** some code uses `Console.WriteLine` instead of `ILogger<T>`.
- **`async void`:** some handlers still are, for example `FishHotspotSystem.OnGameModeInit` and `PaymentSystem.ContinueAcceptedPayments`.
- **Long classes:** `CreateOrUpdateVehicleDialog` and `Inventory` are far too long.
- **Test code:** some `OnPlayerConnect` handlers contain temporary test logic.

Want more? → [ECS model](ecs.md) · [Threading](threading.md)
