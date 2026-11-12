# Setting up a bank and its tellers

A teller is an AI actor that works for a bank, stands inside one of its branches, and has the actor permissions for what it may do.

## Step by step

1. **Create the bank:** `/editbanks`, then **Create a bank**: the name, the 4-letter code (it can't change later, because it's part of every OBAN), and the jurisdiction. The jurisdiction must have a currency. The bank's own house account is created with it.
2. **Set it up** in the bank's menu:
   - **Monthly fee:** 0 to 100 per account per 30 days.
   - **Run by:** the faction running the bank. Its leader and members with `faction.bank` use the house account, where the fees land.
   - **Branches:** add the bank's properties. The editor warns when a branch lies outside the bank's jurisdiction.
3. **Make the actor:** `/editactors`. Give it an AI prompt (a bank teller) and put it **inside** a branch property (Virtual world, Inside).
4. **Hire it:** in `/editbanks`, the bank's **Tellers**: add the actor nearest to you, or pick one from the list.
5. **Give it permissions:** in `/editactors`, the actor's **Permissions**:

   | Permission | Lets the teller |
   |---|---|
   | `bank.view` | show customers their accounts, and open the full menu |
   | `bank.open_account` | open accounts |
   | `bank.deposit` | take cash deposits |
   | `bank.withdraw` | pay out withdrawals |
   | `bank.transfer` | make transfers |
   | `bank.manage_access` | change who has access |
   | `bank.close_account` | close accounts |

   An actor has no permissions by default.

## When a teller won't serve

| Reason | What the customer sees |
|---|---|
| The actor isn't a teller | `/bank`: "You need to see a bank teller." |
| It stands outside its bank's branches | "… isn't at a branch of …" |
| The customer is more than 4 units away, or in another virtual world | "You need to stand at the counter …" |
| It lacks the permission | the row is missing, or "… can't help you with that here." |

Want more? → [Banking](../banking.md) · [Actors](../actors.md) · [ActorRoleplay](../actor-roleplay.md#actions)
