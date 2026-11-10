# Server events

[Server events](glossary.md#server-event) are the Framework's own events. One feature publishes one when something happens; any feature can react.

**Why:** the publisher doesn't need to know who reacts, so features stay independent and new reactions need no changes to the publisher.

## Publish and handle

```csharp
// publish, in the service where it happens
await serverEventAggregator.PublishAsync(new OnCharacterSelectedEventArgs { Player = player, Character = character });

// handle, in any IServerSystem
[ServerEvent]
public void OnCharacterSelected(OnCharacterSelectedEventArgs args, ISkillService skillService) { … }
```

## The events

| Event | When | Published by | Handled by |
|---|---|---|---|
| `OnAccountLoggedIn` | a player logged in | [Accounts](features/accounts.md) | [AccountSettings](features/account-settings.md) (load settings), [MainMenu](features/main-menu.md) (open character selection) |
| `OnCharacterSelected` | a player picked a character | [Characters](features/characters.md) | Characters (spawn), [MainMenu](features/main-menu.md) (leave scene), [Permissions](permissions.md), [Currencies](features/currencies.md) (GTA money), [Skills](features/skills.md), [DebugSettings](features/debug-settings.md), [Discord](features/discord.md) ("now playing") |
| `OnPlayerCashUpdate` | a player's money changed | [Currencies](features/currencies.md), [DroppedItems](features/dropped-items.md) | Currencies (GTA money, if default currency) |
| <a id="onfactionpermissionschanged"></a>`OnFactionPermissionsChanged` | faction permissions may have changed | [Factions](features/factions.md) | [Permissions](permissions.md) (reload those online) |
| <a id="onresourceharvested"></a>`OnResourceHarvested` | a player harvested a resource | [Harvestables](features/harvestables.md) | nothing yet: for gamemodes to give rewards |
| <a id="onpropertydeleted"></a>`OnPropertyDeleted` | an admin deleted a property | [Properties](features/properties.md) | [Actors](features/actors.md) (actors inside it move outside) |
| <a id="onjurisdictionsloaded"></a>`OnJurisdictionsLoaded` | the jurisdictions were loaded at startup | [Jurisdictions](features/jurisdictions.md) | [Vehicles](features/vehicles.md) (set plates again) |
| <a id="onjurisdictionplatetagchanged"></a>`OnJurisdictionPlateTagChanged` | a jurisdiction's tags changed, or it moved to another country | [Jurisdictions](features/jurisdictions.md) | [Vehicles](features/vehicles.md) (set plates again) |
| <a id="onjurisdictiongovernmentchanged"></a>`OnJurisdictionGovernmentChanged` | a jurisdiction got another government of its own (old and new faction id, null = none) | [Jurisdictions](features/jurisdictions.md) | nothing yet: for police, laws and taxes later |
| <a id="onjurisdictiondeleted"></a>`OnJurisdictionDeleted` | an admin deleted a jurisdiction | [Jurisdictions](features/jurisdictions.md) | [Properties](features/properties.md) and [Vehicles](features/vehicles.md) (have no jurisdiction) |
| <a id="onphoneringing"></a>`OnPhoneRinging` | a call started ringing on a phone someone carries | [Phones](features/phones.md) | Phones (opens the Answer or Decline dialog) |
| <a id="onplayerchatmessagesent"></a>`OnPlayerChatMessageSent` | a character's nearby chat was sent: /me, /do, /my, /say, /low or /shout | [Chat](shared/chat.md) | [ActorRoleplay](features/actor-roleplay.md) (actors hear it) |
| `OnPlayerInventoryItemUsed` | (unused) | nobody | nobody |

The event args live in `Shared/ServerEvents/Entities/EventArgs/`. Their properties are mostly `Player`, plus what the event is about (`Account`, `Character`, `CurrencyId`, `FactionId` + `CharacterIds`, `ResourceName`, or the chat `Type`, `Text`, `Position` and `Range`).

## Adding an event

1. Add `On<Thing>EventArgs : ServerEventArgs`.
2. Publish it.
3. Handle it.

Nothing needs registering. Handlers must not depend on running in a particular order.

Want more? → [How the dispatcher works](shared/server-events.md)
