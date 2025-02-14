using OpenRP.Framework.Shared;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Systems
{
    public class OpenRoleplayFrameworkSystem : ISystem
    {
        [Event]
        public void OnGameModeInit(IServerService serverService)
        {
            Console.WriteLine(@"
            ==================================================
               Loading Open Roleplay (open-roleplay.com)
                          by Koert Lichtendonk
            --------------------------------------------------
               Licensed under GNU LGPL v3
               (http://www.gnu.org/licenses/lgpl-3.0.html)
            ==================================================
            ");

            /*
             * Open Roleplay (open-roleplay.com) by Koert Lichtendonk
             * Licensed under GNU LGPL v3
             * (http://www.gnu.org/licenses/lgpl-3.0.html)
             *
             * NOTICE: Under the GNU LGPL v3, you must retain the above credits and
             * license notice in any redistributed or modified versions of this library.
             * This includes ensuring that these credits remain clearly visible in any
             * user interface. Do not remove, alter, or hide these credits—this means
             * you must not, for example, obscure them by spamming additional messages
             * or otherwise modifying their presentation.
             */
        }

        [Event]
        public void OnPlayerConnect(Player player)
        {
            for (int i = 0; i < 25; i++)
            {
                player.SendClientMessage(string.Empty);
            }
            player.SendClientMessage(string.Format("{1}Welcome to {0}Open Roleplay {1}({0}open-roleplay.com{1}) by {0}Koert Lichtendonk{1}.", ChatColor.Main, ChatColor.White, ChatColor.Highlight));
            player.SendClientMessage(string.Format("{1}Licensed under {0}GNU LGPL v3{1} ({2}http://www.gnu.org/licenses/lgpl-3.0.html{1})", ChatColor.Main, ChatColor.White, ChatColor.Highlight));

            /*
             * Open Roleplay (open-roleplay.com) by Koert Lichtendonk
             * Licensed under GNU LGPL v3
             * (http://www.gnu.org/licenses/lgpl-3.0.html)
             *
             * NOTICE: Under the GNU LGPL v3, you must retain the above credits and
             * license notice in any redistributed or modified versions of this library.
             * This includes ensuring that these credits remain clearly visible in any
             * user interface. Do not remove, alter, or hide these credits—this means
             * you must not, for example, obscure them by spamming additional messages
             * or otherwise modifying their presentation.
             */
        }
    }
}
