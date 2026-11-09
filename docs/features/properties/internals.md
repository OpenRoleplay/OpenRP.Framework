# Properties: internals

How properties are built (`Features/Properties/`). For what they do, see [Properties](../properties.md).

## Components

| Component | Holds |
|---|---|
| `Property` | name. `GetDoors()` skips deleted doors. Can be deleted (`IDeletable`) |
| `PropertyDoor` | name, property, linked door, position, angle, interior, and the property it's inside. `GetLabelText()`, `IsInside()`, `IsPlayerNearby()`, `IsLinkedTo()` |

A door is a child entity of its property, and its label is a child of the door.

## Services

| Service | Does |
|---|---|
| `IPropertyService` | Using doors, finding doors, labels, `MovePlayerToDoor` |
| `IPropertyEditorService` | Creating, renaming, moving, linking and deleting. Returns an error message, or null |
| `IPropertyDialogService` | The editor menus, in `PropertyDialogService.Properties.cs` and `.Doors.cs` |
| `IPropertyManager`, `IPropertyDoorManager` | [BaseManager](../../persistence/base-manager.md) persistence, with `CreateAndLoad` |
| `IPropertyDefaultDataExporter` | The [default data export](default-data.md) |

## Rules worth knowing

- **Deleting a door:**
  1. its label disappears at once;
  2. every door linking to it is unlinked in memory;
  3. it's marked deleted.

  **Why unlink in memory:** the database clears those links too, but a loaded door would save its old link back over the cleared one.
- **A door's virtual world:** `PropertyService.GetDoorVirtualWorld` returns the virtual world of the property the door is inside (`InsidePropertyId`), or 0 when it's outside. The interior number plays no part.
- **The default for a new or moved door** (`PropertyEditorService.GetInsidePropertyIdForPlayer`):
  1. the property whose virtual world the admin stands in, found with `IVirtualWorldManager.GetPropertyIdOfVirtualWorld`;
  2. otherwise, in a non-zero interior, the door's own property;
  3. otherwise outside.
- **Deleting a property:** loaded doors inside it are put outside in memory. **Why:** the same reason as unlinking: a loaded door would save the old value back.
- **Where a door leads:** `PropertyService.GetDestinationDoor(door)`: its own link, otherwise the lowest-numbered door linking to it (`GetDoorsLinkingTo`), otherwise nowhere. `/enter`, `/exit` and the editor all use it.
- **Linking** (`PropertyEditorService.LinkDoors`) stores one direction. If the other door already links back, the door's own link is cleared instead, and a link that would close a loop is refused (`WouldCreateLoop`). **Why:** the links must stay cycle-free to fit in seed data.
- **Creating with `CreateAndLoad`:** new properties and doors are created this way. **Why:** so they have an id at once, and can be linked straight away.
- **`OnComponentLoaded`** redraws labels. **Why:** a rename on another server shows up after the sync.

## Systems and tables

- **`PropertySystem`:**
  - loads properties, then doors, then exports the default data;
  - syncs every 60 seconds;
  - saves on shutdown.
- **Tables:** [`Properties`, `PropertyDoors`](../../database.md#world).
  - Deleting a property deletes its doors ([cascade](../../glossary.md#cascade)).
  - Deleting a door clears links to it (set null).

Want more? → [Properties](../properties.md) · [Editor](editor.md)
