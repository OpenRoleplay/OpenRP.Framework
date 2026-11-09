# The actor editor

`/editactors` (Admin) creates, changes and deletes actors in-game. Changes show at once, and are saved within 60 seconds and on shutdown.

## The menus

**Main list**
- **Create actor here:** where you stand, facing your way, with your skin. It's inside the property whose virtual world you're in, otherwise outside.
- **Actor nearest to me:** shown when an actor is within 3 units, in your virtual world.
- **Every actor,** for example `Finn O'Hare (#1) - outside` or `Bartender (#7) - inside Little Lady`. Main menu scene actors aren't listed.

**Actor menu**
- **Go to:** puts you in front of the actor, in its interior and virtual world.
- **Move here:** to your spot, angle and interior. Which property it's inside is picked again, as for a new actor.
- **Rename.**
- **Skin:** a number from 0 to 311, except 74 (it doesn't exist in GTA, and crashes clients).
- **Animation:** none (just stand), a preset, or custom (library and name).
- **Virtual world:** outside (the shared world), or inside any property. Puts you at the actor afterwards, so you still see it.
- **AI prompt:** read-only, over pages. Prompts are changed in the database (`Actors.Prompt`).
- **Delete:** asks first.

Names are 1 to 64 characters.

## Animation presets

| Preset | Library / animation |
|---|---|
| Sit on the ground | `BEACH` / `ParkSit_M_loop` |
| Lean on a wall | `GANGS` / `leanIDLE` |
| Smoke against a wall | `SMOKING` / `M_smklean_loop` |
| Look around | `COP_AMBIENT` / `Coplook_loop` |
| Injured against a wall | `SWAT` / `gnstwall_injurd` |
| Sit at a table | `INT_OFFICE` / `OFF_Sit_Idle_Loop` |
| Lean on a bar | `BAR` / `Barcustom_loop` |
| Chat | `PED` / `IDLE_chat` |
| Dealer | `DEALER` / `DEALER_IDLE` |
| Wash up | `INT_HOUSE` / `wash_up` |

They're in `Helpers/ActorAnimationPresets`.

## Tips

- **An actor in a house:** go through the house's door first (or "Go to" one of its inside doors in `/editproperties`), then "Create actor here". It's inside the house, so only its visitors see it.
- **An actor that talks:** create it in-game, then add its prompt in the database. It answers players once the change is loaded (within 60 seconds).

Want more? → [Actors](../actors.md) · [Properties](../properties.md)
