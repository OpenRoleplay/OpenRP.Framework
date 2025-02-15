using OpenRP.Framework.Features.Accounts.Components;
using OpenRP.Framework.Features.Characters.Components;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Players.Extensions
{
    public static partial class PlayerExtensions
    {
        public static bool IsPlayerLoggedInAccount(this Player player)
        {
            Account accountComponent = player.GetComponent<Account>();

            if (accountComponent != null)
            {
                return true;
            }
            return false;
        }

        public static Account GetPlayerCurrentlyLoggedInAccount(this Player player)
        {
            Account accountComponent = player.GetComponent<Account>();

            if (accountComponent != null)
            {
                return accountComponent;
            }
            return null;
        }

        public static bool IsPlayerPlayingAsCharacter(this Player player)
        {
            Character character = player.GetComponent<Character>();

            if (character != null)
            {
                return true;
            }

            return false;
        }

        public static Character GetPlayerCurrentlyPlayingAsCharacter(this Player player)
        {
            Character character = player.GetComponent<Character>();

            return character;
        }
    }
}
