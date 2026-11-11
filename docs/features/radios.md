# Radios

Two-way radios on numbered frequencies: a handheld radio item, and a radio an admin installs in a vehicle. Everyone on the same frequency hears a transmission, and people standing near a radio overhear it, unless its holder uses an earpiece. [911 dispatch](emergency-calls.md) broadcasts on frequency 911.

## How to use it

| Command | Groups | Does |
|---|---|---|
| `/radio` | Default | The radio menu: power, frequency, Speaker or Earpiece, and the battery. Holding several radios, you pick one first. "Use" on a radio in your inventory opens that radio's menu |
| `/r [message]` | Default | Says the message out loud and sends it over your radio |
| `/freq [frequency]` | Default | Tunes your radio, for example `/freq 1500` |
| `/giveradio [player]` | Admin | Gives the player a new handheld radio, tuned to 1000 |
| `/giveearpiece [player]` | Admin | Gives the player a radio earpiece |
| `/editfrequencies` | Admin | Creates protected and encrypted frequencies and picks their factions. See [Frequencies](radios/frequencies.md) |

**Vehicle radios:** an admin sets the **Radio** row in [`/createvehicle` or `/editvehicle`](vehicles.md). Everyone inside can then use `/radio`, `/freq` and `/r`.

## Who can use a frequency

| Frequency | Listen | Talk |
|---|---|---|
| 1000 to 9999, no rules set | Anyone with a radio | Anyone with a radio |
| Protected | Anyone with a radio | Members of its factions |
| Encrypted | Members of its factions | Members of its factions |
| Anything else | Nobody: no signal | Nobody |

- **Members** are characters in one of the frequency's factions, or in a faction it controls (a sub-faction). It's checked on every transmission, so joining or leaving a faction counts at once.
- **Protected and encrypted frequencies** can use any number from 1 to 99999, also inside 1000 to 9999.
- **Why protected exists:** in 1992 most police and fire radio wasn't encrypted. Anyone could listen in, but only the agency could transmit.

## What people see

Say Unit 12 types `/r Shots fired on Grove Street` on frequency 911:

| Who | Sees |
|---|---|
| People near Unit 12 | `Unit 12 says (radio): Shots fired on Grove Street`, like normal talk (7 units) |
| Holders of other radios on 911 | `[Radio 911 (911 Dispatch)] Unit 12: Shots fired on Grove Street` |
| People within 4 units of another radio playing out loud | `(Radio) Shots fired on Grove Street`, more muffled further away |

- **Holders:** whoever carries a handheld, or everyone inside a vehicle with a radio.
- **Unit 12** sees their own radio line too, so they know it went out. Their own radio doesn't play it out loud.
- **Talking on the phone:** `/r` is also heard by a phone call you're on, because you say it out loud.

**Which radios play out loud:**

| Radio | Plays from | Out loud? |
|---|---|---|
| A carried handheld | Its carrier | Yes, unless it's set to Earpiece and the carrier has an earpiece |
| A dropped handheld that's on | Where it lies | Yes |
| A vehicle radio | The vehicle, even when it's empty | Yes |

- **The earpiece** is its own item. Only while you carry it does `/radio` offer Speaker or Earpiece. Drop it and your radio plays out loud again.
  - **Why an item:** earpieces were a cheap accessory in 1992, so keeping traffic private costs one.
- **Encrypted frequencies** only play on a radio that a member holds or sits with. Bystanders still overhear it from the speaker, because the radio decrypts it.

## Battery

A handheld runs on a battery, in real time, like a 1992 police handheld. Vehicle radios run on the car and have none.
- **How long:** a full battery lasts about one 10-hour shift switched on, and each `/r` costs a little extra (0.25%). Off, it doesn't drain.
- **Warnings:** a `/do` at 10%, 5% and 2% ("Their radio chirps: low battery."). At 0% it crackles, goes dead and switches off.
- **Recharge:** the **Battery** row in `/radio`, in up to a minute from empty. The radio stays usable meanwhile. A dead radio can't be turned on until it's recharged.
- **Offline:** in your inventory while you're offline, it recharges after the first hour, from empty to full in 24 hours. A switched-off radio never recharges while you play.
- See [Batteries](../shared/batteries.md) for the exact rules.

## Known limitations

- **One frequency per radio.** No scanning or channel lists.
- **Same server only:** a transmission reaches radios on the same server.
- **Vehicle radios** are installed by admins only, and only in saved vehicles.
- **No animation** for talking into a radio.
- **Dropped radios** play in every virtual world, because dropped items aren't tied to one.
- **Drop models:** the radio uses object 19942 and the earpiece object 19421. Check them in game.

## Deep dive

- **Code:** `Features/Radios/`.
- **Components:** `Radio`, `RadioFrequency` and `RadioFrequencyFaction`: rows, loaded by their [BaseManagers](../persistence/base-manager.md) and synced every 60 seconds.
- **The items:** "Handheld Radio" (item 50) points to its radio with `[RADIO_ID=x]`. "Radio Earpiece" is item 51. Only items in your character's own inventory count (not in a wallet).
- **Services:**
  - `IRadioFrequencyService`: the rules: `CanListen`, `GetTalkError` and labels;
  - `IRadioService`: finding radios: carried, in your vehicle, and every radio playing a frequency (`GetPlayingRadios`);
  - `IRadioBroadcastService`: `TransmitFromPlayer` and `TransmitFromSystem`, which other features use to talk on the radio, like 911;
  - `IRadioEditorService`: giving radios, tuning, frequencies and vehicle radios;
  - `IRadioDialogService`: the menus.
- **Events:** publishes [`OnRadiosLoaded`](../server-events.md#onradiosloaded) after loading, so features can create the frequencies they need, and handles [`OnPlayerInventoryItemUsed`](../server-events.md#onplayerinventoryitemused) to open the menu.
- **Tables:** [Radios](../database.md#radios).

Want more? → [Frequencies](radios/frequencies.md) · [911](emergency-calls.md) · [Phones](phones.md)
