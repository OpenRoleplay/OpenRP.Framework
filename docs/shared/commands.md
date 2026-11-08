# How commands work

Commands are methods with `[ServerCommand]` in an `ISystem` class. The list of commands is on [Commands](../commands.md).

## Writing a command

```csharp
public class CreateFactionCommand : ISystem
{
    [ServerCommand(PermissionGroups = new[] { "Admin" },
        Description = "Create a faction with a Leader rank.",
        CommandGroups = new[] { "Admin", "Factions" })]
    public void CreateFaction(Player player, IFactionMembershipService factionMembershipService, string name)
    {
        Faction faction = factionMembershipService.CreateFaction(name);
        player.SendPlayerInfoMessage(PlayerInfoMessageType.ADMIN, $"Created faction {faction.GetName()}.");
    }
}
```

**Parameters:**
- the first is the `Player`;
- typed arguments are `string`, `int`, `float`, `Player`, enums and the like;
- anything else is a service, filled in by [dependency injection](../glossary.md#dependency-injection);
- a `string` as the **last** argument takes the rest of the line;
- add an overload without arguments to show the syntax.

**Never use SampSharp's `[PlayerCommand]`.** **Why:** it skips the permission check.

## What happens when a player types one

1. The text must start with `/`.
2. The player must be playing a character.
3. The command must exist.
4. The character must have `cmd.<name>`. See [Permissions](../permissions.md).
5. The arguments are read. If they don't fit, the player sees the syntax: `[required]`, `<optional>`.

## Deep dive

- **Built on SampSharp's command system.** The Framework replaces three of its services:
  - `ServerCommandPermissionChecker` (`IPermissionChecker`): a character, with `cmd.<name>`;
  - `ServerCommandMessageService` (`IPlayerCommandMessageService`): the error and syntax messages;
  - `ServerCommandParameterParserFactory` (`ICommandParameterParserFactory`): the float parser below.

  `[ServerCommand]` inherits SampSharp's `[PlayerCommand]`, so SampSharp finds the commands.
- **Decimal numbers.** `float` arguments accept `.` and `,` (`InvariantFloatParser`). **Why:** the server runs in Dutch, where SampSharp's own parser reads `1234.56` as `123456`.
- **The command cache.** `ServerCommandCache` finds commands in the Framework, and in assemblies the host registers with `RegisterApplicationAssembly`.
- **`/help`** lists the commands you may use, grouped by `CommandGroups` (built by `ServerCommandGroupCache`).
- **The attribute** is in `Shared/Commands/Attributes/`. Its namespace is still `OpenRP.Framework.Features.Commands.Attributes`.

Want more? → [Commands](../commands.md) · [Permissions](../permissions.md)
