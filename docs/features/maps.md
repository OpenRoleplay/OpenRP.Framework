# Maps

Maps are named sets of objects that admins build in-game with `/editmaps`. Their objects, textures, text and the GTA buildings they remove are saved in the database. A loaded map comes back after a restart.

## How to use it

| Command | Groups | Does |
|---|---|---|
| `/editmaps` | Admin | Opens the map list. While you're editing a map, it opens that map's editor |

To make a map:
1. `/editmaps`, then **Create map**, and enter a name of up to 64 characters.
2. Open the map and choose **Edit objects**. If the map isn't loaded, it asks you to load it first. Then you start editing it.
3. Choose **Add object**, then enter a model ID, for example `3374 Shop sign`. The object appears in front of you.

See [The map editor](maps/editor.md) for every menu.

## How it works

1. **Rows in the database.** A map is one row in `Maps`. Its objects, their materials and its removed buildings are rows in their own tables. See [Database](../database.md#maps).
2. **Loaded or unloaded.** Only a loaded map's objects are in the game. The loaded state is saved, so a loaded map loads again after a restart.
   - **Why:** a restart shows the same world that the admins left.
3. **Saving.** A new map, object, material or removal is saved at once. Other changes are saved within 60 seconds, and on shutdown.
4. **Virtual world and interior per object.** Each object has its own [virtual world](../glossary.md#virtual-world) and [interior](../glossary.md#interior). -1 means everywhere.
   - **Why:** one map can mix outdoor and indoor parts, such as a building's outside and its inside.
5. **Removed buildings.** A map can remove GTA buildings for its players. A removal applies to every online player when its map loads or when it's added to a loaded map, and to each player who connects while the map is loaded.
   - **Why:** GTA can't bring a removed building back for a connected player. So unloading a map, or deleting a removal, takes effect after players reconnect.

## Known limitations

- **No catalogue.** You type model IDs and texture names. The game doesn't list the models or textures you can use.
- **No undo.** Deleting asks first, but it can't be undone.
- **Two servers.** When two servers change the same row, the last save wins.
- **No collision.** Map objects have no collision on the server.
- **Official server maps.** The maps hard-coded in the official server aren't in the database yet.
- **No export.** A map can't be exported as seed data.

## Deep dive

- [The map editor](maps/editor.md): every menu and what it does.
- [Internals](maps/internals.md): components, managers, services and how objects spawn.
- **Code:** `Features/Maps/`.
- **Tables:** [Maps, MapObjects, MapObjectMaterials and MapRemovedBuildings](../database.md#maps).

Want more? → [The map editor](maps/editor.md) · [Fly mode](fly-mode.md) · [Virtual worlds](virtual-worlds.md) · [Admins](admins.md)
