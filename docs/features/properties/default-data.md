# Default data export

On every startup, the server writes the properties and doors from the database as C# [seed data](../../glossary.md#seed-data) classes, so properties built in-game can ship with a server.

**Why:** typing coordinates and door links into code by hand is slow, and easy to get wrong.

## The files

They're written to `scriptfiles/DefaultData/`, and overwritten on every start.

| File | Contains |
|---|---|
| `PropertyDefaultData.cs` | The properties (`HasDefaultData` rows) |
| `PropertyDoorDefaultData.cs` | The doors, **without** their links |
| `PropertyDoorLinks.cs` | `SetLinks(MigrationBuilder)`: one `UpdateData` call per linked door |

The namespace is `<host>.Features.Properties.Models.DefaultData`, so the files fit the host as they are.

**Why the links are separate:** linked doors point at each other, and EF Core can't insert rows that point at each other in seed data. So the doors are inserted first, and linked afterwards.

## Shipping properties with a server

1. Copy the two DefaultData classes into the host, and apply them in its `DataContext`.
2. Run `migrations add`.
3. Paste the lines inside `SetLinks` at the end of the migration's `Up()`.
   - Don't call the class itself. **Why:** the file is regenerated, and an applied migration must never change.
4. On the database you exported from, mark the migration as applied instead of running it, because the rows are already there.

## Deep dive

- **The exporter:** `IPropertyDefaultDataExporter.ExportDefaultData()`, called by `PropertySystem` right after loading.
- **Writing values:** `Helpers/PropertyDefaultDataFiles` builds the text, and `Helpers/CSharpLiterals` writes the values.
  - **Floats** use the invariant culture with round-trip precision. **Why:** a door ends up exactly where it was, and the Dutch server culture doesn't swap `.` for `,`.
  - **Names** are escaped.

Want more? → [Seed data and migrations](../../persistence/seed-data-and-migrations.md)
