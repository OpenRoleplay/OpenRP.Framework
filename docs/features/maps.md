# Maps

A placeholder for storing mappings (custom objects) in the database (`Features/Maps/`). It doesn't work yet.

## What exists

- A `Map` component.
- A `MapModel` (`Name`).

## What's missing

- A table: `MapModel` has no `DbSet`.
- A manager, a service, a system and commands.

Mappings are code in the host for now, for example the official server's `Features/Mappings`.

Want more? → [Persistence](../persistence.md)
