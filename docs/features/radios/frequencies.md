# Frequencies

`/editfrequencies` (Admin) sets which frequencies are protected or encrypted, and which factions may use them. Frequencies without rules follow the defaults: 1000 to 9999 are open to everyone, anything else has no signal.

## The editor

| Row | Does |
|---|---|
| Create a frequency | Asks for the number (1 to 99999), a name (at most 32 characters) and the mode |
| A frequency | Shows its number, name, mode and how many factions it has |

**A frequency's menu:**

| Row | Does |
|---|---|
| Rename | A new name, shown on the radio, for example `[Radio 911 (911 Dispatch)]` |
| Mode | Protected (everyone listens, its factions talk) or Encrypted (only its factions listen and talk) |
| Factions | Add a faction from the list of all factions, or pick one to remove it |
| Delete | Removes its rules: the number goes back to the defaults |

- **Sub-factions count:** adding a faction also lets every faction it controls in.
- **A frequency without factions** is listen-only when protected, and silent when encrypted.

## Example: a police channel

1. `/editfrequencies`, then **Create a frequency**: `100`, `LSPD Tactical`, Encrypted.
2. **Factions**, then **Add a faction**: the police faction.
3. Give officers a radio with `/giveradio`, and they tune with `/freq 100`.

**911 Dispatch** (frequency 911) is created at startup when [911](../emergency-calls.md) is on. Add the factions that may talk back on it.

Want more? → [Radios](../radios.md)
