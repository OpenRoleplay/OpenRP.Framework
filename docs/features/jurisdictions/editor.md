# The jurisdiction editor

`/editjurisdictions` (Admin) creates, changes and deletes jurisdictions and tax registrations in-game. Changes are saved within 60 seconds and on shutdown, and reach other servers within a minute after that.

## The menus

**Main list**
- **Create a country.**
- **Every jurisdiction** as a tree, each with its tags, for example `Palomino Creek (PC/PCR/PCRK)` indented under its country.

**Jurisdiction menu**
- **Rename.**
- **Tags:** the 2-, 3- and 4-letter tags, entered separated by spaces, for example `SA SAN USSA`. Letters only; lower case is turned into upper case. A tag used by another jurisdiction is refused.
- **Currency:** pick one, or none to use the parent's. Shows "(from a parent)" when inherited.
- **Lies in:** move it into another jurisdiction, or make it a country. Moving it into itself or a place inside it is refused.
- **Governed by:** pick a faction, or none to use the parent's government. Shows "(from ...)" when inherited.
- **Tax registrations:** the factions registered here. "Register a faction" adds one (refused when it's registered already); picking a faction asks to remove it.
- **Create a sub-jurisdiction:** asks for the name, then the tags.
- **Delete:** asks first. Refused while other jurisdictions lie in it. Its properties and vehicles have no jurisdiction afterwards.

Names are 1 to 64 characters.

## Tips

- **Plates follow the country.** Changing a country's 2-letter tag, or moving a city to another country, changes the plates of the vehicles registered there.
- **Properties and vehicles** are linked from their own editors: `/editproperties` (the "Jurisdiction" row) and `/editvehicle` (the "Registered in" row).

Want more? → [Jurisdictions](../jurisdictions.md)
