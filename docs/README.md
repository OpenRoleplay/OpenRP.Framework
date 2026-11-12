# OpenRP.Framework docs

OpenRP.Framework is the open-source (LGPL v3) gameplay library behind Open Roleplay, a roleplay gamemode for open.mp written in C#. A server references it, and adds what's specific to that server.

## Start here

- [Getting started](getting-started.md): run a server on the Framework.
- [Architecture](architecture.md): how the code is organised; read this before changing it.
- [Glossary](glossary.md): what words like entity, component and host mean.

## Guides

- [Persistence](persistence.md): how data is loaded and saved, and why.
- [Permissions](permissions.md): who may use which command, including through factions.
- [Server events](server-events.md): how features react to each other.

## Look-ups

- [Commands](commands.md): every command, with its syntax.
- [Database](database.md): every table and column.

## Features

- [Accounts](features/accounts.md): register, log in, one login per account
- [AccountSettings](features/account-settings.md): chat toggles and `/settings`
- [Actors](features/actors.md): non-player characters in the world, created and edited in-game with `/editactors`
- [Banking](features/banking.md): banks, accounts with OBANs, AI bank tellers, `/bank` and `/editbanks`
- [ActorConversations](features/actor-conversations.md): AI conversations between actors
- [ActorRoleplay](features/actor-roleplay.md): AI actors that answer players' /me, /do and /say
- [Admins](features/admins.md): admin and tester commands
- [Animations](features/animations.md): preloads animations
- [BiomeGenerator](features/biome-generator.md): biomes and objects for generated landscapes
- [CDN](features/cdn.md): plays audio
- [Characters](features/characters.md): create, select and save characters; consent
- [ChickenCoops](features/chicken-coops.md): collect eggs
- [Currencies](features/currencies.md): money as bills and coins, `/pay`
- [DebugSettings](features/debug-settings.md): debug messages for testers
- [Discord](features/discord.md): chat bridge and stats channels
- [DroppedItems](features/dropped-items.md): items on the ground
- [EmergencyCalls](features/emergency-calls.md): 911 with an AI dispatcher that relays calls on the radio
- [Factions](features/factions.md): organisations with ranks and permissions
- [Fishing](features/fishing.md): early fishing
- [Harvestables](features/harvestables.md): harvestable plants
- [Hotwire](features/hotwire.md): start a vehicle without a key
- [Inventories](features/inventories.md): weight-based inventories
- [Items](features/items.md): item types
- [Jurisdictions](features/jurisdictions.md): countries and cities with tags, currency, government and tax registrations; `/editjurisdictions`
- [MainMenu](features/main-menu.md): what you see after connecting
- [Maps](features/maps.md): placeholder, not working yet
- [Permissions](features/permissions.md): see the Permissions guide
- [Phones](features/phones.md): 1992-style mobile phones: calls, texts, phone book, call log
- [Players](features/players.md): find a player's account and character
- [Properties](features/properties.md): buildings, doors and the property editor
- [Radios](features/radios.md): handheld and vehicle radios on frequencies, earpieces, `/editfrequencies`
- [RoleplayChats](features/roleplay-chats.md): `/me`, `/do`, talk, OOC
- [Skills](features/skills.md): character skills
- [Vehicles](features/vehicles.md): saved vehicles, keys, parking
- [VirtualWorlds](features/virtual-worlds.md): hands out virtual worlds
- [WorldTime](features/world-time.md): the in-game clock
- [WorldWeather](features/world-weather.md): weather by climate

## Shared code

- [Chat](shared/chat.md): messages to players
- [How commands work](shared/commands.md): writing commands
- [Dialogs](shared/dialogs.md): menus
- [Batteries](shared/batteries.md): battery levels of phones and radios: draining, recharging and offline charging
- [Locations](shared/locations.md): the name of the area a position is in
- [Logging](shared/logging.md): where logs go
- [Extensions and colours](shared/extensions.md): small helpers

When you change code, update its page in the same change.
