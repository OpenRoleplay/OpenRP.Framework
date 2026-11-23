# Contributing to OpenRP.Framework

Thanks for helping build Open Roleplay. This page explains how to get a change merged. How the code works is in [`docs/`](docs/README.md).

## Before you start

- **Small fixes** (bugs, typos, docs): open a pull request.
- **New features or big changes:** talk to us first, in a GitHub issue or a support ticket on the [Open Roleplay Discord](https://open-roleplay.com/). Many features are partly built already, and we'd rather tell you before you spend a weekend on it.
- **What belongs here:** gameplay logic any roleplay server could use. Server-specific things (branding, maps, default data for one server, keys and passwords) belong in that server's own project, the [host](docs/glossary.md#host).

## Set up

You need the .NET 10 SDK (a recent one: older SDKs fail with "no static Main method"), and to run it, MySQL or MariaDB and an open.mp x64 server.

1. Fork and clone the repo, then `dotnet build OpenRP.Framework.sln`.
2. To try your change in game, use [OpenRP.Boilerplate](https://github.com/OpenRoleplay/OpenRP.Boilerplate), a minimal host for Windows. Its `Prepare.ps1` sets up a local server.
3. The Boilerplate uses the published NuGet package. To test your local Framework, temporarily replace its `PackageReference` with a `ProjectReference` to your clone. Don't commit that swap.

More: [Getting started](docs/getting-started.md).

## Making a change

Read [Architecture](docs/architecture.md) first. The short version:

| Rule | Details |
|---|---|
| Code lives in `Features/<Feature>/`, split by role; code two features use goes in `Shared/`. | [Conventions](docs/architecture/conventions.md) |
| State lives in components, logic in services, systems only react. | [ECS](docs/architecture/ecs.md) |
| Features react to events; they don't call each other to make things happen. | [Server events](docs/server-events.md) |
| Only touch the game world on the main thread. | [Threading](docs/architecture/threading.md) |
| Small methods with names that say what they do (`Get…`, `Is…`, `Create…`). | [Conventions](docs/architecture/conventions.md#naming) |
| Commands use `[ServerCommand]`, dialogs use `ShowIgnoringDisconnect`. | [Commands](docs/shared/commands.md), [Dialogs](docs/shared/dialogs.md) |

- **Database changes:** the Framework has no migrations; each host makes its own. Seed with `HasDefaultData`, add new columns as nullable, and say what changed in your commit (see below). More: [Seed data and migrations](docs/persistence/seed-data-and-migrations.md).
- **Docs:** update `docs/` in the same pull request: the feature's page, and `commands.md`, `server-events.md` or `database.md` when you add one of those. Keep pages short and plain.
- **Don't** reformat or refactor code your change doesn't need, and never commit keys, tokens, IPs or Discord IDs.

## Commits and pull requests

- Branch from `dev` and open your pull request against `dev`. `main` is the release branch: every merge into it publishes a new NuGet version.
- **Commit messages:** one line that says what changed, in sentence case without a full stop, for example `Add batteries to phones and handheld radios`. Add bullet points below it if needed. If you changed the database, add a line like `Database: new table JurisdictionSettings. Hosts need a migration.`
- **In the pull request,** say what changed and why, how you tested it in game, and whether it changes the database.
- The build must pass. There are no automated tests yet, so describe your in-game test.

## Using AI tools

AI-assisted contributions are welcome, as long as you have read and tested what you submit. [AGENTS.md](AGENTS.md) gives coding agents the project's rules; point yours at it.

## Licence

OpenRP.Framework is licensed under the [LGPL-3.0](LICENSE.md). By contributing, you agree that your contribution is licensed under it too. Only submit code you wrote or have the right to share.

## Questions

Ask in a GitHub issue or open a support ticket on the Open Roleplay Discord.
