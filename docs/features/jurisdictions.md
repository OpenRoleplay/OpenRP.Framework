# Jurisdictions

Places with their own rules: countries, and the states and cities in them. A jurisdiction has tags, a currency and a governing faction, and factions can be registered there for taxes. Properties lie in one, and vehicles are registered in one. Admins build them with `/editjurisdictions`.

**Why:** factions describe organisations, but nothing described the places they act in. Jurisdictions give the game "United States of San Andreas", its tags and currency, who is in charge there, and who pays taxes to it.

## How to use it

| Command | Groups | Does |
|---|---|---|
| `/editjurisdictions` | Admin | The jurisdiction editor |

To add a city:
1. `/editjurisdictions`, pick the country, and choose "Create a sub-jurisdiction".
2. Enter the name, then its three tags, for example `PC PCR PCRK`.

See [The jurisdiction editor](jurisdictions/editor.md).

## How it works

1. **A tree.** A jurisdiction without a parent is a country. Any depth is allowed: country, state, county, city.
   - **No loops:** a jurisdiction can't be moved into itself or into a place inside it.
   - **Deleting** is refused while other jurisdictions lie in it, or while another feature depends on it, such as a [bank](banking.md). Features add that check with `services.AddJurisdictionDeletionGuard<T>()` (an `IJurisdictionDeletionGuard`).
2. **Three tags,** like ISO country codes: 2, 3 and 4 letters, for example `SA`, `SAN` and `USSA`. Every jurisdiction has all three, cities too, and no two jurisdictions share a tag of the same length.
   - **Why:** tags identify a place in short text, such as licence plates now and bank account numbers later.
3. **Currency, inherited.** A jurisdiction without a currency of its own uses its nearest parent's.
4. **Government, inherited.** The government is the faction its Government field points to. Without one, the nearest parent's government governs it. The editor shows where it comes from, for example "Federal Government (from United States of San Andreas)".
   - **Why the jurisdiction points to the faction:** power belongs to a place, not to an organisation. A government can fall, or a party take power, without any faction changing.
   - **Changing it** publishes [`OnJurisdictionGovernmentChanged`](../server-events.md#onjurisdictiongovernmentchanged). Nothing reacts to it yet.
   - A faction's type `Government` is only a label now; it doesn't decide who governs.
5. **Tax registrations.** A faction can be registered in several jurisdictions. A sub-faction without registrations of its own uses its nearest parent faction's. Nothing collects taxes yet.
6. **Properties** lie in one jurisdiction or none, set in [`/editproperties`](properties/editor.md).
7. **Licence plates** start with the 2-letter tag of the **country** the vehicle is registered in, so a car registered in Palomino Creek gets `SA`. Unregistered vehicles get `SA`. Admins set it in the [vehicle editor](vehicles.md)'s "Registered in" row.

**Deleting a jurisdiction** removes its tax registrations, and its properties and vehicles have none afterwards ([`OnJurisdictionDeleted`](../server-events.md#onjurisdictiondeleted)).

## Known limitations

- **Storage only:** no tax collection, laws, citizenship or borders yet.
- **Admins only:** government leaders can't change their jurisdictions in-game yet.
- **Plates of other servers:** a tag change made on another server reaches this server's data within a minute, but plates only follow at the next restart or vehicle edit.

## Deep dive

- **Code:** `Features/Jurisdictions/`.
- **Components:** `Jurisdiction` and `FactionTaxRegistration`, loaded by their [BaseManagers](../persistence/base-manager.md) at startup and synced every 60 seconds.
- **Services:**
  - `IJurisdictionService`: reading: the tree, `GetCountry`, `GetGovernment`, `GetCurrencyId`, `GetJurisdictionsGovernedBy`, `GetTaxJurisdictions` and `GetPlateTag`;
  - `IJurisdictionEditorService`: every change; methods that can fail return an error message, or null;
  - `IJurisdictionDialogService`: the menus, and `OpenJurisdictionPicker` for other features' editors.
- **Events:** `OnJurisdictionsLoaded` and `OnJurisdictionPlateTagChanged` make [Vehicles](vehicles.md) set their plates again. **Why at startup:** vehicles may spawn before the jurisdictions are loaded.
- **Tables:** [`Jurisdictions` and `FactionTaxRegistrations`](../database.md#jurisdictions).
- **Seed data:** the Framework seeds none. The official server seeds United States of San Andreas (`SA`/`SAN`/`USSA`, San Andreas Dollar) and Caeroyna (`CR`/`CRY`/`CAER`, Caeroyna Ruble).

Want more? → [The jurisdiction editor](jurisdictions/editor.md) · [Factions](factions.md) · [Properties](properties.md)
