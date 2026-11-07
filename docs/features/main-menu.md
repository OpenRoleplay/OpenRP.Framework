# MainMenu

What a player sees between connecting and playing: a living scene in the background, the login/register choice, then the character selection.

**Why a scene:** it shows new players what Open Roleplay is about before they even log in, and shows off the AI actors.

## How it works

1. **Connecting.** A connecting player is put in a scene, and gets the main menu.
2. **Login or register,** handled by [Accounts](accounts.md).
3. **After login,** the [character selection](characters.md) opens. Going back returns to step 2.
4. **When a character is picked,** the player leaves the scene: music and conversations stop.

Each scene runs in its own [virtual world](virtual-worlds.md).

| Scene | Contents |
|---|---|
| `FamilyBarbecueScene` | A family barbecue with actors, music from the [CDN](cdn.md), and an [AI conversation](actor-conversations.md) between actors 2, 3 and 4 |

## Add your own scene

Implement `IMainMenuScene`:

| Method | Does |
|---|---|
| `Prepare()` | One-time setup at startup |
| `Play(player)` | Shows the scene: camera, world, music |
| `Leave(player)` | Stops what the scene started for the player. Safe to call for a player who isn't watching |

## Known limitations

- **One scene, fixed actors.** It needs actors 2, 3 and 4 to be seeded by the host.

## Deep dive

- **Code:** `Features/MainMenu/`.
- **Service:** `IMainMenuDialogService`.
- **Systems:**
  - `MainMenuSystem`, at `OnGameModeInit` and `OnPlayerRequestClass`;
  - `MainMenuServerSystem`, on login and character selection.

Want more? → [Accounts](accounts.md) · [Characters](characters.md)
