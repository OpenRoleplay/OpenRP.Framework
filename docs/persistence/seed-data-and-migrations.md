# Seed data and migrations

**[Seed data](../glossary.md#seed-data)** is written with `HasDefaultData`. **[Migrations](../glossary.md#migration)** live in the [host](../glossary.md#host), never in the Framework.

## Seed data

Use `HasDefaultData(...)` (`Database/Extensions/EntityTypeBuilderExtensions.cs`), never EF's `HasData(...)`.

**Why:** `BaseModel` stamps new rows with the current time. With `HasData`, every new migration would update every seed row again. `HasDefaultData` fixes the timestamps at 1 January 2025.

- **The Framework seeds only what every server needs:**
  - permission groups;
  - currencies and their units;
  - items;
  - the world inventory.
- **Server-specific rows** go in the host.
- **Properties built in-game** can be [exported as seed classes](../features/properties/default-data.md).

## Timestamps

`BaseDataContext.SaveChanges` sets `CreatedOn` and `UpdatedOn` on every row. It also gives every new character an inventory of 10 kg.

## Migrations

- **They live in the host.** **Why:** every host adds its own tables, so only the host can make migrations for its complete database.
- **Every model change gets a migration** in the host. The Framework commit says so with a `Database:` line.
- **Read the generated migration.** EF can drop and re-add a column you meant to rename.
- **Check the snapshot.** Right after `migrations add`, a second `migrations add Check` must come out empty; delete it after.
- **Never edit an applied migration.** Add a new one.
- **No auto-migrate.** Servers never call `Database.Migrate()` at startup. Run a reviewed [idempotent script](../glossary.md#idempotent-script) during the update instead. **Why:** servers sharing one database would race each other.
- **Stay backward-compatible** while servers update one by one, for example by adding columns as nullable first.
- **Indexes on foreign keys.** MySQL won't drop the only index a foreign key uses. Drop and re-add the foreign key around it, as in ECS's `PropertyDoorLinks` migration.

Want more? → [Persistence](../persistence.md) · [Every table](../database.md)
