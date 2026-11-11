# Phones

1992-style mobile phones. Each phone is an item with its own number. You can call and text other phones, keep a phone book, and see missed calls and new messages. A phone that is on and not silent rings out loud: its carrier does a `/do` that people nearby see.

## How to use it

| Command | Groups | Does |
|---|---|---|
| `/phone` | Default | The phone menu, also opened by "Use" on the phone in your inventory. See [The phone menus](phones/menus.md) |
| `/call [number]` | Default | Calls a number, for example `/call 382-0142` or `/call 911` |
| `/sms [number] [message]` | Default | Sends a text |
| `/answer`, `/p` | Default | Answers the call ringing on your phone |
| `/hangup`, `/h` | Default | Ends your call, or declines a ringing one |
| `/givephone [player]` | Admin | Gives the player a new phone, and tells you its number |

**Talking:** in a call, just talk (or `/low`, `/shout`). People near you see `Name says (phone): text`, so they know you're on the phone, and the other side gets it as `(Phone) <name or number>: text`.

## How it works

1. **Numbers.** A new phone gets a random free 7-digit number, written `382-0142`. Numbers below 1000 belong to the server, such as [911](emergency-calls.md).
   - **Why 7 random digits:** 9 million numbers, so the server never runs out, and numbers don't give away how many phones exist.
2. **The item.** "Mobile Phone" (item 49) points to its phone with `[PHONE_ID=x]`. Only phones in your character's own inventory work (not in a wallet), and holding several, you pick one in `/phone`.
3. **Calls.**
   - **Ringing:** a call rings for 30 seconds, ringing again every 6 seconds; unanswered, it's a missed call.
   - **Who answers:** the person carrying the phone gets an Answer or Decline dialog, or uses `/answer` and `/hangup`.
   - **When it can't connect:** a number nobody has is "not in service", a phone that's off is "switched off", and a phone in a call is "busy". The caller holds the phone for about 3 seconds before hearing it, like a real call; `/hangup` stops it sooner.
     - **Why the wait:** GTA's phone animation has to start on the player's game before it can be stopped; stopping it at once left the phone stuck at the player's ear.
   - **When it ends early:** a call ends when either side drops, gives away or turns off their phone, or leaves the server.
   - **The animation:** both sides hold the phone to their ear (GTA's phone animation) while in a call.
4. **Ringing out loud.**

   | Phone | What happens |
   |---|---|
   | On | The carrier does a `/do`: "Their phone rings." for calls, "Their phone beeps twice." for texts |
   | Silent | Only the carrier sees it |
   | Off | Nothing |

5. **What people nearby see.** Using a phone is a `/me`, so others (and AI actors) see it happen. Status lines like "It's ringing" or "Nobody answers" stay private, as `(Phone)` lines.

   | Moment | Who | `/me` |
   |---|---|---|
   | Dialing | caller | takes out their phone and dials a number. |
   | Answering | the one called | takes out their phone and answers it. |
   | Declining | the one called | glances at their phone and declines the call. |
   | Hanging up | whoever hangs up | hangs up and puts the phone away. |
   | The call ends otherwise (no answer, the other side hung up, cut off, not in service, busy) | whoever still holds the phone | lowers the phone from their ear. |
   | Sending a text | sender | types out a message on their phone. |

6. **Texts** are at most 160 characters, the length of an SMS in 1992. Both phones keep a copy. Texts to a phone that's off wait, and it beeps for them when it's turned on, as 1992 networks did.
7. **Memory, like 1992.** A phone keeps its last 20 texts and 20 calls; the oldest makes room for the newest. The phone book holds 50 names.
8. **When you log in,** you're told about missed calls and new messages on the phones you carry.

**Why calls use what you say in chat:** you roleplay a phone call the way you roleplay talking, and people around you hear your side of it, as in real life.

## Known limitations

- **Same server only:** calls only connect to a phone carried on the same server. Texts reach phones on other servers within a minute.
- **Dropped phones** don't ring. Their calls become missed calls.
- **No payphones, battery or call costs yet.** For radios, see [Radios](radios.md).

## Deep dive

- **Code:** `Features/Phones/`.
- **Components:**
  - `Phone`, `PhoneContact`, `PhoneMessage` and `PhoneCallLog`: rows, loaded by their [BaseManagers](../persistence/base-manager.md) and synced every 60 seconds;
  - `PhoneCall`: a call in progress, on its own entity that only lives as long as the call.
- **Services:**
  - `IPhoneService`: reading: phones, who carries them, the phone book, texts, the call log and what's new;
  - `IPhoneEditorService`: giving phones, power, ringer, the phone book, sending texts and writing call logs, with the memory limits;
  - `IPhoneCallService`: dialing, answering, hanging up and passing on speech; `ProcessCalls` runs every second for ringing and timeouts;
  - `IPhoneAlertService`: the ring and beep `/do`s;
  - `IPhoneRoleplayService`: the `/me`s of dialing, answering, declining, hanging up, lowering the phone and texting;
  - `IPhoneDialogService`: the menus.
- **Events:**
  - **Handled:** [`OnPlayerChatMessageSent`](../server-events.md) (speech in a call) and [`OnCharacterSelected`](../server-events.md) (the news on login).
  - **Published:** [`OnPhoneRinging`](../server-events.md#onphoneringing), which opens the answer dialog.
- **The "(phone)" label:** `PhoneSpeechLabelProvider`, an [`IChatSpeechLabelProvider`](../shared/chat.md), returns "phone" while the player is in a connected call.
- **Server numbers:** a number below 1000 is an `IServerPhoneNumber`, registered with `AddServerPhoneNumber<T>()`.
  - **What it gets:** an `IServerPhoneLine` for each call, to talk back and hang up.
  - **Example:** [911](emergency-calls.md). A gamemode can add its own, such as a taxi company.
- **Numbers:** `Helpers/PhoneNumbers` formats and reads them.
- **Tables:** [Phones](../database.md#phones).

Want more? → [The phone menus](phones/menus.md) · [Emergency calls](emergency-calls.md)
