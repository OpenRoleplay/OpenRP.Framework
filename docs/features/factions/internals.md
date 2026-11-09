# Factions: internals

How factions are built (`Features/Factions/`). For what they do, see [Factions](../factions.md).

## The /faction menu

If you can open several factions, `/faction` first asks which. Then, depending on your permissions:
- **Members:** change their rank, or remove them.
- **Ranks:** add, rename, move or delete, and set rank permissions. A rank can only be deleted when it has no members and no sub-ranks.
- **Invite a player.**
- **Sub-factions** and their permissions.
- **Leave,** or choose your active faction.

The code is in `Services/Dialogs/FactionDialogService.*.cs`, one file per flow: Menu, Members, Ranks, Invites and Permissions.

## Components

All five are loaded by [BaseManagers](../../persistence/base-manager.md) and synced every 60 seconds.

| Component | Holds |
|---|---|
| `Faction` | name, parent, type (`None`/`Government`) |
| `FactionRank` | faction, parent rank, name |
| `FactionMember` | character, faction, rank, active |
| `FactionPermission` | a grant to a faction |
| `FactionRankPermission` | a grant to a rank |

`PlayerFactionInvitation` (on the player) is a pending invitation.

## Services

| Service | Responsibility |
|---|---|
| `IFactionService` | The faction tree: `GetParentFaction`, `GetChildFactions`, `GetTopFaction`, `IsControlledBy` |
| `IFactionRankService` | The rank tree: create, rename, move, delete |
| `IFactionMembershipService` | Members, leaders, invitations, active faction, granting |
| `IFactionPermissionService` | What factions and ranks have. Also an `ICharacterPermissionSource` |
| `IFactionChatService` | `/fooc` |
| `IFactionDialogService` | The menus |
| 5 managers | Persistence, with `CreateAndLoad` |

Methods that can fail return an error message, or null.

## How permissions are resolved

1. **A faction has:** the `Faction` group, plus its grants. A sub-faction's grants only count while its parent has them too.
2. **The top rank has:** everything its faction has.
3. **Other ranks have:** their grants, as far as the faction has them.
4. **On every change,** [`OnFactionPermissionsChanged`](../../server-events.md#onfactionpermissionschanged) is published for the members affected.

`FactionSystem` loads everything at startup, syncs every 60 seconds, and saves on shutdown.

## Tables

[Factions](../../database.md#factions)

Want more? → [Factions](../factions.md) · [Permissions](../../permissions.md)
