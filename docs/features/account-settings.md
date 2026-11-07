# AccountSettings

Settings shared by all characters of an account: whether you see global OOC chat and newbie chat, and a graphics preset.

## How to use it

| Command | Groups | Does |
|---|---|---|
| `/settings` | Default | Opens the settings dialog; every change is saved at once |

## How it works

1. **Loading.** On login ([`OnAccountLoggedIn`](../server-events.md)), the settings are loaded onto the player. If the account has none yet, defaults are created.
2. **Chat.** `/ooc` and `/newbie` only reach players who have that chat turned on.
3. **Graphics preset.** It sets how many objects the streamer shows you, and how far away. It's applied on login and each time you switch it.

   | Preset | Visible objects | Radius × |
   |---|---|---|
   | Low | 500 | 1.0 |
   | Medium | 1000 | 1.0 |
   | High | 2000 | 1.0 |
   | Very High | 2000 | 1.5 |
   | Ultra | 2000 | 2.0 |

## Known limitations

- **Lowering the preset** from Ultra to Low only takes effect after you relog.

## Deep dive

- **Code:** `Features/AccountSettingsFeature/`.
- **Component:** `AccountSettings`, on the player.
- **Service:** `IAccountSettingsService`: `ReloadAccountSettings`, `GetAccountSettings` and `OpenAccountSettingsDialog`.
- **Table:** [`AccountSettings`](../database.md#accountsettings-accountsettingsmodel), one row per account.

Want more? → [RoleplayChats](roleplay-chats.md) · [Accounts](accounts.md)
