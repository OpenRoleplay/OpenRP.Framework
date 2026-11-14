# Jurisdiction settings

A setting is a value that a jurisdiction can have of its own, such as the [bank account fee cap](../banking.md). Government leaders change settings with `/government`, and admins change them in `/editjurisdictions`.

## How to use it

**Government leaders:** everyone can type `/government`, but it only lists the jurisdictions your faction governs. With one jurisdiction it opens straight away; with several, you pick one first. Leaders of a faction above the governing one count too.

The settings table has a row per setting:

| Column | Shows |
|---|---|
| Setting | The setting's name |
| Value | The value in effect, and where it comes from: "(from …)" when inherited, naming the jurisdiction that supplies it; "(limited by …)" when the parent limits it; "(default)" when nothing is set |
| Sub-jurisdictions | For a country, "Bound" or "Free". For any other place, "Bound by country" or "-" |

Choose a row to change it:

| Action | Shown when | Does |
|---|---|---|
| Change value | always | Asks for a new value |
| Use default, or Use parent's value | the place has a value of its own | Removes that value, so the place inherits again |
| Bind sub-jurisdictions: On or Off | the place is a country | Turns binding on or off |

The leader check runs again before every change, so a leader who loses the government while a menu is open is turned away.

**Admins** open the same table from `/editjurisdictions`: pick a jurisdiction, then **Settings**. Admins can change any jurisdiction, with no leader check.

## How settings resolve

A place's value in effect comes from the first of these that applies:

1. **Own value.** A place with a value of its own uses it.
2. **Inherited.** Otherwise it uses the value of its nearest parent that has one.
3. **Default.** When nothing above it has one, the setting's default applies.

**"(from …)"** names the jurisdiction the value really comes from. Usually that's the nearest ancestor with a value of its own. If that ancestor is limited by a higher one, the name is the higher one, because it sets the value. For example, a city has no value of its own. Its state has 150, but its country binds it to 100. The city's value is 100, and the label names the country.

**Binding.** A country can bind its sub-jurisdictions to its values, one setting at a time.

**Why:** different values need different answers. A country might hold every state to its bank fee cap, but let them choose freely on other values.

Each place below a bound country is held to the value of its parent:

- **Limited on save:** a place can't save a value beyond its parent's. For the fee cap, a higher cap is refused, and the message names the parent's cap.
- **Limited on read:** if the parent later lowers its value, the place keeps its own value but uses the lower one, shown as "(limited by …)". When the parent raises its value again, the place goes back up toward its own value.

**Why:** the limit applies when read, not saved into the child. A country that lowers and later raises its cap doesn't wipe out what its states chose: their own values come back.

A country's own value is never limited. A bound place under a country with no value of its own is held to the default.

**Example:** a country binds its sub-jurisdictions and sets the fee cap to 200. A city sets 150. Later the country lowers the cap to 100: the city keeps 150 stored but uses 100. When the country raises the cap to 200, the city uses 150 again.

## Known limitations

- **One setting so far:** the bank account fee cap.
- **No event on change:** changing a value publishes no server event, so features read a value when they need it.

## Deep dive

- **Code:** `Features/Jurisdictions/`: `Entities/JurisdictionSettingDefinition.cs`, `Services/JurisdictionSettingService.cs`, `Commands/GovernmentCommand.cs` and `Services/Dialogs/JurisdictionDialogService.Settings.cs`.
- **Services:** `IJurisdictionSettingService`:
  - `GetValue` and `GetState` read a value, and where it comes from;
  - `SetValue`, `ClearValue` and `SetBindsSubJurisdictions` change one; a refused change returns the reason, and null when it worked;
  - `CanGovern` is true for the leader of the faction that governs the place, or of a faction above it. The governing faction is inherited, as in [Jurisdictions](../jurisdictions.md).
- **Component:** `JurisdictionSetting`, one per row. `JurisdictionSettingManager` loads them at startup and saves changes with the jurisdiction sync, every 60 seconds and on shutdown.
- **Table:** [`JurisdictionSettings`](../../database.md#jurisdictions).

Want more? → [Adding a setting](settings/adding-a-setting.md) · [Jurisdictions](../jurisdictions.md) · [Banking](../banking.md)
