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
2. **Going through** puts you at the door it leads to: its spot, its angle, its interior and its virtual world.
   - **One link per pair.** Linking an entrance to its exit stores only "entrance → exit"; the exit has no link of its own, so it leads back to the door that links to it.
   - **Why:** seed data can't hold two doors that point at each other (EF Core can't insert such rows), so with one link per pair the links ship with the [default data](properties/default-data.md).
3. **Each door is outside or inside a property.** Outside doors are in the shared world (virtual world 0); a door inside a property is in that property's own [virtual world](../glossary.md#virtual-world).
   - **Why own virtual worlds:** two properties can use the same GTA interior without their visitors meeting.
   - **Why a setting instead of the interior number:** rooms built into the map at interior 0 (Palomino Bank) can be reused for another property, and a property can be inside another (an apartment off a building's hallway). The editor picks a sensible default, see [The property editor](properties/editor.md).
4. **Several doors can lead to the same door.**
   - **Why:** a building can have a front and a back entrance into one interior.
   - **Leading back:** an exit without a link of its own leads back to the lowest-numbered door that links to it. Give it a link of its own to choose another.
5. **On every startup,** the properties are exported as seed-data code. See [Default data export](properties/default-data.md).
6. **[Actors](actors.md) can be inside a property too,** in its virtual world. Deleting a property publishes `OnPropertyDeleted`, and the actors inside it move outside.

## Known limitations

- **Doors only:** no owners, buying, renting, locks, storage or furniture yet.
- **Plain transitions:** going through is instant, with no fade or freeze, and no event for other features.
- **Labels only:** no pickup or map icon.
- **Virtual worlds can change.** A property's virtual world can differ after a restart.

Want more?
- [The property editor](properties/editor.md)
- [Default data export](properties/default-data.md)
- [How it's built](properties/internals.md)
