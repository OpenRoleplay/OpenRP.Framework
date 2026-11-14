# The map editor

`/editmaps` (Admin) creates and changes maps in-game. Changes show in the game at once. They're saved as described in [Maps](../maps.md#how-it-works). A dialog for a map or object that has been deleted says so, and goes back.

## Map list

`/editmaps` shows this list when you aren't editing a map.

| Row | Does |
|---|---|
| **Create map** | Asks for a name of 1 to 64 characters, unique ignoring case. The new map is unloaded. |
| A map | Shows its name, its number of objects and its state, Loaded or Unloaded. Opens the map menu. |

A deleted map's name can be used again once the deletion is saved, which takes up to a minute. Until then, Create map and Rename say: "A map with that name was just deleted. Wait a minute for it to be saved, then try again."

## Map menu

Opened from the list, or from **Map menu** in the editor.

| Row | Does |
|---|---|
| **Edit objects** | Starts editing the map. An unloaded map asks you to load it first. |
| **Load** or **Unload** | Puts the objects in the game, or takes them out. After an unload, removed buildings stay removed until players reconnect. |
| **Teleport to map** | Puts you just above the map's first object (the lowest ID). It uses that object's world and interior, unless they're -1. While you fly, you land there instead, and your health, armour, skin and weapons are restored. |
| **Rename** | Takes a new name, with the same rules as create. |
| **Removed buildings** | See [Removed buildings](#removed-buildings). |
| **Delete** | Asks first. Deletes the map with its objects, materials and removed buildings. Every admin who is editing it stops editing. |

## Editor menu

Shown while you're editing a map. `/editmaps` opens it again.

| Row | Does |
|---|---|
| **Select object (click)** | Click an object of the map. Press Escape to stop. |
| **Nearest object** | Selects the object nearest to you, within 20 m. While you fly, it measures from the camera. |
| **Object list** | Lists the map's objects by ID. Shows the 200 nearest when there are more. |
| **Search by model** | Lists the objects with the model ID you enter. |
| **Add object** | Takes a model ID and an optional description, for example `3374 Shop sign`. The object goes in front of you, or in front of the camera while flying, in your world and interior. Its menu opens. |
| **Fly mode: On** or **Off** | Turns [fly mode](../fly-mode.md) on or off. |
| **Removed buildings** | See below. |
| **Map menu** | The map menu above. |
| **Stop editing** | Leaves edit mode and stops flying. The map stays loaded. |

## Object menu

Opened when you pick an object.

| Row | Does |
|---|---|
| **Move / rotate** | Moves the object with the game's edit mode. Final saves the new place and rotation, and the object moves in place. Cancel keeps the old one and reopens this menu. The map must be loaded. The editor first streams in the objects near you. If the object still isn't streamed in, you get "That object isn't streamed in near you yet. Move closer and try again." |
| **Model** | Changes the model ID. |
| **Duplicate** | Adds a copy in the same place, with the same materials, and puts the copy in move mode. |
| **Textures** | Material slots 0 to 15. Each slot has **Set texture** and **Clear**. |
| **Material text** | Puts text on a slot. |
| **World and interior** | Your own world and interior, everywhere (-1 -1), or two numbers you enter. |
| **Description** | Up to 64 characters. An empty entry clears it. |
| **Delete** | Asks first. |
| **Info** | The ID, model, position, rotation, world, interior, description and materials. |

**Textures** take `model txd texture [ARGB colour]`, for example `3374 ce_farmxref sw_barnwood1 0x00000000`. The colour is optional. Leaving it out keeps the texture's own colour.

**Material text** takes `text+index+font+size+bold+fontcolor+backcolor+alignment`, for example `Montgomery DMV+5+Arial+36+1+0xFF000000+0x00000000+1`. The index is 0 to 15, bold is 0 or 1, the colours are ARGB hex, and the alignment is 0 (left), 1 (centre) or 2 (right). The text can hold `+` signs, because the last seven fields are read from the end. Spaces around the `+` signs are fine, because every field is trimmed.

## Removed buildings

Opened from the map menu or the editor menu. The list has **Add**, then one row per removal.

- **Add** takes `model [radius]`, which removes the building around where you stand (or the camera, while flying). Or it takes `model x y z radius`. The radius defaults to 0.25. For example: `3374` or `3374 1.5 2.0 10.0 5`.
- When the map is loaded, the building goes for online players at once. Otherwise it goes when the map loads.
- Click a removal to bring its building back. It asks first. Players see the building again after they reconnect.

## Tips

- **Add object** only places the new object. Use **Move / rotate** to put it exactly where you want it.
- **Fly** to place objects high up or out of reach. **Add object** and **Nearest object** measure from the camera.

## Known limitations

- **Clicks on other objects stop there.** While you edit, a click on an object that isn't part of a map doesn't reach that object. You get "That object isn't part of a map." instead.
- **Object list cap.** It shows at most 200 objects.
- **Move needs a loaded map and a nearby object.** An object of an unloaded map isn't in the game, so it can't be moved. An object that isn't streamed in near you can't be moved either. Move closer to it.

Want more? → [Maps](../maps.md) · [Fly mode](../fly-mode.md) · [Internals](internals.md)
