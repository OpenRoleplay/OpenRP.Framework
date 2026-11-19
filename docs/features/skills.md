# Skills

Character skills with levels and experience, shown with `/skills`.

## How to use it

| Command | Groups | Does |
|---|---|---|
| `/skills` | Default | Lists your skills and progress; pick one for details |

## How it works

1. **Loading.** Skills are loaded onto the player when a character is selected.
2. **Experience.** Reaching a level takes **100 × level²** experience in total. The maximum is **level 100**, at 1,000,000 experience. Experience above that isn't kept, and `/skills` shows the skill as `Max`.
3. **Gaining experience.** `ISkillService.AddExperience` adds experience to a skill by name, saves it and raises the level. [Fishing](fishing.md) uses it.
4. **Which skills exist** is up to the host, which seeds them.

## Known limitations

- **Only fishing gives experience so far.** A gamemode can add more, for example on [`OnResourceHarvested`](../server-events.md#onresourceharvested).

## Deep dive

- **Code:** `Features/Skills/`.
- **Component:** `CharacterSkills`, on the player.
- **Record:** `CharacterSkill` (level, experience, last use).
- **Services:** `ISkillService` (`LoadCharacterSkills`, `AddExperience`) and `ISkillDialogService`.
- **Tables:** [`Skills`, `CharacterSkills`](../database.md#skills-skillmodel-and-characterskills-characterskillmodel).

Want more? → [Characters](characters.md)
