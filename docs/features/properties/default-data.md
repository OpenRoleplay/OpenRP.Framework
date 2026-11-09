# Default data export

On every startup, the server writes the properties and doors from the database as C# [seed data](../../glossary.md#seed-data) classes, so properties built in-game can ship with a server.

**Why:** typing coordinates and door links into code by hand is slow, and easy to get wrong.

## The files

They're written to `scriptfiles/DefaultData/`, and overwritten on every start.

| File | Contains |
|---|---|
| `PropertyDefaultData.cs` | The properties (`HasDefaultData` rows), with their jurisdiction |
| `PropertyDoorDefaultData.cs` | The doors, with the property each is inside and their links |

The namespace is `<host>.Features.Properties.Models.DefaultData`, so the files fit the host as they are.

**Why the links fit in the doors file:** each pair of doors stores one link, so no two rows point at each other. EF Core can't insert seed rows that do: its sort only breaks dependencies within a table or on unique indexes, never foreign keys. Two doors that still link to each other (linked before links went one way) are exported with only the lower id's link; the other door leads back, so they work the same.

## Shipping properties with a server

1. Copy the two DefaultData classes into the host, and apply them in its `DataContext`.
   - **Jurisdictions:** a property's `JurisdictionId` must point at a jurisdiction the host seeds too. Set it to `null` for jurisdictions created in-game, or seed those as well.
2. Run `migrations add`. The links are in the doors' rows, so there's nothing to add by hand.
3. On the database you exported from, mark the migration as applied instead of running it, because the rows are already there.

Changing seeded doors or links later is the same: edit the seed file and run `migrations add`.

## Deep dive

- **The exporter:** `IPropertyDefaultDataExporter.ExportDefaultData()`, called by `PropertySystem` right after loading.
- **Writing values:** `Helpers/PropertyDefaultDataFiles` builds the text, and `Helpers/CSharpLiterals` writes the values.
  - **Floats** use the invariant culture with round-trip precision. **Why:** a door ends up exactly where it was, and the Dutch server culture doesn't swap `.` for `,`.
  - **Names** are escaped.

Want more? → [Seed data and migrations](../../persistence/seed-data-and-migrations.md)
