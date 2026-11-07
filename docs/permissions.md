# Permissions

A player may use a command when their character has its permission, for example `cmd.pay` for `/pay`. A character gets permissions from **permission groups** and from their **factions**.

## Permission groups

| Group | Who has it | Contains |
|---|---|---|
| `Default` | every character, automatically | commands for everyone |
| `Admin` | given with `/givepermissiongroup` | admin commands |
| `Tester` | given with `/givepermissiongroup` | test commands such as `/sv` |
| `Faction` | nobody directly | what every faction has by default: `/fo`, `/fooc`, `faction.*` |

A command picks its groups in code, for example `[ServerCommand(PermissionGroups = new[] { "Admin" }, …)]`. At startup the server creates `cmd.<name>` and puts it in those groups.

**Why in code:** the code decides who may use a new command, so nobody has to edit the database. Links an admin adds by hand are left alone, so a server can still give extra groups access.

## Faction permissions

- **A faction** has the `Faction` group, plus what's granted to it:
  - top-level factions get grants from admins (`/factionpermission`);
  - sub-factions get them from their parent's leaders, only as far as the parent has them.
- **A rank** has its grants, as far as its faction has them. **The top rank has everything its faction has.**
- **A character** gets all permissions of all their ranks. Changes apply straight away.

**Why the `Faction` group:** every faction needs faction chat and the means to manage itself. Admins only grant what goes beyond that.

**Why the top rank has everything:** leaders run their own faction without admins, and a faction can never lock itself out.

## Commands

| Command | Groups | Does |
|---|---|---|
| `/permissions` | Default | Shows your permissions |
| `/showpermissions [target]` | Admin | Shows someone else's |
| `/givepermissiongroup [player] [group]` | Admin | Gives a group, effective at once |

## Deep dive

- **How a command is checked:** `PlayerServerCommandService` checks that the player plays a character, that the command exists, and that the character has `cmd.<name>`. Each failure has its own message.
- **Where the permissions live:** a character's permissions are loaded into the `CharacterPermissions` component on character selection, and on [`OnFactionPermissionsChanged`](server-events.md#onfactionpermissionschanged). Changes made straight in the database apply after the next selection.
- **Gamemode-managed links:** links made from attributes are marked `GamemodeManaged`. Only those are added or removed automatically.
- **Permission sources:** `ICharacterPermissionSource.GetPermissions(characterId)` lets a feature add permissions. Factions is one. Register your own as `ICharacterPermissionSource`.
- **`IPermissionService`** creates permissions, adds them to groups, gives groups and reloads a character.
- **In code:** `character.GetComponent<CharacterPermissions>().DoesCharacterHavePermission("name")`.
- **Code:** `Features/Permissions/`. **Tables:** [Permissions](database.md#permissions).

**Known limitations**
- Groups can't be removed or listed in-game.
- The startup sync writes to the console rather than the log.

Want more? → [Factions](features/factions.md) · [Commands](commands.md)
