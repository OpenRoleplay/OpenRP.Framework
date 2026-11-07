# Characters

An account can have several characters. Each has its own name, skin, accent, inventory, money, skills and factions. This feature covers creating, selecting, spawning and saving them, plus their consent preferences.

## How to use it

| Command | Groups | Does |
|---|---|---|
| `/characterpreferences` | Default | Shows and changes your consent settings and Hardcore Mode |
| `/traits` | Default | Shows your character's traits |

## How it works

1. **Picking a character.** After login, you pick a character or create one. Quitting the selection kicks you.
2. **Selecting** loads the character onto the player, and publishes [`OnCharacterSelected`](../server-events.md). Many features react: permissions, money, skills, Discord.
3. **Spawning.** Your name becomes `First_Last`, your skin is applied, and you spawn at the default spawn.
4. **Saving.** Changes are saved every 60 seconds, and when you disconnect.

### Consent and Hardcore Mode

- **Consent settings:** each character says whether others may roleplay character kill, rape, slavery and world events with them.
- **Hardcore Mode** consents to everything:
  - it can't be turned off;
  - it locks the consent settings;
  - it earns the **Blessed by Misfortune** trait.

**Why:** heavy roleplay only happens to players who agreed to it. Stored consent also prevents disputes about what was allowed.

## Known limitations

- **One spawn point.** Everyone spawns at the same place; the last position isn't saved.
- **No death handling.** Dying uses open.mp's default respawn.

## Deep dive

- **Code:** `Features/Characters/`.
- **Components on the player:**
  - `Character`: name, skin, accent, inventory, `GetWallet()`;
  - `CharacterPreferences`;
  - `CharacterCreation`, while creating.
- **Services:**
  - `ICharacterService`: create, load, select, save;
  - `ICharacterDialogService`: selection, preferences, traits.
- **Systems:**
  - `CharacterSpawnServerSystem` handles the spawn;
  - `CharacterPersistenceSystem` saves.
- **The default spawn:** 2273.56, 82.37, 26.48, facing 358°, virtual world 0.
- **New characters:** `BaseDataContext` gives every new character an inventory.
- **Tables:** [`Characters`, `CharacterPreferences`](../database.md#characters).

Want more? → [Sessions](../persistence/sessions.md) · [Players](players.md) · [Currencies](currencies.md)
