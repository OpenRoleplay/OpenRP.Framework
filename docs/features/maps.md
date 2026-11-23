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
3. **Saving.** Every change is saved to the database as soon as you make it. The server reads the maps once, at startup, and keeps them in memory after that.
   - **Why:** a map can hold hundreds of thousands of objects. The 60-second sync that other data uses can't handle that many rows, so maps save each change at once.
4. **Virtual world and interior per object.** Each object has its own [virtual world](../glossary.md#virtual-world) and [interior](../glossary.md#interior). -1 means everywhere.
   - **Why:** one map can mix outdoor and indoor parts, such as a building's outside and its inside.
5. **Removed buildings.** A map can remove GTA buildings for its players. A removal applies to every online player when its map loads or when it's added to a loaded map, and to each player who connects while the map is loaded.
   - **Why:** GTA can't bring a removed building back for a connected player. So unloading a map, or deleting a removal, takes effect after players reconnect.
6. **Stream distance.** Each object has its own distance, in metres, at which the game streams it in. Set it in the object menu, from 1 to 1000. Empty means the default, 300. Changing it respawns the object, because a streamer object keeps its old distance until it's respawned.
   - **Why:** generated grass and flowers only matter up close, so a shorter stream distance keeps the number of objects each player has to stream in down. The default stays 300 m for hand-placed objects.
7. **Objects that come alive.** Some models do more when they spawn. Hemp (object 19473) becomes a harvestable plant, and birch trees (models 763, 770 and 775) can be chopped in the official gamemode. This applies to every object of those models, including the ones you place by hand. See [Harvestables](harvestables.md) and [spawn handlers](maps/internals.md#spawn-handlers).
8. **Regenerated maps.** The official world generator can replace all of a map's objects at once. While it does, the map refuses almost everything, including edits, teleports and selecting, with "This map is being regenerated." If the database part succeeds but the game part fails, the map stays locked until the server restarts. See [Bulk replace](maps/internals.md#bulk-replace).

## Known limitations

- **No catalogue.** You type model IDs and texture names. The game doesn't list the models or textures you can use.
- **No undo.** Deleting asks first, but it can't be undone.
- **Other servers.** Other servers that share the database see map edits only after their own restart. If a row was changed or deleted on another server, the editor refuses the change and says so, and asks for a restart. If you delete an object that another server already deleted, the editor also removes it from the game.
- **No collision.** Map objects have no collision on the server.
- **Official server maps.** The maps hard-coded in the official server aren't in the database yet.
- **No export.** A map can't be exported as seed data.

## Deep dive

- [The map editor](maps/editor.md): every menu and what it does.
- [Internals](maps/internals.md): the store and database, spawn handlers and bulk replace.
- **Code:** `Features/Maps/`.
- **Tables:** [Maps, MapObjects, MapObjectMaterials and MapRemovedBuildings](../database.md#maps).

Want more? → [The map editor](maps/editor.md) · [Fly mode](fly-mode.md) · [Virtual worlds](virtual-worlds.md) · [Admins](admins.md)
