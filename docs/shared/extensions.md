# Extensions and colours

Small shared helpers.

| Where | What |
|---|---|
| `Shared/ChatColor.cs` | Colour codes inside text: `White`, `Highlight`, `Main`, `Red`, `LightGray`, … |
| `Shared/ServerColor.cs` | `Color` values, such as `RoleplayActionColor` (the colour of `/me`) |
| `Shared/Extensions/StringExtensions.cs` | `LowercaseFirstLetter()` |
| `Shared/Extensions/VectorExtensions.cs` | `vector.XY`: the X and Y of a `Vector3`, like SampSharp 0.x |
| `Shared/Extensions/PlayerPositionExtensions.cs` | `player.GetPointInFront(distance)`: the point in front of the player, in the direction they face |
| `Shared/Chat/Extensions/StringExtensions.cs` | `Muffle(…)`: turns part of a text into dots, for far-away speech |
| `Shared/Dialogs/Extensions/StringExtensions.cs` | `InsertNewlinesAtLength()` wraps long dialog text. `GetVisibleLength()` counts the length without colour codes |
| `Shared/FastNoiseLite.cs` | A noise library, used for weather and biomes |

The player helpers (`IsPlayerPlayingAsCharacter()` and the rest) are in [Players](../features/players.md).

Want more? → [Chat](chat.md)
