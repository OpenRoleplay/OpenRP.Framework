# Database

Every table the Framework defines, grouped by feature. Use it to look up a table or column.

The code for these tables lives in three places:
- the models in `Database/Models/`;
- the `DbSet`s in `BaseDataContext`;
- the rules (keys, lengths, relationships) in each feature's `Models/Configuration(s)` folder.

**Common columns.** Every model inherits `BaseModel`:

| Column | Type | Meaning |
|---|---|---|
| `Id` | `ulong` | Primary key, auto-increment |
| `CreatedOn` | `DateTime` (UTC) | Set on insert |
| `UpdatedOn` | `DateTime` (UTC) | Set on insert and update |

The [BaseManager sync](persistence/base-manager.md) relies on `CreatedOn` and `UpdatedOn`.

**Notation.** "→ X" means a foreign key to table X. "[cascade](glossary.md#cascade)" or "set null" is what happens to the row when X is deleted.

The [host](glossary.md#host) owns the actual schema and its migrations; see [Seed data and migrations](persistence/seed-data-and-migrations.md).

## Accounts

### `Accounts` (`AccountModel`)
| Column | Type | Meaning |
|---|---|---|
| `Username` | string | Login name |
| `Password` | string | BCrypt hash |
| `Level`, `Experience` | uint | Account level (not used by Framework features yet) |
| `IsOnline` | bool | Logged in on some server right now ([one login per account](persistence/sessions.md)) |
| `LastSeenAt` | DateTime? | Last time the server holding the login confirmed it |

### `AccountSettings` (`AccountSettingsModel`)
One row per account, → `Accounts` (cascade).

| Column | Type | Meaning |
|---|---|---|
| `AccountId` | ulong | The account |
| `AccountGraphicPreset` | enum | Low, Medium, High, VeryHigh, Ultra |
| `GlobalChatEnabled` | bool | Default true |
| `NewbieChatEnabled` | bool | Default true |

## Characters

### `Characters` (`CharacterModel`)
| Column | Type | Meaning |
|---|---|---|
| `FirstName`, `LastName` | string(35) | Name |
| `MiddleName` | string(30)? | Optional middle name |
| `DateOfBirth` | DateTime | |
| `Accent` | string(30)? | Shown in talk messages, for example "[French accent]" |
| `Skin` | int | Skin model, default 26 |
| `InventoryId` | ulong? | → `Inventories` (cascade). Every new character gets one automatically. |
| `AccountId` | ulong? | → `Accounts` |

### `CharacterPreferences` (`CharacterPreferencesModel`)
| Column | Type | Meaning |
|---|---|---|
| `CharacterId` | ulong | → `Characters` |
| `HardcoreMode` | bool | Consents to everything; can't be turned off |
| `AllowCharacterKill`, `AllowRape`, `AllowSlavery`, `AllowWorldEvents` | bool | Consent settings |
| `DefaultCurrencyId` | ulong | The currency shown as GTA money |

### `Skills` (`SkillModel`) and `CharacterSkills` (`CharacterSkillModel`)
- **`Skills`** has `Name` and `Description`. The skills themselves are seeded by the host.
- **`CharacterSkills`** has `CharacterId` and `SkillId`, plus `Level`, `Experience` and `LastUsedDate`.

## Permissions

| Table | Columns | Notes |
|---|---|---|
| `Permissions` (`PermissionModel`) | `Name` (unique), `Description` | `cmd.<command>`, `faction.*`, … |
| `PermissionGroups` (`PermissionGroupModel`) | `Name` (max 100) | Seeded: Default, Admin, Tester, Faction |
| `PermissionGroupPermissions` (`PermissionGroupPermissionModel`) | `PermissionGroupId` → groups (cascade), `PermissionId` → permissions (cascade), `GamemodeManaged` | `GamemodeManaged` links are synced from `[ServerCommand]` attributes |
| `CharacterPermissionGroups` (`CharacterPermissionGroupModel`) | `CharacterId` → characters (cascade), `PermissionGroupId` → groups (cascade) | |

See [Permissions](permissions.md).

## Factions

| Table | Columns | Notes |
|---|---|---|
| `Factions` (`FactionModel`) | `Name`, `ParentFactionId` → `Factions`, `Type` (None, Government) | Factions form a tree |
| `FactionRanks` (`FactionRankModel`) | `FactionId` → factions (cascade), `ParentRankId` → ranks (cascade), `Name` (max 64) | The rank without a parent is the top (leader) rank |
| `FactionMembers` (`FactionMemberModel`) | `CharacterId` → characters (cascade), `FactionId` → factions (cascade), `FactionRankId` → ranks (cascade), `IsActive` | Unique per character + faction. `IsActive` marks the faction that faction chat goes to |
| `FactionPermissions` (`FactionPermissionModel`) | `FactionId` → factions (cascade), `PermissionId` → permissions (cascade) | A faction's grants. Unique per faction + permission |
| `FactionRankPermissions` (`FactionRankPermissionModel`) | `FactionRankId` → ranks (cascade), `PermissionId` → permissions (cascade) | A rank's grants. Unique per rank + permission |

See [Factions](features/factions.md).

## Phones

| Table | Columns | Notes |
|---|---|---|
| `Phones` (`PhoneModel`) | `Number` (unique), `IsOn`, `IsSilent`, `BatteryLevel` (percent), `BatteryUpdatedAt` (nullable) | Random 7-digit numbers; below 1000 is reserved for the server |
| `PhoneContacts` (`PhoneContactModel`) | `PhoneId` → phones (cascade), `Name` (24), `Number` | The phone book. Unique per phone + number; at most 50 per phone |
| `PhoneMessages` (`PhoneMessageModel`) | `PhoneId` → phones (cascade), `OtherNumber`, `Text` (160), `IsIncoming`, `IsRead`, `SentAt` | Each phone keeps its own copy; the last 20 per phone |
| `PhoneCallLogs` (`PhoneCallLogModel`) | `PhoneId` → phones (cascade), `OtherNumber`, `Type` (Incoming, Outgoing, Missed), `StartedAt`, `DurationSeconds`, `IsSeen` | The last 20 per phone |

See [Phones](features/phones.md).

## Radios

| Table | Columns | Notes |
|---|---|---|
| `Radios` (`RadioModel`) | `Frequency`, `IsOn`, `UsesEarpiece`, `BatteryLevel` (percent), `BatteryUpdatedAt` (nullable), `VehicleId` → `Vehicles` (cascade, unique) | A handheld (its item points to it) or, with `VehicleId`, a vehicle radio |
| `RadioFrequencies` (`RadioFrequencyModel`) | `Number` (unique), `Name` (32), `Mode` (Protected, Encrypted) | Only frequencies with their own rules; others follow the defaults |
| `RadioFrequencyFactions` (`RadioFrequencyFactionModel`) | `RadioFrequencyId` → frequencies (cascade), `FactionId` → factions (cascade) | The factions on a frequency. Unique per frequency + faction |

See [Radios](features/radios.md).

## Jurisdictions

| Table | Columns | Notes |
|---|---|---|
| `Jurisdictions` (`JurisdictionModel`) | `Name` (64), `TwoLetterTag` (2), `ThreeLetterTag` (3), `FourLetterTag` (4), `ParentJurisdictionId` → `Jurisdictions` (restrict), `CurrencyId` → `Currencies` (set null), `GovernmentFactionId` → `Factions` (set null) | A tree; no parent = a country. Each tag is required and unique. Null currency or government = the parent's |
| `FactionTaxRegistrations` (`FactionTaxRegistrationModel`) | `FactionId` → factions (cascade), `JurisdictionId` → jurisdictions (cascade) | Where a faction pays taxes. Unique per faction + jurisdiction |

See [Jurisdictions](features/jurisdictions.md).

## Items, inventories and money

### `Items` (`ItemModel`)
The item types (definitions), not the items players own.

| Column | Type | Meaning |
|---|---|---|
| `Name` | string(255) | |
| `Description` | string? | |
| `UseType` | `ItemType` | Skin, Wallet, Currency, Attachment, VehicleKey, Misc, Fish, Phone, Radio, RadioEarpiece |
| `UseValue` | string? | Meaning depends on `UseType`, for example the skin id or currency unit id |
| `Weight` | uint | Grams per unit |
| `MaxUses` | uint? | Uses before it's used up |
| `KeepOnDeath`, `CanDrop`, `CanDestroy`, `Stackable` | bool | |
| `DropModelId` | int? | Object model shown when dropped |

The Framework seeds 51 items:
- a uniform (skin);
- a wallet;
- the bills and coins of the San Andreas Dollar and the Caeroyna Ruble;
- a vehicle key;
- the hotwire tools (rusty paperclip, worn-out screwdriver, roll of electrical tape);
- a chicken egg;
- twelve fish species;
- a mobile phone (item 49, `UseType` Phone). Each phone item points to its row in `Phones` with `[PHONE_ID=x]`;
- a handheld radio (item 50, `UseType` Radio), pointing to its row in `Radios` with `[RADIO_ID=x]`, and a radio earpiece (item 51).

### `Inventories` (`InventoryModel`)
`Name`, `MaxWeight` (grams, null = unlimited). Seeded: "World Inventory", which holds items lying on the ground.

### `InventoryItems` (`InventoryItemModel`)
An item someone or something owns.

| Column | Type | Meaning |
|---|---|---|
| `ItemId` | ulong | → `Items` (cascade) |
| `InventoryId` | ulong | → `Inventories` |
| `Amount` | uint | Stack size |
| `UsesRemaining` | uint? | |
| `KeepOnDeath`, `CanDrop`, `CanDestroy` | bool | Copied from the item, can differ per stack |
| `AdditionalData` | string? | Key/value data, for example `DEFAULT_WALLET`, `WEARING` or the vehicle id of a key. See [Inventories](features/inventories.md) |
| `Weight` | uint? | Overrides the item weight, for example for a fish |

An item that holds an inventory (a wallet) links to that inventory through its additional data.

### `DroppedInventoryItems` (`DroppedInventoryItemModel`)
`InventoryItemId` → `InventoryItems` (cascade, one-to-one), `PosX/Y/Z`, `RotX/Y/Z`. The inventory item itself sits in the world inventory.

### `Currencies` (`CurrencyModel`) and `CurrencyUnits` (`CurrencyUnitModel`)
- **`Currencies`** has `CurrencyCode` and `Name` (max 100). Seeded: SAD (San Andreas Dollar), CRU (Caeroyna Ruble), USD (United States Dollar).
- **`CurrencyUnits`** has `CurrencyId` → `Currencies` (cascade) and `UnitValue` (decimal), for example 20.00. 29 units are seeded. Each bill or coin is an item whose `UseValue` points to its unit.

## World

### `PropertyDoors`
Table `PropertyDoors` (`PropertyDoorModel`).

| Column | Type | Meaning |
|---|---|---|
| `Name` | string | |
| `PropertyId` | ulong? | → `Properties` (cascade) |
| `LinkedToPropertyDoorId` | ulong? | → `PropertyDoors` (set null). The door this one leads to. Not unique: several doors may lead to one door |
| `X`, `Y`, `Z`, `Angle` | float | Position and facing |
| `Interior` | int? | The GTA interior; null or 0 = the open world's |
| `InsidePropertyId` | ulong? | → `Properties` (set null). The property whose virtual world the door is in; null = outside, in the shared world |

### `Properties`
Table `Properties` (`PropertyModel`): `Name`, `JurisdictionId` → `Jurisdictions` (set null; null = none).

### `Vehicles` (`VehicleModel`)
Persistent vehicles.

| Column | Type | Meaning |
|---|---|---|
| `ModelId` | ulong? | → `VehicleModels` (set null) |
| `ModelSpawnTypeId` | ulong? | → `VehicleModelSpawnTypes` (set null). Spawn with a random model of this type |
| `X`, `Y`, `Z`, `Rotation` | float | Parking spot |
| `Color1`, `Color2` | int | |
| `IsEngineOn`, `AreLightsOn`, `IsAlarmOn`, `AreDoorsLocked`, `IsBonnetOpen`, `IsBootOpen` | bool | State |
| `Is…DoorOpen`, `Is…WindowClosed` | bool | Per door and window |
| `CurrentHealth`, `MaxHealth` | float | |
| `ServerVehicle` | bool | Belongs to the server, not a player |
| `OwnerCharacterId` | ulong? | → `Characters` |
| `JurisdictionId` | ulong? | → `Jurisdictions` (set null). Where it's registered; its plate starts with that country's tag. Null = none, an `SA` plate |
| `LoadedAs` | int? | |

### Vehicle model lookups
| Table | Columns |
|---|---|
| `VehicleModels` (`VehicleModelModel`) | `ModelId` (GTA model id), `Name` |
| `VehicleModelDefaultColors` (`VehicleModelDefaultColorModel`) | `VehicleModelId` → `VehicleModels` (cascade), `Color1`, `Color2` |
| `VehicleModelSpawnTypes` (`VehicleModelSpawnTypeModel`) | `Name` |
| `VehicleModelSpawnTypeModels` (`VehicleModelSpawnTypeModelModel`) | `VehicleSpawnTypeId` (restrict), `VehicleModelId` (restrict) |

### Actors and the main menu
| Table | Columns |
|---|---|
| `Actors` (`ActorModel`) | `Name` (64), `Skin`, `X/Y/Z/Angle`, `Interior`, `AnimLibrary` and `AnimName` (null = no animation), `AnimSpeed` (default 4.1), `Prompt` (the AI prompt, null = doesn't talk), `InsidePropertyId` → `Properties` (set null; null = outside), `ActorLinkedToMainMenuSceneId` → `MainMenuScenes` (cascade). No longer read: `ActorCharacterId` → `Characters` and `ActorPromptId` → `ActorPrompts`, to be dropped later |
| `ActorPrompts` (`ActorPromptModel`) | `Prompt`. No longer read: prompts moved to `Actors.Prompt` |
| `MainMenuScenes` (`MainMenuSceneModel`) | `Name` |

### Fishing
Table `FishLoot`, with the `DbSet` named `FishSpecies` (`FishLootModel`). Columns:
- `Name` (255);
- `FishLootType`: Saltwater, Freshwater, Murkywater or Misc;
- `MinWeightInGrams` and `MaxWeightInGrams`;
- `OddsToCatch`: 1 in N;
- `ItemId` → `Items`.

## Models without a table

`MapModel` (`Name`) exists, but has no `DbSet`, so it isn't part of the database. See [Maps](features/maps.md).

Want more? → [Persistence](persistence.md) · [Glossary](glossary.md)
