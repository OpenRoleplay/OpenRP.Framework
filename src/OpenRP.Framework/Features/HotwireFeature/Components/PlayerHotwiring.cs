using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.HotwireFeature.Components
{
    public class PlayerHotwiring : Component
    {
        // Tracks the progression step (0 = just started, then 1, 2, etc.)
        public int ProgressStep { get; set; } = 0;

        public Player Player => GetComponent<Player>();

        // Ends the hotwire process on the player, restoring controllability and removing this component.
        public void EndHotwiring()
        {
            Player.ToggleControllable(true);
            Destroy();
        }
    }

}
