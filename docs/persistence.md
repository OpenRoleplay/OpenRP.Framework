# Persistence

How game data is loaded and saved. The Framework keeps data in MySQL/MariaDB through Entity Framework Core 7. The [host](glossary.md#host) owns the database and its [migrations](glossary.md#migration); the Framework owns the table models.

## Three ways data is saved

| Way | Used for | Live state is in |
|---|---|---|
| [BaseManager](persistence/base-manager.md) | world data: factions, inventories, properties | memory; saved every 60 seconds and on shutdown |
| [Session data](persistence/sessions.md) | the character a player is playing | the player; saved every 60 seconds and on disconnect |
| Direct database access | accounts, settings, permissions, vehicles, dropped items, actors, items | the database; read when needed or at startup |

**Why memory first:** the game runs on one [thread](glossary.md#main-thread). A database call on every change would stall it, while memory is fast and saving in batches cuts the database load. A crash loses at most a minute.

**Why the 60-second sync:** several servers can share one database. Each sync also picks up what the other servers changed.

## One login per account

An account can be logged in once, on one server, at a time. That makes the selected character's data safe to save without merging. See [Sessions](persistence/sessions.md).

## Seed data and migrations

- **[Seed data](glossary.md#seed-data):** rows every database starts with. Use `HasDefaultData`, never `HasData`.
- **Migrations:** live in the host, never in the Framework.
- **No auto-migrate:** servers never migrate themselves at startup.

See [Seed data and migrations](persistence/seed-data-and-migrations.md).

Want more?
- [BaseManager in detail](persistence/base-manager.md)
- [Sessions and logins](persistence/sessions.md)
- [Seed data and migrations](persistence/seed-data-and-migrations.md)
- [Every table](database.md)
