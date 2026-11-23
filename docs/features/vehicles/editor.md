# The vehicle editor

`/createvehicle` (a new vehicle) and `/editvehicle` (the vehicle you're in) open the vehicle editor: a list of rows. Pick a row to change it. **Save** checks the changes and applies them. **Cancel** throws them away.

## The rows

| Row | What it does |
|---|---|
| Model ID | Type a GTA model id from 400 to 611. The row shows the model's name and id, for example `Infernus (411)`. An id that isn't in the database is refused. A model replaces a random spawn type, so a vehicle has one or the other. |
| Position (X, Y, Z) + Heading | Moves the vehicle to where you stand, facing where you face. When you're in a vehicle, it uses that vehicle's place. Asks first. |
| Color 1, Color 2 | Type a colour. Checked on Save. |
| Engine On, Lights On, Alarm On, Doors Locked, Bonnet Open, Boot Open | Toggle Yes or No. |
| Door and window rows | Toggle Yes or No for each door, and for each window (Window Closed). |
| Current Health, Max Health | Type a number. Checked on Save. |
| Registered in | Pick the jurisdiction the vehicle is registered in, or "Nowhere (SA plate)". See [Jurisdictions](../jurisdictions.md). |
| Radio | Installs or removes a [radio](../radios.md) at once. A new vehicle gets its radio when it's saved. |
| Owner | Type a character id, or a full name (First Last). Type None to clear it. |
| Server vehicle | Yes or No. |
| Virtual world and interior | "Use my world and interior" takes yours. Or type two numbers, for example `0 0`. |
| Give key | Gives you a key to this vehicle, in your inventory, so `/engine` works for you. An existing vehicle gets its key at once. A new vehicle toggles "Give key on save", and the key is given when you save. See [Giving a key](#giving-a-key). |
| Save | Checks the changes, then saves and applies them. |

## How it works

1. The editor works on a copy of the vehicle's data. Choosing rows changes only the copy.
2. **Save** checks the copy. A failed check shows its message in chat, and the editor stays open. Nothing is saved.
3. Before the copy is saved, every setting you didn't edit takes the vehicle's live value. So `/engine`, `/lights` and `/park` changes made while the editor was open are kept.
4. The copy is saved to the database. Then:
   - **New vehicle:** it spawns 4 m in front of you, in your virtual world and interior. You're put in the driver seat, so you can drive it away. Chat shows its id and where it is. If **Give key on save** is Yes, you also get a key for it.
   - **Existing vehicle:** only the settings that changed are applied to the live vehicle. Toggling a light never moves or repairs the car. A changed Model ID replaces the vehicle at its saved position, and anyone inside is thrown out.
5. **Cancel** or closing the editor discards the copy.

**Why:** the editor works on a copy so Cancel really cancels, and toggling an option never teleports or repairs a car. Save keeps live values for the settings you didn't edit, so a Save never reverts what players did while the editor was open.

## Checks on Save

- A model: a Model ID, or a spawn type that was set in the database.
- Color 1 and Color 2: from 0 to 255.
- Max Health: above 0. Current Health: above 0, and at most Max Health.
- Virtual world and interior: 0 or more.

If the database save fails, the chat says "The vehicle couldn't be saved", and nothing spawns.

## Giving a key

The **Give key** row gives you a key to the vehicle, in your own inventory. The key lets `/engine` start this vehicle.

| Case | Result |
|---|---|
| Existing vehicle, no key yet | The key goes into your inventory at once. Chat confirms it, and the row then says "You have a key". |
| Existing vehicle, you already have a key | Refused: "You already have a key for this vehicle." |
| Your inventory is full | Refused: "The key doesn't fit in your inventory." |
| You aren't playing a character | Refused: "You're not playing a character with an inventory." |
| The key item is missing from the database | Refused: "The vehicle key item doesn't exist." |
| New vehicle | The row shows "Give key on save: Yes" or "No". Save gives the key once the vehicle has its id, and chat reports a refusal. |

The code is `IVehicleKeyService.GiveVehicleKey(player, vehicleId)`. It returns the refusal, or null when the key was given.

**Why:** a key carries the vehicle's id, and a new vehicle has no id until it's saved.

## Server vehicles

A server vehicle gets random colours from its model's default colours each time it spawns. Those colours are never saved, so the Color rows keep the colours you set.

## Known limitations

- **Only you get a key.** The row gives a key to the admin who uses it. The vehicle's owner doesn't get one automatically.

Want more? → [Vehicles](../vehicles.md) · [Jurisdictions](../jurisdictions.md) · [Radios](../radios.md)
