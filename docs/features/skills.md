# Skills

Character skills with levels and experience, shown with `/skills`.

## How to use it

| Command | Groups | Does |
|---|---|---|
| `/skills` | Default | Lists your skills and progress; pick one for details |

## How it works

1. **Loading.** Skills are loaded onto the player when a character is selected.
2. **Experience.** Reaching a level takes **100 × level²** experience in total.
3. **Which skills exist** is up to the host, which seeds them.

## Known limitations

- **Nothing gives experience yet.** A gamemode can, for example on [`OnResourceHarvested`](../server-events.md#onresourceharvested).

## Deep dive

- **Code:** `Features/Skills/`.
- **Component:** `CharacterSkills`, on the player.
- **Record:** `CharacterSkill` (level, experience, last use).
- **Services:** `ISkillService` and `ISkillDialogService`.
- **Tables:** [`Skills`, `CharacterSkills`](../database.md#skills-skillmodel-and-characterskills-characterskillmodel).

Want more? → [Characters](characters.md)
