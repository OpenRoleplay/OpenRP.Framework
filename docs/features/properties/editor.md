# The property editor

`/editproperties` (Admin) creates, changes and deletes properties and doors in-game. Changes show at once, and are saved within 60 seconds and on shutdown.

## The menus

**Main list**
- **Create property.**
- **Door I'm standing at:** shown when you're at a door.
- **Every property,** for example `Little Lady (#3) - 2 doors`.

**Property menu**
- **Rename.**
- **Add a door here:** where you stand, facing your way. It puts you on the door, so inside an interior you're in the property's world.
- **Go to:** the first door.
- **Delete:** asks first, and deletes its doors too.
- **Its doors,** for example `Entrance (#7) -> Little Lady: Exit (#8)`.

**Door menu**
- **Go to.**
- **Move here:** to your spot, angle and interior.
- **Rename.**
- **Link to another door:** both ways. Doors that led back to either of the two no longer do.
- **Unlink:** both ways.
- **Delete:** asks first. Doors that led to it then lead nowhere.

Names are 1–64 characters.

## Tips

- **Getting into an interior:** use `/gotopos [x] [y] [z] [interior]`, for example to place an exit door.
- **Seeing a label:** "Add a door here" and "Move here" put you in the door's virtual world, so you see its label straight away.
- **Characters:** menu text uses plain characters (`->`, `-`). **Why:** SA-MP dialogs use the Windows-1252 encoding, which can't show symbols like `→`.

Want more? → [Properties](../properties.md) · [How it's built](internals.md)
