using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.MainMenu.Entities
{
    public interface IMainMenuScene
    {
        void Prepare();
        void Play(Player player);

        /// <summary>
        /// Stops everything this scene started for the player, such as music and actor conversations.
        /// Safe to call for a player who isn't watching this scene.
        /// </summary>
        void Leave(Player player);
    }
}
