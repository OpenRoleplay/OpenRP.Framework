# Players

Four extension methods on `Player` (`Features/Players/`) that every feature uses to find the player's account and character.

| Method | Returns |
|---|---|
| `IsPlayerLoggedInAccount()` | Whether the player is logged in |
| `GetPlayerCurrentlyLoggedInAccount()` | The `Account` component |
| `IsPlayerPlayingAsCharacter()` | Whether the player has selected a character |
| `GetPlayerCurrentlyPlayingAsCharacter()` | The `Character` component |

Check the `Is…` method first: the `Get…` methods assume the account or character is there.

Want more? → [Accounts](accounts.md) · [Characters](characters.md)
