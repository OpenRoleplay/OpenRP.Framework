# Commands

All 61 player commands of the Framework, grouped as in `/help`. `[argument]` is required, `<argument>` optional.

- **Groups** are the [permission groups](permissions.md) that may use the command.
- Every command needs a selected character.
- This list is generated from the `[ServerCommand]` attributes; keep it in sync when commands change.

## Account

| Command | Groups | Description | Feature |
|---|---|---|---|
| `/settings` | Default | Change your account settings that are shared across all of your characters. | [AccountSettingsFeature](features/account-settings.md) |

## Admin

| Command | Groups | Description | Feature |
|---|---|---|---|
| `/givemoney [receiver] [currency code] [amount]` | Admin | Give money of a currency to a player. Use a negative amount to take money. | [Admins](features/admins.md) |
| `/gotopos [x] [y] [z] <interior> <virtual world>` | Admin | Teleport to a position, for example into a GTA interior. | [Admins](features/admins.md) |
| `/tprel [player] [distance] [direction]` | Admin | Move a player a distance forward, back, left, right, up or down, keeping their interior and virtual world. | [Admins](features/admins.md) |

## Admin > Bank

| Command | Groups | Description | Feature |
|---|---|---|---|
| `/editbanks` | Admin | Create banks and set their fee, branches, tellers and property accounts, or look up an OBAN. | [Banking](features/banking.md) |

## Admin > Factions

| Command | Groups | Description | Feature |
|---|---|---|---|
| `/createfaction [name]` | Admin | Create a faction with a Leader rank. Make someone its leader with /setfactionleader. | [Factions](features/factions.md) |
| `/factionpermission [faction id] [permission]` | Admin | Give a faction a permission, or take it away again. This is how top-level factions get their permissions. | [Factions](features/factions.md) |
| `/setfactionleader [target] [faction id]` | Admin | Make a player a leader of a faction, with its top rank. | [Factions](features/factions.md) |

## Admin > Jurisdictions

| Command | Groups | Description | Feature |
|---|---|---|---|
| `/editjurisdictions` | Admin | Create countries and the places in them, and set their tags, currency, government and tax registrations. | [Jurisdictions](features/jurisdictions.md) |

## Admin > Permissions

| Command | Groups | Description | Feature |
|---|---|---|---|
| `/givepermissiongroup [playerid] [permission group]` | Admin | Give a permission group to a player. | [Admins](features/admins.md) |

## Admin > Phone

| Command | Groups | Description | Feature |
|---|---|---|---|
| `/givephone [player]` | Admin | Give a player a new mobile phone with its own number. | [Phones](features/phones.md) |

## Admin > Radio

| Command | Groups | Description | Feature |
|---|---|---|---|
| `/giveradio [player]` | Admin | Give a player a new handheld radio. | [Radios](features/radios.md) |
| `/giveearpiece [player]` | Admin | Give a player a radio earpiece, so they can keep their radio traffic to themselves. | [Radios](features/radios.md) |
| `/editfrequencies` | Admin | Create protected and encrypted radio frequencies and choose the factions on them. | [Radios](features/radios.md) |

## Admin > Properties

| Command | Groups | Description | Feature |
|---|---|---|---|
| `/editproperties` | Admin | Create properties, add and link their doors, and manage them. | [Properties](features/properties.md) |
| `/editactors` | Admin | Create actors, and move and change them: name, skin, animation and which property they're in. | [Actors](features/actors.md) |

## Bank

| Command | Groups | Description | Feature |
|---|---|---|---|
| `/bank` | Default | Open the bank menu at the teller you're standing at. | [Banking](features/banking.md) |

## Character

| Command | Groups | Description | Feature |
|---|---|---|---|
| `/accent <accent>` | Default | Change your character's accent. | [RoleplayChats](features/roleplay-chats.md) |
| `/characterpreferences` | Default | View and change your character's preferences, such as Hardcore Mode and consent settings. | [Characters](features/characters.md) |
| `/skills` | Default | Show your character's skills and how far you are in each of them. | [Skills](features/skills.md) |
| `/traits` | Default | View the traits of your character. | [Characters](features/characters.md) |

## Chat

| Command | Groups | Description | Feature |
|---|---|---|---|
| `/ame [text]` | Default | Perform an action above your head. For example, /ame waves would display "Player waves" above your head to nearby players. | [RoleplayChats](features/roleplay-chats.md) |
| `/b [text]` | Default | Send a message to the local Out-Of-Character (OOC) chat. Use /b followed by your message to communicate OOCly with players in your vicinity. | [RoleplayChats](features/roleplay-chats.md) |
| `/do [text]` | Default | Describe the environment or context in the game world. For example, /do The sun sets over the hills would display "The sun sets over the hills (( Player ))" to nearby players. | [RoleplayChats](features/roleplay-chats.md) |
| `/l [text]` | Default | Short alias for /low. Example: /l [text] | [RoleplayChats](features/roleplay-chats.md) |
| `/low [text]` | Default | Send a low whisper to nearby players. For example, /low Why are we whispering? would display "Player says quietly: Why are we whispering?" in chat to nearby players. | [RoleplayChats](features/roleplay-chats.md) |
| `/me [text]` | Default | Perform an action in chat. For example, /me waves would display "Player waves" in chat to nearby players. | [RoleplayChats](features/roleplay-chats.md) |
| `/my [text]` | Default | Describe something related to your character. For example, /my face is cut would display "Player's face is cut" in chat to nearby players. | [RoleplayChats](features/roleplay-chats.md) |
| `/n [text]` | Default | Short alias for /newbie. Example: /n [text] | [RoleplayChats](features/roleplay-chats.md) |
| `/newbie [text]` | Default | Send a message to the newbie chat. Use /newbie followed by your message to ask other players server-related questions. | [RoleplayChats](features/roleplay-chats.md) |
| `/o [text]` | Default | Send a message to the global Out-Of-Character (OOC) chat. Use /o followed by your message to communicate OOCly with all players. | [RoleplayChats](features/roleplay-chats.md) |
| `/ooc [text]` | Default | Send a message to the global Out-Of-Character (OOC) chat. Use /ooc followed by your message to communicate OOCly with all players. | [RoleplayChats](features/roleplay-chats.md) |
| `/s [text]` | Default | Short alias for /shout. Example: /s [text] | [RoleplayChats](features/roleplay-chats.md) |
| `/say [text]` | Default | Send a message in In-Character (IC) proximity chat. Use /say followed by your message to communicate with players nearby your character. | [RoleplayChats](features/roleplay-chats.md) |
| `/shout [text]` | Default | Send a shout to nearby players. For example, /shout Why are we shouting? would display "Player shouts: Why are we shouting?" in chat to nearby players. | [RoleplayChats](features/roleplay-chats.md) |
| `/talk [text]` | Default | Send a message in In-Character (IC) proximity chat. Use /talk followed by your message to communicate with players nearby your character. | [RoleplayChats](features/roleplay-chats.md) |

## Currencies

| Command | Groups | Description | Feature |
|---|---|---|---|
| `/acceptpay` | Default | Accept a transaction request from another player. | [Currencies](features/currencies.md) |
| `/pay [playerid] [currency code] [amount]` | Default | Pay money to another player. | [Currencies](features/currencies.md) |

## Debug

| Command | Groups | Description | Feature |
|---|---|---|---|
| `/debug` | Tester, Admin | Open the debug settings dialog. | [DebugSettingsFeature](features/debug-settings.md) |

## Factions

| Command | Groups | Description | Feature |
|---|---|---|---|
| `/faction` | Default | Open the menu of your faction: members, ranks, invitations and sub-factions. | [Factions](features/factions.md) |

## Factions > Chat

| Command | Groups | Description | Feature |
|---|---|---|---|
| `/fo [text]` | Faction | Send an out-of-character message to the online members of your active faction. Short for /fooc. | [Factions](features/factions.md) |
| `/fooc [text]` | Faction | Send an out-of-character message to the online members of your active faction. | [Factions](features/factions.md) |

## Farming

| Command | Groups | Description | Feature |
|---|---|---|---|
| `/collectegg` | Default | Collect an egg from the chicken coop you're at. Crouch so you don't scare the chickens. | [ChickenCoops](features/chicken-coops.md) |

## Gathering

| Command | Groups | Description | Feature |
|---|---|---|---|
| `/harvest [resource]` | Default | Harvest a resource. | [Harvestables](features/harvestables.md) |

## Info

| Command | Groups | Description | Feature |
|---|---|---|---|
| `/help` | Default | Show a list of all commands you have access to. | [Commands (shared)](shared/commands.md) |

## Inventory

| Command | Groups | Description | Feature |
|---|---|---|---|
| `/pickup` | Default | Pickup a dropped item from the ground. | [DroppedItems](features/dropped-items.md) |

## Jobs > Fishing

| Command | Groups | Description | Feature |
|---|---|---|---|
| `/fish` | Default | Command to start fishing. | [Fishing](features/fishing.md) |

## Phone

| Command | Groups | Description | Feature |
|---|---|---|---|
| `/phone` | Default | Use your phone: dial, the phone book, messages, the call log, the ringer and power. | [Phones](features/phones.md) |
| `/call [number]` | Default | Call a phone number, for example /call 382-0142 or /call 911. | [Phones](features/phones.md) |
| `/sms [number] [message]` | Default | Send a text message. | [Phones](features/phones.md) |
| `/answer`, `/p` | Default | Answer the call ringing on your phone. | [Phones](features/phones.md) |
| `/hangup`, `/h` | Default | End your call, or decline the call ringing on your phone. | [Phones](features/phones.md) |

## Radio

| Command | Groups | Description | Feature |
|---|---|---|---|
| `/radio` | Default | Use your radio, or the one in your vehicle: power, frequency and earpiece. | [Radios](features/radios.md) |
| `/r [message]` | Default | Talk over your radio, for example /r Unit 12 en route. | [Radios](features/radios.md) |
| `/freq [frequency]` | Default | Tune your radio, or the one in your vehicle, for example /freq 1500. | [Radios](features/radios.md) |

## Properties

| Command | Groups | Description | Feature |
|---|---|---|---|
| `/enter` | Default | Go through the property door you're standing at. | [Properties](features/properties.md) |
| `/exit` | Default | Go through the property door you're standing at. | [Properties](features/properties.md) |

## Tester

| Command | Groups | Description | Feature |
|---|---|---|---|
| `/spawnveh [model] <first color> <second color>` | Admin, Tester | Spawn a vehicle. Use /spawnveh followed by the vehicle name or ID to generate a new vehicle for your use. | [Admins](features/admins.md) |
| `/spawnvehicle [model] <first color> <second color>` | Admin, Tester | Spawn a vehicle. Use /sv followed by the vehicle name or ID to generate a new vehicle for your use. | [Admins](features/admins.md) |
| `/sv [model] <first color> <second color>` | Admin, Tester | Spawn a vehicle. Use /sv followed by the vehicle name or ID to generate a new vehicle for your use. | [Admins](features/admins.md) |

## Vehicles > General

| Command | Groups | Description | Feature |
|---|---|---|---|
| `/e` | Default | Toggle your vehicle's engine status. Use /e to toggle the engine on or off. | [Vehicles](features/vehicles.md) |
| `/engine` | Default | Toggle your vehicle's engine status. Use /engine to toggle the engine on or off. | [Vehicles](features/vehicles.md) |
| `/eoff` | Default | Toggle your vehicle's engine status. Use /eoff to turn the engine off. | [Vehicles](features/vehicles.md) |
| `/eon` | Default | Toggle your vehicle's engine status. Use /eon to turn the engine on. | [Vehicles](features/vehicles.md) |
| `/hotwire` | Default | Attempt to hotwire a vehicle. This allows you to start a car without a key, provided you have the necessary tools. | [HotwireFeature](features/hotwire.md) |
| `/lights` | Default | Control your vehicle's lights. Use /lights to toggle the headlights. | [Vehicles](features/vehicles.md) |
| `/loff` | Default | Control your vehicle's lights. Use /loff to turn them off. | [Vehicles](features/vehicles.md) |
| `/lon` | Default | Control your vehicle's lights. Use /lon to turn them on. | [Vehicles](features/vehicles.md) |

## Vehicles > Ownership

| Command | Groups | Description | Feature |
|---|---|---|---|
| `/park` | Default | Park your currently owned vehicle. Use /park to save your vehicle in a new parking spot. | [Vehicles](features/vehicles.md) |
| `/savecar` | Default | Park your currently owned vehicle. Use /savecar to save your vehicle in a new parking spot. | [Vehicles](features/vehicles.md) |

## Ungrouped

| Command | Groups | Description | Feature |
|---|---|---|---|
| `/createvehicle` | Admin | Create a persistent vehicle that can be owned. | [Vehicles](features/vehicles.md) |
| `/editvehicle` | Admin | Edit a persistent vehicle that can be owned. | [Vehicles](features/vehicles.md) |
| `/inv` | Default | Open your character's inventory. Use this command to view, manage, and interact with the items you possess. | [Inventories](features/inventories.md) |
| `/inventory` | Default | Open your character's inventory. Use this command to view, manage, and interact with the items you possess. | [Inventories](features/inventories.md) |
| `/permissions` | Default | Display your own permissions. Use this command to view a list of all permissions currently assigned to your character. | [Permissions](features/permissions.md) |
| `/setweathertime [weather] [hour] [minutes]` | Admin | Allows you to override the weather and/or time. | [Admins](features/admins.md) |
| `/showpermissions [target]` | Admin | Display another player's permissions. Use this admin command to view the permissions assigned to a specified player. | [Permissions](features/permissions.md) |
| `/testerkit` | Tester, Admin | Ensures you have the hotwire tools: a Rusty Paperclip, a Wornout Screwdriver and Electrical Tape. | [Admins](features/admins.md) |

Want more? → [Permissions](permissions.md) · [How commands work](shared/commands.md)
