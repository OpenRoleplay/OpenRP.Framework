# RoleplayChats

The chat commands: speech and actions in character (IC), and out-of-character (OOC) chat.

## How to use it

| Command | Shows | Heard within |
|---|---|---|
| plain text, `/say`, `/talk` | `Name says: text` (+ `[accent]`) | 7 units, muffled from 5 |
| `/low`, `/l` | `Name says quietly: text` | 3 units |
| `/shout`, `/s` | `Name shouts: text` | 30 units |
| `/r` | `Name says (radio): text`, and the text goes over your [radio](radios.md) | 7 units, muffled from 5 |
| `/me` | `* Name text *` | 20 units |
| `/ame` | `* Name text *` above your head | 20 units |
| `/do` | `text (( Name ))` | 20 units |
| `/my` | `* Name's text` | 20 units |
| `/b` | `(( L-OOC \| Name: text ))` | 20 units |
| `/ooc`, `/o` | `(( OOC \| Name: text ))`, also on [Discord](discord.md) | everyone with OOC on |
| `/newbie`, `/n` | `(( Newbie \| Name [id]: text ))`, also on Discord | everyone with newbie chat on |
| `/accent <accent>` | sets your accent; empty removes it | |

All are in the Default group. Players turn OOC and newbie chat off in [`/settings`](account-settings.md).

**In a phone call,** talk, `/low` and `/shout` show what you talk into: `Name says (phone): text`, `Name says quietly (phone): text`, `Name shouts (phone): text`.

## Known limitations

- **Unused types.** `WHISPER` and `MEGAPHONE` exist but have no command.

## Deep dive

- **Code:** `Features/RoleplayChats/` holds the commands, and `Shared/Chat` the formatting. See [Chat](../shared/chat.md).
- **Plain text** goes through `ChatSystem.OnPlayerText`, which sends it as talk.

Want more? → [Chat](../shared/chat.md) · [Discord](discord.md)
