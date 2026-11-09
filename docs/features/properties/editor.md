# The property editor

`/editproperties` (Admin) creates, changes and deletes properties and doors in-game. Changes show at once, and are saved within 60 seconds and on shutdown.

## The menus

**Main list**
- **Create property.**
- **Door I'm standing at:** shown when you're at a door.
- **Every property,** for example `Little Lady (#3) - 2 doors`.

**Property menu**
- **Rename.**
- **Add a door here:** where you stand, facing your way. It's inside the property whose virtual world you're in; otherwise, in an interior, inside its own property; otherwise outside. It puts you on the door, so you see its label.
- **Go to:** the first door.
- **Delete:** asks first, and deletes its doors too.
- **Its doors,** for example `Entrance (#7) -> Little Lady: Exit (#8)`.

**Door menu**
- **Go to.**
- **Move here:** to your spot, angle and interior. Which property it's inside is picked again, as for a new door.
- **Rename.**
- **Virtual world:** outside (the shared world), inside its own property, or inside another property. Puts you on the door afterwards.
- **Link to another door:** both ways. Doors that led back to either of the two no longer do.
- **Unlink:** both ways.
- **Delete:** asks first. Doors that led to it then lead nowhere.

Names are 1–64 characters.

## Tips

- **Getting into an interior:** use `/gotopos [x] [y] [z] [interior]`, for example to place an exit door. Without the interior you're at interior 0, and the door starts outside.
- **Rooms built into the map at interior 0** (Palomino Bank, Dillimore Gas Station): a door placed there starts outside, which is right for the real building. When the room is used for another property, set the door's virtual world to that property, so its visitors don't meet people in the real building.
- **A property inside another** (an apartment in a building): add the apartment's entrance while standing in the building's interior. It starts inside the building, so people in the hallway can see and use it.
- **Seeing a label:** "Add a door here" and "Move here" put you in the door's virtual world, so you see its label straight away.
- **Characters:** menu text uses plain characters (`->`, `-`). **Why:** SA-MP dialogs use the Windows-1252 encoding, which can't show symbols like `→`.

Want more? → [Properties](../properties.md) · [How it's built](internals.md)
