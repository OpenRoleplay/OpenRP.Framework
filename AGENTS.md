# AGENTS.md

Instructions for AI coding agents working on OpenRP.Framework. Humans: see [CONTRIBUTING.md](CONTRIBUTING.md).

## The project

OpenRP.Framework is the open-source (LGPL-3.0) gameplay library of Open Roleplay, a GTA San Andreas roleplay gamemode for open.mp. It is written in C# on .NET 10 with SampSharp 1.0 (`SampSharp.OpenMp.Entities`, an entity-component-system) and EF Core 9. Servers ("hosts") reference it as the `OpenRP.Framework` NuGet package and add their own data and settings. The Framework never references a host.

- Code: `src/OpenRP.Framework/` (`Features/`, `Shared/`, `Database/`, `Extensions/`).
- Docs: `docs/`, starting at `docs/README.md`.

## Commands

- Build: `dotnet build OpenRP.Framework.sln`. CI runs `dotnet build --configuration Release`.
- There are no automated tests. Changes are tested in game through a host such as OpenRP.Boilerplate; say so if you couldn't.

## Read first

| Before you touch | Read |
|---|---|
| Anything | `docs/architecture.md`, `docs/architecture/conventions.md` |
| Components, entities, systems | `docs/architecture/ecs.md` |
| async code, timers | `docs/architecture/threading.md` |
| Saving data, tables, seed data | `docs/persistence.md`, `docs/persistence/seed-data-and-migrations.md` |
| Events between features | `docs/server-events.md` |
| Commands, dialogs | `docs/shared/commands.md`, `docs/shared/dialogs.md` |

## Rules

### Where code goes
- Put all logic in `Features/<Feature>/`, split by role: `Commands/`, `Components/`, `Entities/`, `Enums/`, `Extensions/`, `Helpers/`, `Models/` (EF `Configuration/`, `DefaultData/`), `Services/` (`Dialogs/`), `Systems/`. Name the feature after the game concept (`Fishing`, `Vehicles`).
- Code used by more than one feature goes in `Shared/`. Don't reach into another feature's `Helpers/`.
- Namespaces match folders: `OpenRP.Framework.Features.<Feature>.<Folder>`.
- Each feature registers its services in exactly one `Add<Feature>()` in its `Extensions/ServiceCollectionExtensions.cs`, chained into `AddOpenRoleplayFramework()`. A feature that needs options exposes `Add<Feature>(options => ...)` for the host instead.

### How code is written
- One method does one thing; about 20 lines is the point to extract. Verb-first names: `Get…` (no side effects), `Set…`, `Is…`/`Has…`/`Can…` (bool, no side effects), `Create…`/`Destroy…`, `Load…`/`Save…`, `Add…`/`Remove…`, `Send…`/`Show…`/`Open…`, `Begin…`/`End…`, and `On…` only for event handlers.
- Full words, guard clauses, named constants for model ids and intervals. Comments explain why. Public service and component methods get a one-line `/// <summary>`.
- Log with an injected `ILogger<T>`, never `Console.WriteLine`. Handlers return `Task`, never `async void`.
- Colours and messages: `ChatColor`, `ServerColor`, `PlayerInfoMessageType`; never hardcode colour strings.

### Events and ECS
- React to events: `[Event]` for open.mp callbacks, `[Timer(ms)]` only for genuinely time-based work, `[ServerEvent]` for the Framework's own events. Don't poll in a timer when an event exists or can be added.
- To let other features react, add `Shared/ServerEvents/Entities/EventArgs/On<Subject><PastTenseVerb>EventArgs.cs` and publish it with `IServerEventAggregator.PublishAsync`. Nothing to register.
- State lives in components (small `Get/Set/Is/Has` methods, no database, no services). Logic involving other entities, services or events lives in services. Systems only hold handlers that call services.
- Entity ids are random and only last while the server runs: save and share row ids. Database rows get their entity id from `RowEntityIds.Get(typeKey, rowId)`.
- After an `await`, check `component.IsComponentAlive` before using a component you got earlier.
- Only touch entities, components and the game world on the main thread. `async`/`await` started from the main thread is fine; never touch the world from `Task.Run` or other threads.
- Services holding a `BaseDataContext` are transient: take them as handler method parameters, never in a singleton's constructor.

### Commands and dialogs
- Commands: an `ISystem` in `Commands/` with `[ServerCommand(...)]`. Never use SampSharp's `[PlayerCommand]`: it skips permissions.
- Dialogs: the shared `Better…Dialog` classes inside the feature's `I<Feature>DialogService`, always shown with `ShowIgnoringDisconnect`, never `Show` (answering a player who left crashes the server).

### Database
- The Framework owns the model and never contains migrations; hosts generate them.
- Seed with `HasDefaultData(...)`, never `HasData(...)`, and only seed what every server needs.
- Several servers can share one database: add new columns as nullable first.
- A commit that changes the model gets a line like `Database: new table X. Hosts need a migration.`

### Docs
- Update `docs/` in the same change: the feature's page (`docs/features/<feature>.md`, new features also get a line in `docs/README.md`), plus `commands.md`, `server-events.md` and `database.md` when those change. Remove known limitations you fixed; add ones you find.
- Style: answer first, about one screen per page, tables for lists, plain English, a **Why:** line per design decision, relative links, no em dashes. If you don't know why something was done, ask instead of guessing.

### Don't
- Don't add keys, tokens, passwords, IPs, Discord IDs or anything specific to one server.
- Don't refactor or reformat code your task doesn't need. Existing code that breaks these rules is listed in `docs/architecture/conventions.md#known-inconsistencies`; fix it only where you are working anyway.
- Don't commit, push or open pull requests unless asked. Branch from `dev`, never commit to `main`. 

## Before you finish

- `dotnet build` passes with no new warnings.
- Docs updated; every link and `#heading` works.
- Say what you tested in game and what you didn't.
