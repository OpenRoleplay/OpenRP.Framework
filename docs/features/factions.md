# Factions

Player organisations, such as police, gangs, companies and governments. Each faction has ranks, members and permissions, and can have sub-factions. Leaders run their faction themselves in `/faction`; admins only set it up.

**Why:** real organisations nest (a government with a police department with units), and players should run them without needing admins.

## How to use it

| Command | Groups | Does |
|---|---|---|
| `/faction` | Default | Your faction menu: members, ranks, invitations, sub-factions |
| `/fooc`, `/fo [text]` | Faction | OOC message to the online members of your active faction |
| `/createfaction [name]` | Admin | Creates a faction with a "Leader" rank |
| `/setfactionleader [player] [faction id]` | Admin | Makes someone a leader |
| `/factionpermission [faction id] [permission]` | Admin | Grants or removes a permission of a top-level faction |

## How it works

1. **The faction tree.** A faction can have a parent faction. Leaders of a faction above can manage the ones below. The type `Government` is only a label; which faction governs a place is set on the [jurisdiction](jurisdictions.md).
2. **Ranks** form a tree inside each faction.
   - The rank at the top is the leader rank.
   - You manage ranks below your own, if you have `faction.manageranks`.
3. **Members.** A character can be in several factions, with one rank in each. With several, they choose an **active** faction for `/fooc`.
4. **Invitations.** With `faction.invite`, you invite a player at a rank below your own. They have **60 seconds** to accept.
   - **Why:** an invitation is meant to be answered on the spot, in person.
5. **Permissions.**
   - **What every faction has:** `faction.invite`, `faction.remove`, `faction.setrank`, `faction.manageranks`, `faction.bank` (use and open the faction's [bank accounts](banking/accounts.md)), `/fo` and `/fooc`.
   - **What leaders have:** everything their faction has.
   - **How it reaches members:** members get their rank's permissions, as command permissions, effective at once.

   See [Permissions](../permissions.md#faction-permissions).

## Known limitations

- **Database-only changes.** Factions can't be renamed, deleted, retyped or moved in-game.
- **No faction gameplay yet:** no vehicles, properties, duty, radio or police tools.

Want more? → [How it's built](factions/internals.md) · [Permissions](../permissions.md) · [Jurisdictions](jurisdictions.md)
