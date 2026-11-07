# Properties

Buildings and places with doors. A door can lead to another door, usually an entrance outside to an exit inside. Players go through with `/enter` and `/exit`; admins build everything in-game with `/editproperties`.

## How to use it

| Command | Groups | Does |
|---|---|---|
| `/enter`, `/exit` | Default | Go through the door you're standing at (within 3 units) |
| `/editproperties` | Admin | The property editor |

To build a house:
1. Create the property at the building, and add a door "Entrance".
2. `/gotopos` into the interior, and add a door "Exit".
3. Link Exit to Entrance.

See [The property editor](properties/editor.md).

## How it works

1. **Doors.** A door has a position, a facing angle, an [interior](../glossary.md#interior) and a name. Its label reads, for example, "Entrance of Little Lady".
2. **Going through** puts you at the linked door: its spot, its angle, its interior and its virtual world.
3. **Inside doors** use their property's own [virtual world](../glossary.md#virtual-world).
   - **Why:** two properties can use the same GTA interior without their visitors meeting.
4. **Several doors can lead to the same door.**
   - **Why:** a building can have a front and a back entrance into one interior.
5. **On every startup,** the properties are exported as seed-data code. See [Default data export](properties/default-data.md).

## Known limitations

- **Doors only:** no owners, buying, renting, locks, storage or furniture yet.
- **Plain transitions:** going through is instant, with no fade or freeze, and no event for other features.
- **Labels only:** no pickup or map icon.
- **Virtual worlds can change.** A property's virtual world can differ after a restart.

Want more?
- [The property editor](properties/editor.md)
- [Default data export](properties/default-data.md)
- [How it's built](properties/internals.md)
